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
    }
    public void End(Vector3 pos)
    {
        if (pos.y != startNotation.transform.position.y)
            return;

        if (endNotation != null)
            DestroyNotation(endNotation);

        endNotation = CommonFunction.Instantiate("Prefabs/Game/Order/Notation", null, pos);

        ChangeNotationState();

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBuilderPencilBtn();
    }

    public void CreateBlueprint()
    {
        var startPos = startNotation.transform.position;
        var endPos = endNotation.transform.position;

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

        Debug.LogFormat("s:{0}, n:{1}",startPos, endPos);

        List<MapBlockData> blocks = new List<MapBlockData>();
        for (int y = (int)startPos.y; y < DataMng.E.MapData.MapSize.y - startPos.y; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                for (int z = minZ; z < maxZ; z++)
                {
                    if (DataMng.E.MapData.Map[x, y, z] == null)
                        continue;

                    var block = DataMng.E.MapData.Map[x, y, z];
                    block.Pos = new Vector3Int(x - minX - centerX, y - (int)startPos.y, z - minZ - centerZ);
                    block.ClearBlock();
                    blocks.Add(block);
                }
            }
        }

        var blueprintData = new BlueprintData(blocks, new Vector2Int(maxX - minX, maxZ - minZ));

        PlayerCtl.E.AddItem(3001, 1, blueprintData);

        CancelCreateBlueprint();
    }
    public void CancelCreateBlueprint()
    {
        if (startNotation != null) DestroyNotation(startNotation);
        if (endNotation != null) DestroyNotation(endNotation);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBuilderPencilBtn(false);
    }

    public void UseBlueprint(Vector3Int startPos, object data)
    {
        Debug.Log("UserBlueprint");

        if (selectBlueprintData == null)
            selectBlueprintData = new BlueprintData(data);

        WorldMng.E.MapCtl.DeleteBuilderPencil();
        ClearSelectBlueprintDataBlock();
        buildPos = startPos;
        selectBlueprintData.IsDuplicate = false;

        WorldMng.E.MapCtl.CreateTransparentBlocks(selectBlueprintData, buildPos);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBlueprintBtn();
    }
    public void CancelUserBlueprint()
    {
        Debug.Log("CancelUserBlueprint");

        WorldMng.E.MapCtl.DeleteBuilderPencil();
        selectBlueprintData = null;

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBlueprintBtn(false);
    }
    public void SpinBlueprint()
    {
        Debug.Log("SpinBlueprint");

        WorldMng.E.MapCtl.DeleteBuilderPencil();
        ClearSelectBlueprintDataBlock();
        selectBlueprintData.IsDuplicate = false;

        for (int i = 0; i < selectBlueprintData.BlockList.Count; i++)
        {
            var work = selectBlueprintData.BlockList[i];
            work.Pos = new Vector3Int(work.Pos.z, work.Pos.y, work.Pos.x);
        }

        for (int i = 0; i < selectBlueprintData.BlockList.Count; i++)
        {
            var work = selectBlueprintData.BlockList[i];
            work.Pos = new Vector3Int(work.Pos.x, work.Pos.y, -work.Pos.z);
        }

        WorldMng.E.MapCtl.CreateTransparentBlocks(selectBlueprintData, buildPos);
    }
    public void BuildBlueprint()
    {
        Debug.Log("BuildBlueprint");

        if (!selectBlueprintData.IsDuplicate)
        {
            Dictionary<int, int> consumableItems = new Dictionary<int, int>();

            foreach (MapBlockData item in selectBlueprintData.BlockList)
            {
                if (consumableItems.ContainsKey(item.ItemID))
                {
                    consumableItems[item.ItemID]++;
                }
                else
                {
                    consumableItems[item.ItemID] = 1;
                }
            }

            var ret = PlayerCtl.E.ConsumableItems(consumableItems);
            if (!ret)
            {
                Debug.Log("持っているアイテム数が足りないです。");
                return;
            }

            // ブロックを作る
            WorldMng.E.MapCtl.CreateBlocks(selectBlueprintData, buildPos);

            // 設計図を消耗
            PlayerCtl.E.ConsumableSelectItem();

            CancelUserBlueprint();
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
        
        foreach (var item in selectBlueprintData.BlockList)
        {
            item.ClearBlock();
        }
    }
}