using System;
using UnityEngine;

public class TestDataFoctry
{
    private static Vector3Int mapSize;
    private static MapBlockData[,,] map;

    public static MapData CreateTestMap()
    {
        var startTime = DateTime.Now;

        mapSize = new Vector3Int(50, 50, 50);
        map = new MapBlockData[mapSize.x, mapSize.y, mapSize.z];
        //for (int x = 0; x < mapSize.x; x++)
        //{
        //    for (int z = 0; z < mapSize.z; z++)
        //    {
        //        var randomY = UnityEngine.Random.Range(20, 20 + 2);
        //        for (int y = 0; y < randomY; y++)
        //        {
        //            map[x, y, z] = GetNewBlockData(new Vector3Int(x, y, z));
        //        }
        //    }
        //}

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                for (int y = 0; y < 4; y++)
                {
                    int blockId = 1001;

                    if (y < 1) blockId = 1001;
                    if (y >= 2 && y < 10) blockId = 1002;
                    if (y >= 3 && y < 15) blockId = 1003;
                    if (y >= 4 && y < 20) blockId = 1004;

                    map[x, y, z] = new MapBlockData(blockId, new Vector3Int(x, y, z));
                }
            }
        }

        AddMountain(map[10, 3, 10], 4, 2);
        AddMountain(map[7, 2, 7], 3, 3);
        AddMountain(map[19, 2, 6], 4, 6);
        AddMountain(map[16, 1, 13], 7, 7);
        AddMountain(map[26, 1, 15], 6, 10);

        //for (int x = 0; x < mapSize.x; x++)
        //{
        //    for (int z = 0; z < mapSize.z; z++)
        //    {
        //        for (int y = 0; y < mapSize.y; y++)
        //        {
        //            if (map[x, y, z] == null)
        //                continue;

        //            map[x, y, z].IsIn = !CheckBlockIsSurface(map[x, y, z]);

        //            if (map[x, y, z].IsIn == false)
        //                Debug.Log(map[x, y, z].ToString());
        //        }
        //    }
        //}

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Debug.LogWarningFormat("mapData 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);

        return new MapData(map, mapSize);
    }

    public static CharacterData CreateTestCharacter()
    {
        var cData = new CharacterData();
        cData.Pos = new Vector3(1, 25, 1);
        cData.EulerAngles = Vector3.zero;

        return cData;
    }

    private static MapBlockData GetNewBlockData(Vector3Int pos)
    {
        int blockId = UnityEngine.Random.Range(1001, 1005);
        return new MapBlockData(blockId, pos);
    }

    private static bool CheckBlockIsSurface(MapBlockData data)
    {
        var isSurface = IsSurface(new Vector3Int(data.Pos.x - 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x + 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y - 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y + 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z - 1));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z + 1));
        return isSurface;
    }
    private static bool IsSurface(Vector3Int pos)
    {
        if (IsOutRange(pos))
            return false;

        return GetNextToBlock(pos) == null;
    }
    private static MapBlockData GetNextToBlock(Vector3Int pos)
    {
        if (IsOutRange(pos))
            return null;

        return map[pos.x, pos.y, pos.z];
    }
    private static bool IsOutRange(Vector3Int pos)
    {
        return pos.x < 0 || pos.x > mapSize.x - 1
            || pos.y < 0 || pos.y > mapSize.y - 1
            || pos.z < 0 || pos.z > mapSize.z - 1;
    }

    private static void AddMountain(MapBlockData parent, int height, int offset = 0)
    {
        for (int x = parent.Pos.x - (height - 1 + offset); x <= parent.Pos.x + (height - 1 + offset); x++)
        {
            for (int z = parent.Pos.z - (height - 1 + offset); z <= parent.Pos.z + (height - 1 + offset); z++)
            {
                int abX = Mathf.Abs(x - parent.Pos.x);
                int abZ = Mathf.Abs(z - parent.Pos.z);
                int aby = height - abX - abZ + offset;

                if (aby <= 0)
                    continue;

                int random = UnityEngine.Random.Range(-1, 1);

                for (int i = 1; i <= aby; i++)
                {
                    int offsetY = i;
                    if (offsetY > height)
                        offsetY = height;

                    Vector3Int newPos = new Vector3Int(x, offsetY + parent.Pos.y + random, z);
                    if (IsOutRange(newPos))
                        continue;

                    map[x, newPos.y, z] = map[x, parent.Pos.y, z].Copy();
                    map[x, newPos.y, z].Pos = newPos;
                }
            }
        }
    }
}
