
using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlast : EntityBase
{
    Blast config;
    float curTime;

    public void Set(int entityId)
    {
        foreach (var item in ConfigMng.E.Blast.Values)
        {
            if (item.EntityID == entityId)
            {
                config = item;
                StartTimer();
                break;
            }
        }
    }

    private void StartTimer()
    {
        TimeZoneMng.E.AddTimerEvent01(Update02S);
    }
    private void EndTimer()
    {
        TimeZoneMng.E.RemoveTimerEvent01(Update02S);
        Burst();
    }

    private void Update02S()
    {
        curTime += 0.2f;

        if (curTime >= config.Timer)
        {
            EndTimer();
        }
    }

    private void Burst()
    {
        // add effect
        var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, config.Effect);
        effect.Init(3);

        DestroyEntitys(config.Radius);

        Destroy(gameObject);
    }

    private void DestroyEntitys(int radius)
    {
        Dictionary<int, int> addItems = new Dictionary<int, int>();

        var startPos = Vector3Int.CeilToInt(transform.position);
        for (int y = -radius; y <= radius; y++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int indexX = startPos.x + x;
                    int indexY = startPos.y + y;
                    int indexZ = startPos.z + z;

                    // マップ以外のPos場合、スキップ
                    if (indexX < 0 || indexX >= DataMng.E.MapData.SizeX ||
                        indexY < 1 || indexY >= DataMng.E.MapData.SizeY ||
                        indexZ < 0 || indexZ >= DataMng.E.MapData.SizeZ ||
                        DataMng.E.MapData.Map[indexX, indexY, indexZ].entityID == 10000)
                        continue;

                    var pos = new Vector3Int(indexX, indexY, indexZ);

                    // 手にいるアイテムを記録
                    if (DataMng.E.MapData.Map[indexX, indexY, indexZ].entityID > 0)
                    {
                        var config = ConfigMng.E.Entity[DataMng.E.MapData.Map[indexX, indexY, indexZ].entityID];
                        if (config.ItemID > 0)
                        {
                            if (addItems.ContainsKey(config.ItemID))
                            {
                                addItems[config.ItemID] += 1;
                            }
                            else
                            {
                                addItems[config.ItemID] = 1;
                            }
                        }
                    }

                    // マップデータ削除
                    DataMng.E.MapData.Map[indexX, indexY, indexZ] = new MapData.MapCellData() { entityID = 0, direction = 0 };

                    // entityを削除
                    WorldMng.E.MapCtl.DeleteEntity(pos);

                    // 爆破周りのブロックをチェック
                    if (x == -radius || x == radius ||
                       y == -radius || y == radius ||
                       z == -radius || z == radius ||
                       y == 0)
                        WorldMng.E.MapCtl.CheckNextToEntitys(pos);
                }
            }
        }

        WorldMng.E.MapCtl.CombineMesh();

        NWMng.E.AddItems((rp) => 
        {
            NWMng.E.GetItems();
        }, addItems);
    }
}