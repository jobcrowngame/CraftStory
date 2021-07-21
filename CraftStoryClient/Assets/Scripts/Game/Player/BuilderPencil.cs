using JsonConfigData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BuilderPencil
{
    GameObject startNotation;
    GameObject endNotation;

    BlueprintData selectBlueprintData;
    Vector3Int buildPos;

    public bool IsStart { get => startNotation == null; }

    public void Start(Vector3 pos)
    {
        if (startNotation != null)
            DestroyNotation(startNotation);

        startNotation = CommonFunction.Instantiate("Prefabs/Game/Order/Notation", null, pos);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBuilderPencilBtn();
    }
    public void End(Vector3 pos)
    {
        if (pos.y != startNotation.transform.position.y)
            return;

        if (endNotation != null)
            DestroyNotation(endNotation);

        endNotation = CommonFunction.Instantiate("Prefabs/Game/Order/Notation", null, pos);

        ChangeNotationState();
    }

    public void CreateBlueprint()
    {
        var startPos = startNotation.transform.position;
        var endPos = endNotation.transform.position;

        var centPos = new Vector3((startPos.x + endPos.x) / 2, startPos.y, (startPos.z + endPos.z) / 2);

        int minX, maxX, minZ, maxZ;
        if (startPos.x >= endPos.x)
        {
            minX = (int)endPos.x;
            maxX = (int)startPos.x + 1;
        }
        else
        {
            minX = (int)startPos.x;
            maxX = (int)endPos.x + 1; 
        }

        if (startPos.z >= endPos.z)
        {
            minZ = (int)endPos.z;
            maxZ = (int)startPos.z + 1;
        }
        else
        {
            minZ = (int)startPos.z;
            maxZ = (int)endPos.z + 1;
        }

        int centerX = (maxX - minX) / 2;
        int centerZ = (maxZ - minZ) / 2;

        Logger.Log("s:{0}, n:{1}",startPos, endPos);

        List<EntityBase> entitys = new List<EntityBase>();
        for (int y = (int)startPos.y; y < DataMng.E.MapData.MapSize.y - startPos.y; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                for (int z = minZ; z < maxZ; z++)
                {
                    if (DataMng.E.MapData.Map[x, y, z].entityID < 1)
                        continue;

                    var entity = DataMng.E.MapData.GetEntity(new Vector3Int(x, y, z));
                    if(entity != null) entitys.Add(entity);
                }
            }
        }

        var blueprintData = new BlueprintData(entitys, new Vector2Int(maxX - minX, maxZ - minZ), Vector3Int.CeilToInt(centPos));

        var ui = UICtl.E.OpenUI<BlueprintReNameUI>(UIType.BlueprintReName);
        ui.SetMapData(blueprintData.ToJosn());

        Debug.Log(ui.mapData);

        CancelCreateBlueprint();
    }
    public void CancelCreateBlueprint()
    {
        if (startNotation != null) DestroyNotation(startNotation);
        if (endNotation != null) DestroyNotation(endNotation);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBuilderPencilBtn(false);
    }

    public void UseBlueprint(Vector3Int startPos, string data)
    {
        Logger.Log("UserBlueprint");

        selectBlueprintData = new BlueprintData(data);

        WorldMng.E.MapCtl.DeleteBuilderPencil();
        ClearSelectBlueprintDataBlock();
        buildPos = startPos;
        selectBlueprintData.IsDuplicate = false;

        if (!selectBlueprintData.CheckPos(startPos))
        {
            CancelUserBlueprint();
            return;
        }

        // 半透明ブロックを作る
        WorldMng.E.MapCtl.InstantiateTransparenEntitys(selectBlueprintData, buildPos);

        // コストブロックを設定
        HomeLG.E.UI.AddBlueprintCostItems(selectBlueprintData);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBlueprintBtn();
    }
    public void CancelUserBlueprint()
    {
        Logger.Log("CancelUserBlueprint");

        WorldMng.E.MapCtl.DeleteBuilderPencil();
        selectBlueprintData = null;

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBlueprintBtn(false);
    }
    public void SpinBlueprint()
    {
        Logger.Log("SpinBlueprint");

        WorldMng.E.MapCtl.DeleteBuilderPencil();
        ClearSelectBlueprintDataBlock();
        selectBlueprintData.IsDuplicate = false;

        for (int i = 0; i < selectBlueprintData.blocks.Count; i++)
        {
            var work = selectBlueprintData.blocks[i];
            work.SetPos(new Vector3Int(work.GetPos().z, work.GetPos().y, work.GetPos().x));
            work.direction += 90;
        }

        for (int i = 0; i < selectBlueprintData.blocks.Count; i++)
        {
            var work = selectBlueprintData.blocks[i];
            work.SetPos(new Vector3Int(work.GetPos().x, work.GetPos().y, -work.GetPos().z));
        }

        // チェックPos
        foreach (var item in selectBlueprintData.blocks)
        {
            var newPos = CommonFunction.Vector3Sum(item.GetPos(), buildPos);
            if (MapCtl.IsOutRange(DataMng.E.MapData, newPos))
            {
                CommonFunction.ShowHintBar(8);
                CancelUserBlueprint();
                return;
            }
        }

        WorldMng.E.MapCtl.InstantiateTransparenEntitys(selectBlueprintData, buildPos);
    }
    public void BuildBlueprint()
    {
        Logger.Log("BuildBlueprint");

        if (!selectBlueprintData.IsDuplicate)
        {
            Dictionary<int, int> consumableItems = new Dictionary<int, int>();

            foreach (var entity in selectBlueprintData.blocks)
            {
                int itemId = ConfigMng.E.Entity[entity.id].ItemID;
                if (consumableItems.ContainsKey(itemId))
                {
                    consumableItems[itemId]++;
                }
                else
                {
                    consumableItems[itemId] = 1;
                }
            }

            var ret = PlayerCtl.E.ConsumableItems(consumableItems);
            if (!ret)
            {
                CommonFunction.ShowHintBar(1);
                return;
            }

            // ブロックを作る
            WorldMng.E.MapCtl.InstantiateEntitys(selectBlueprintData, buildPos);

            // 設計図を消耗
            PlayerCtl.E.ConsumableSelectItem();

            CancelUserBlueprint();
        }
        else
        {
            CommonFunction.ShowHintBar(8);
        }
    }

    private void DestroyNotation(GameObject notation)
    {
        if (notation != null)
            GameObject.Destroy(notation);
    }
    private void ChangeNotationState()
    {
        var posX = endNotation.transform.position.x - startNotation.transform.position.x;
        var posZ = endNotation.transform.position.z - startNotation.transform.position.z;

        var startBarX = CommonFunction.FindChiledByName(startNotation.transform, "X");
        var startBarZ = CommonFunction.FindChiledByName(startNotation.transform, "Z");
        var endBarX = CommonFunction.FindChiledByName(endNotation.transform, "X");
        var endBarZ = CommonFunction.FindChiledByName(endNotation.transform, "Z");

        if (posX >=0 && posZ >= 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
        }
        else if (posX >= 0 && posZ < 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
        }
        else if (posX < 0 && posZ >= 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
        }
        else if (posX < 0 && posZ < 0)
        {
            startNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            endNotation.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            ChangePosAndScaleX(startBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(startBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
            ChangePosAndScaleX(endBarX, Mathf.Abs(posX) / 2, Mathf.Abs(posX));
            ChangePosAndScaleZ(endBarZ, Mathf.Abs(posZ) / 2, Mathf.Abs(posZ));
        }
    }
    private void ChangePosAndScaleX(GameObject obj, float posX, float scale)
    {
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, scale + 1);
        obj.transform.localPosition = new Vector3(posX, obj.transform.localPosition.y, obj.transform.localPosition.z);
    }
    private void ChangePosAndScaleZ(GameObject obj, float posZ, float scale)
    {
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, scale + 1);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, posZ);
    }
    private void ClearSelectBlueprintDataBlock()
    {
        if (selectBlueprintData == null)
            return;
        
        //foreach (var item in selectBlueprintData.EntityList)
        //{
        //    item.ClearObj();
        //}
    }
}