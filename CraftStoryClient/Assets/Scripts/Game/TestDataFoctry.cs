using UnityEngine;

public class TestDataFoctry
{
    private static Vector3Int mapSize;

    public static MapData CreateTestMap()
    {
        mapSize = new Vector3Int(300, 100, 300);

        BlockData[,,] map = new BlockData[mapSize.x, mapSize.y, mapSize.z];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                var randomY = Random.Range(20, 20 + 2);
                for (int y = 0; y < randomY; y++)
                {
                    map[x, y, z] = GetNewBlockData(new Vector3Int(x, y, z));
                }
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    SetDefaltInfo(map, map[x, y, z]);
                }
            }
        }

        return new MapData(map, mapSize);
    }

    public static CharacterData CreateTestCharacter()
    {
        var cData = new CharacterData();
        cData.Pos = new Vector3(1, 25, 1);
        cData.EulerAngles = Vector3.zero;

        return cData;
    }

    public static BlockData GetNewBlockData(Vector3Int pos)
    {
        int blockId = Random.Range(1001, 1005);
        var config = ConfigMng.E.BlockConfig[blockId];

        var bData = new BlockData(config.ID);
        bData.Pos = pos;
        bData.IsIn = true;

        return bData;
    }

    private static void SetDefaltInfo(BlockData[,,] map, BlockData mData)
    {
        if (mData == null)
            return;

        bool ret = false;
       
        if (!ret) ret = CheckPosIsNull(map, new Vector3Int(mData.Pos.x - 1, mData.Pos.y, mData.Pos.z));
        if (!ret) ret = CheckPosIsNull(map, new Vector3Int(mData.Pos.x + 1, mData.Pos.y, mData.Pos.z));
        if (!ret) ret = CheckPosIsNull(map, new Vector3Int(mData.Pos.x, mData.Pos.y - 1, mData.Pos.z));
        if (!ret) ret = CheckPosIsNull(map, new Vector3Int(mData.Pos.x, mData.Pos.y + 1, mData.Pos.z));
        if (!ret) ret = CheckPosIsNull(map, new Vector3Int(mData.Pos.x, mData.Pos.y, mData.Pos.z - 1));
        if (!ret) ret = CheckPosIsNull(map, new Vector3Int(mData.Pos.x, mData.Pos.y, mData.Pos.z + 1));

        mData.IsIn = !ret;
    }

    private static bool CheckPosIsNull(BlockData[,,] map, Vector3Int pos)
    {
        try
        {
            if (pos.x < 0) pos.x = 0;
            if (pos.x > mapSize.x - 1) pos.x = mapSize.x - 1;
            if (pos.y < 0) pos.y = 0;
            if (pos.y > mapSize.y - 1) pos.y = mapSize.y - 1;
            if (pos.z < 0) pos.z = 0;
            if (pos.z > mapSize.z - 1) pos.z = mapSize.z - 1;

            return map[pos.x, pos.y, pos.z] == null;
        }
        catch
        {
            Debug.LogError(map + ", " + pos);
            return false;
        }
    }
}
