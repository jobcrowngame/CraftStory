using UnityEngine;

public class TestDataFoctry
{
    private static Vector3Int mapSize;

    public static MapData CreateTestMap()
    {
        mapSize = new Vector3Int(100, 100, 100);

        BlockData[,,] map = new BlockData[mapSize.x, mapSize.y, mapSize.z];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                //var randomY = 4;
                var randomY = Random.Range(20-1, 20+1);
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
                    //Debug.LogFormat("{0}, {1}:{2}:{3}", ++count, x, y, z);
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

        //if (mData.Pos.x == 0 || mData.Pos.y == 0 || mData.Pos.z == 0 || mData.Pos.x == mapSize.x - 1 || 
        //    mData.Pos.y == mapSize.y - 1 || mData.Pos.z == mapSize.z - 1)
        //{
        //    mData.IsIn = false;
        //    //Debug.LogFormat("{0}={1}", mData.Pos, mData.IsIn);
        //    return;
        //}

        int minX = mData.Pos.x - 1;
        if (minX < 0) minX = 0;
        int maxX = mData.Pos.x + 1;
        if (maxX > mapSize.x - 1) maxX = mapSize.x - 1;

        int minY = mData.Pos.y - 1;
        if (minY < 0) minY = 0;
        int maxY = mData.Pos.y + 1;
        if (maxY > mapSize.y - 1) maxY = mapSize.y - 1;

        int minZ = mData.Pos.z - 1;
        if (minZ < 0) minZ = 0;
        int maxZ = mData.Pos.z + 1;
        if (maxZ > mapSize.z - 1) maxZ = mapSize.z - 1;


        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    try
                    {
                        if (map[x, y, z] == null)
                        {
                            mData.IsIn = false;
                            //Debug.LogWarning("not in block " + mData.Pos);
                            //Debug.LogFormat("{0}={1}", mData.Pos, mData.IsIn);
                            return;
                        }
                    }
                    catch (System.Exception)
                    {
                        Debug.LogErrorFormat("{0},{1},{2}",x,y,z);
                    }
                }
            }
        }

        //Debug.LogFormat("{0}={1}", mData.Pos, mData.IsIn);
    }
}
