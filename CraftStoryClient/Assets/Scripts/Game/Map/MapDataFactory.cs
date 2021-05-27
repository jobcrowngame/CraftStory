using JsonConfigData;
using System;
using UnityEngine;

public class MapDataFactory
{
    private MapData mData;

    public MapData CreateMap(int id)
    {
        var startTime = DateTime.Now;

        var config = ConfigMng.E.Map[id];
        var mountains = config.Mountains.Split(',');
        var mapSize = new Vector3Int(config.SizeX, config.SizeY, config.SizeZ);
        mData = new MapData(mapSize);

        int groundHeight = config.Block01Height + config.Block02Height + config.Block03Height;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                    int offset = UnityEngine.Random.Range(0, 2);
                for (int y = 0; y < groundHeight; y++)
                {
                    int blockId = GetBlockID(config, y);
                    mData.Map[x, y, z] = new BlockData(blockId, new Vector3Int(x, y + offset, z));
                }
            }
        }

        for (int i = 0; i < mountains.Length; i++)
        {
            Mountain mountainConfig = null;

            try
            {
                mountainConfig = ConfigMng.E.Mountain[int.Parse(mountains[i])];
                if (mountainConfig == null)
                    continue;
            }
            catch (Exception ex)
            {
                Debug.LogError("not find Mountain " + mountains[i]);
                Debug.LogError(ex.Message);
            }

            int startPosX = mountainConfig.StartPosX - 1;
            int startPosY = mountainConfig.StartPosY - 1;
            int startPosZ = mountainConfig.StartPosZ - 1;

            if (startPosX < 0) startPosX = UnityEngine.Random.Range(0, config.SizeX);
            if (startPosZ < 0) startPosZ = UnityEngine.Random.Range(0, config.SizeZ);

            AddMountain(mData.Map[startPosX, startPosY, startPosZ], mountainConfig.Height, mountainConfig.Wide);
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    if (mData.Map[x, y, z] == null)
                        continue;

                    mData.Map[x, y, z].IsIn = !CheckBlockIsSurface(mData.Map[x, y, z]);
                }
            }
        }

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Debug.LogWarningFormat("mapData 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);

        return mData;
    }

    private bool CheckBlockIsSurface(BlockData data)
    {
        var isSurface = IsSurface(new Vector3Int(data.Pos.x - 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x + 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y - 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y + 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z - 1));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z + 1));
        return isSurface;
    }
    private bool IsSurface(Vector3Int pos)
    {
        if (IsOutRange(pos))
            return false;

        return GetNextToBlock(pos) == null;
    }
    private BlockData GetNextToBlock(Vector3Int pos)
    {
        if (IsOutRange(pos))
            return null;

        return mData.Map[pos.x, pos.y, pos.z];
    }
    private bool IsOutRange(Vector3Int pos)
    {
        return pos.x < 0 || pos.x > mData.MapSize.x - 1
            || pos.y < 0 || pos.y > mData.MapSize.y - 1
            || pos.z < 0 || pos.z > mData.MapSize.z - 1;
    }

    private int GetBlockID(Map config, int y)
    {
        if (y < config.Block01Height)
            return config.Block01;
        else if (y < config.Block02Height + config.Block01Height)
            return config.Block02;
        else
            return config.Block03;
    }

    private void AddMountain(BlockData parent, int height, int offset = 0)
    {
        if (parent == null)
        {
            Debug.LogError("bad parent ");
            return;
        }

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

                    mData.Map[x, newPos.y, z] = mData.Map[x, parent.Pos.y, z].Copy();
                    mData.Map[x, newPos.y, z].Pos = newPos;
                }
            }
        }
    }
}