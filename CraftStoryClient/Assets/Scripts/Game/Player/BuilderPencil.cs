using JsonConfigData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 設計図ビルダーペンセル
/// </summary>
public class BuilderPencil
{
    /// <summary>
    /// 始点標記GameObject
    /// </summary>
    GameObject startNotation;

    /// <summary>
    /// 終点標記GameObject
    /// </summary>
    GameObject endNotation;

    /// <summary>
    /// 選択された設計図データ
    /// </summary>
    BlueprintData selectBlueprintData;

    /// <summary>
    /// 作成する座標
    /// </summary>
    Vector3Int buildPos;

    /// <summary>
    /// 始点があるかのチェック
    /// </summary>
    public bool IsStart { get => startNotation == null; }

    /// <summary>
    /// 始点を設定
    /// </summary>
    /// <param name="pos">座標</param>
    public void Start(Vector3 pos)
    {
        if (startNotation != null)
            DestroyNotation(startNotation);

        startNotation = CommonFunction.Instantiate("Prefabs/Game/Order/Notation", null, pos);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBuilderPencilBtn();

        GuideLG.E.Next();
    }

    /// <summary>
    /// 終点を設定
    /// </summary>
    /// <param name="pos">座標</param>
    public void End(Vector3 pos)
    {
        if (pos.y != startNotation.transform.position.y)
            return;

        if (endNotation != null)
            DestroyNotation(endNotation);

        endNotation = CommonFunction.Instantiate("Prefabs/Game/Order/Notation", null, pos);

        ChangeNotationState();

        GuideLG.E.Next();
    }

    /// <summary>
    /// 設計図を作成
    /// </summary>
    public void CreateBlueprint()
    {
        if (startNotation == null || endNotation == null)
            return;

        var startPos = startNotation.transform.position;
        var endPos = endNotation.transform.position;
        var centPos = new Vector3((startPos.x + endPos.x) / 2, startPos.y, (startPos.z + endPos.z) / 2);

        // 始点、終点の座標によって設計図内容座標を決める
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

        Logger.Log("s:{0}, n:{1}",startPos, endPos);

        // 含まれたEntityを検索
        List<EntityBase> entitys = new List<EntityBase>();
        for (int y = (int)startPos.y; y < DataMng.E.MapData.GetMapSize().y; y++)
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

        // 設計図データを生成
        var blueprintData = new BlueprintData(entitys, new Vector2Int(maxX - minX, maxZ - minZ), Vector3Int.CeilToInt(centPos));
        if (blueprintData.blocks.Count > 0)
        {
            // 設計図名を登録Windowを呼び出す
            var ui = UICtl.E.OpenUI<BlueprintReNameUI>(UIType.BlueprintReName);
            ui.SetMapData(blueprintData.ToJosn());
        }
        else
        {
            // 設計図内容が空の場合、エラーメッセージを出す
            CommonFunction.ShowHintBar(26);
        }

        CancelCreateBlueprint();
    }

    /// <summary>
    /// 作成した設計図をキャンセル
    /// </summary>
    public void CancelCreateBlueprint()
    {
        if (startNotation != null) DestroyNotation(startNotation);
        if (endNotation != null) DestroyNotation(endNotation);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBuilderPencilBtn(false);
    }

    /// <summary>
    /// 設計図を使用
    /// </summary>
    /// <param name="startPos">作成点</param>
    /// <param name="data">設計図内容</param>
    public void UseBlueprint(Vector3Int startPos, string data)
    {
        // 設計図内容によって、設計図データを生成
        selectBlueprintData = new BlueprintData(data);

        // 残ってる設計図エンティティを作成
        WorldMng.E.MapCtl.DeleteBuilderPencil();

        // 
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

        GuideLG.E.Next();
    }
    /// <summary>
    /// 使用した設計図をキャンセル
    /// </summary>
    public void CancelUserBlueprint()
    {
        WorldMng.E.MapCtl.DeleteBuilderPencil();
        selectBlueprintData = null;

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.ShowBlueprintBtn(false);

        GuideLG.E.Next(21);
    }

    /// <summary>
    /// 設計図向きを変換
    /// </summary>
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

    /// <summary>
    /// 使用した設計図をインスタンス
    /// </summary>
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
            PlayerCtl.E.UseItem();

            CancelUserBlueprint();
        }
        else
        {
            CommonFunction.ShowHintBar(8);
        }
    }

    /// <summary>
    /// 標記を削除
    /// </summary>
    /// <param name="notation">標記</param>
    private void DestroyNotation(GameObject notation)
    {
        if (notation != null)
            GameObject.Destroy(notation);
    }

    /// <summary>
    /// 選択範囲によって始点、終点の長さを調整
    /// </summary>
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
    /// <summary>
    /// 始点、終点座標X、大きさを変換
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="posX"></param>
    /// <param name="scale"></param>
    private void ChangePosAndScaleX(GameObject obj, float posX, float scale)
    {
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, scale + 1);
        obj.transform.localPosition = new Vector3(posX, obj.transform.localPosition.y, obj.transform.localPosition.z);
    }
    /// <summary>
    /// 始点、終点座標Z、大きさを変換
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="posZ"></param>
    /// <param name="scale"></param>
    private void ChangePosAndScaleZ(GameObject obj, float posZ, float scale)
    {
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, scale + 1);
        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, posZ);
    }

    /// <summary>
    /// 選択した設計図データを削除
    /// </summary>
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