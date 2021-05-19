using UnityEngine;

public class TestDataFoctry
{
    public static MapData CreateTestMap()
    {
        Vector3 mapSize = new Vector3(30, 30, 30);
        MapCellData[,,] map = new MapCellData[(int)mapSize.x, (int)mapSize.y, (int)mapSize.z];
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {


                for (int k = 0; k < mapSize.z; k++)
                {
                    if (j > 3)
                        continue;

                    map[i, j, k] = GetNewMapCellData(new Vector3(i, j, k));
                }
            }
        }

        return new MapData(map, mapSize);
    }

    public static CharacterData CreateTestCharacter()
    {
        var cData = new CharacterData();
        cData.Pos = new Vector3(3, 12, 3);

        return cData;
    }

    public static MapCellData GetNewMapCellData(Vector3 pos)
    {
        MapCellType mcType = (MapCellType)Random.Range(0, 4);
        return GetNewMapCellData(pos, mcType);
    }
    public static MapCellData GetNewMapCellData(Vector3 pos, MapCellType mcType)
    {
        return new MapCellData()
        {
            Pos = pos,
            CellType = mcType,
            DeleteTime = 1f
        };
    }
}
