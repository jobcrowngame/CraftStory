using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 設計図データ
/// </summary>
[Serializable]
public class BlueprintData
{
    // サイズ
    public int sizeX { get; set; }
    public int sizeZ { get; set; }

    // ブロックリスト
    public List<BlueprintEntityData> blocks 
    { 
        get
        {
            if (mblocks == null)
                mblocks = new List<BlueprintEntityData>();
            
            return mblocks; 
        } 
        set => mblocks = value;
    }
    private List<BlueprintEntityData> mblocks;

    // 重複されたエリアがあるかのタグ
    [NonSerialized]
    private bool isDuplicate;
    public bool IsDuplicate { get => isDuplicate; set => isDuplicate = value; }

    /// <summary>
    /// 設計図の状態
    /// </summary>
    public BlueprintState State { get; set; }

    // 買った設計図はロックされる
    [NonSerialized]
    private bool isLocked;
    public bool IsLocked { get => isLocked; set => isLocked = value; }

    public BlueprintData() { }
    public BlueprintData(string json)
    {
        try
        {
            var obj = JsonMapper.ToObject<BlueprintData>(json);
            sizeX = obj.sizeX;
            sizeZ = obj.sizeZ;
            mblocks = obj.blocks;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
        finally
        {
            sizeX = 0;
            sizeZ = 0;
        }
    }

    /// <summary>
    /// 構築メソッド
    /// </summary>
    /// <param name="dataList">データリスト</param>
    /// <param name="size">設計図サイズ</param>
    public BlueprintData(List<BlueprintEntityData> dataList, Vector2Int size)
    {
        blocks = dataList;
        sizeX = size.x;
        sizeZ = size.y;
    }

    public string ToJosn()
    {
        return JsonMapper.ToJson(this);
    }

    /// <summary>
    /// 有効範囲チェック
    /// </summary>
    public bool CheckPos(Vector3Int buildPos)
    {
        foreach (var item in mblocks)
        {
            var newPos = CommonFunction.Vector3Sum(item.GetPos(), buildPos);
            if (MapCtl.IsOutRange(DataMng.E.MapData, newPos))
            {
                CommonFunction.ShowHintBar(8);
                return false;
            }
        }

        return true;
    }

    // 設計図ブロックデータ
    public class BlueprintEntityData
    {
        public int id { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }
        public int direction { get; set; }

        public Vector3Int GetPos()
        {
            return new Vector3Int(posX, posY, posZ);
        }
        public void SetPos(Vector3Int Pos)
        {
            posX = Pos.x;
            posY = Pos.y;
            posZ = Pos.z;
        }
    }

    public enum BlueprintState
    {
        None = 0,

        /// <summary>
        /// 他のブロックと被ってる
        /// </summary>
        IsDuplicate = 1,

        /// <summary>
        /// 高すぎる
        /// </summary>
        TooHigh = 2,

        /// <summary>
        /// マップ範囲を超えてる
        /// </summary>
        IsOutRange = 3,
    }
}