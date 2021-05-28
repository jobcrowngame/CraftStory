using JsonConfigData;
using System;
using UnityEngine;

public class MapDataFactory
{
    private MapData mData;
    private Map mapConfig;

    public MapData CreateMap(int id)
    {
        var startTime = DateTime.Now;

        mapConfig = ConfigMng.E.Map[id];
        mData = new MapData(new Vector3Int(mapConfig.SizeX, mapConfig.SizeY, mapConfig.SizeZ));

        AddBaseBlocks();
        AddMountains();
        AddTrees();
        AddRocks();
        HideBlocks();

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Debug.LogWarningFormat("mapData 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);

        return mData;
    }
    private void AddBaseBlocks()
    {
        int groundHeight = mapConfig.Block01Height + mapConfig.Block02Height + mapConfig.Block03Height;

        for (int x = 0; x < mData.MapSize.x; x++)
        {
            for (int z = 0; z < mData.MapSize.z; z++)
            {
                for (int y = 0; y < groundHeight; y++)
                {
                    int blockId = GetBlockID(mapConfig, y);
                    mData.Map[x, y, z] = new BlockData(blockId, new Vector3Int(x, y, z));
                }
            }
        }
    }
    private void AddMountains()
    {
        var mountains = mapConfig.Mountains.Split(',');

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
                continue;
            }

            int startPosX = mountainConfig.StartPosX;
            int startPosY = mountainConfig.StartPosY;
            int startPosZ = mountainConfig.StartPosZ;

            if (startPosX < 0) startPosX = UnityEngine.Random.Range(0, mapConfig.SizeX);
            if (startPosZ < 0) startPosZ = UnityEngine.Random.Range(0, mapConfig.SizeZ);

            Debug.Log(new Vector3Int(startPosX, startPosY, startPosZ));

            AddMountains(mData.Map[startPosX, startPosY, startPosZ], mountainConfig.Height, mountainConfig.Wide);
        }
    }
    private void AddTrees()
    {
        var trees = mapConfig.Trees.Split(',');

        for (int i = 0; i < trees.Length; i++)
        {
            JsonConfigData.Tree treeConfig = null;

            try
            {
                treeConfig = ConfigMng.E.Tree[int.Parse(trees[i])];
                if (treeConfig == null)
                    continue;
            }
            catch (Exception ex)
            {
                Debug.LogError("not find tree " + trees[i]);
                Debug.LogError(ex.Message);
                continue;
            }

            for (int j = 0; j < treeConfig.Count; j++)
            {
                var pos = GetGroundPos(treeConfig.PosX, treeConfig.PosZ, treeConfig.OffsetY);
                mData.AddResources(new EntityData(treeConfig.ID, EntityType.Tree, pos));
            }
        }
    }
    private void AddRocks()
    {
        var rocks = mapConfig.Rocks.Split(',');

        for (int i = 0; i < rocks.Length; i++)
        {
            Rock rockConfig = null;

            try
            {
                rockConfig = ConfigMng.E.Rock[int.Parse(rocks[i])];
                if (rockConfig == null)
                    continue;
            }
            catch (Exception ex)
            {
                Debug.LogError("not find rock " + rocks[i]);
                Debug.LogError(ex.Message);
                continue;
            }

            for (int j = 0; j < rockConfig.Count; j++)
            {
                var pos = GetGroundPos(rockConfig.PosX, rockConfig.PosZ, rockConfig.OffsetY);
                mData.AddResources(new EntityData(rockConfig.ID, EntityType.Rock, pos));
            }
        }
    }
    private void HideBlocks()
    {
        for (int x = 0; x < mData.MapSize.x; x++)
        {
            for (int z = 0; z < mData.MapSize.z; z++)
            {
                for (int y = 0; y < mData.MapSize.y; y++)
                {
                    if (mData.Map[x, y, z] == null)
                        continue;

                    mData.Map[x, y, z].IsIn = !CheckBlockIsSurface(mData.Map[x, y, z]);
                }
            }
        }
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
    private BlockData GetGroundBlock(int posX, int posZ)
    {
        for (int i = mapConfig.SizeY - 1; i >= 0 ; i--)
        {
            if (mData.Map[posX,i,posZ] == null)
                continue;

            return mData.Map[posX, i, posZ];
        }
        return null;
    }
    private Vector3 GetGroundPos(int posX, int posZ, float offsetY)
    {
        if (posX < 0) posX = UnityEngine.Random.Range(0, mapConfig.SizeX);
        if (posZ < 0) posZ = UnityEngine.Random.Range(0, mapConfig.SizeZ);
        float posY = GetGroundBlock(posX, posZ).Pos.y + 1 + offsetY - 0.5f;

        return new Vector3(posX, posY, posZ);
    }

    private void AddMountains(BlockData parent, int height, int offset = 0)
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