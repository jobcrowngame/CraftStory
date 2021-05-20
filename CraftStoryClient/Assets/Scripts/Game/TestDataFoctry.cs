using UnityEngine;

public class TestDataFoctry
{
    public static MapData CreateTestMap()
    {
        Vector3 mapSize = new Vector3(30, 30, 30);
        BlockData[,,] map = new BlockData[(int)mapSize.x, (int)mapSize.y, (int)mapSize.z];
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {


                for (int k = 0; k < mapSize.z; k++)
                {
                    if (j > 3)
                        continue;

                    map[i, j, k] = GetNewBlockData(new Vector3(i, j, k));
                }
            }
        }

        return new MapData(map, mapSize);
    }

    public static CharacterData CreateTestCharacter()
    {
        var cData = new CharacterData();
        cData.Pos = new Vector3(3, 12, 3);
        cData.Quaternion = Quaternion.identity;

        return cData;
    }

    public static BlockData GetNewBlockData(Vector3 pos)
    {
        int blockId = Random.Range(1001, 1005);
        var config = ConfigMng.E.BlockConfig[blockId];

        var bData = new BlockData(config.ID);
        bData.Pos = pos;

        return bData;
    }
}
