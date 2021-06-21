using JsonConfigData;
using UnityEngine;

public class EntityData
{
    private int id;
    private EntityType type;
    private Vector3 pos;

    public EntityData(string data)
    {
        string[] datas = data.Split('^');
        if (string.IsNullOrEmpty(datas[0]))
            return;

        id = int.Parse(datas[0]);
        type = (EntityType)int.Parse(datas[1]);
        pos = new Vector3(float.Parse(datas[2]), float.Parse(datas[3]), float.Parse(datas[4]));
    }
    public EntityData(int id, EntityType type)
    {
        this.id = id;
        this.type = type;
    }
    public EntityData(int id, EntityType type, Vector3 pos)
    {
        this.id = id;
        this.type = type;
        this.pos = pos;
    }

    public int ID { get => id; }
    public EntityType Type{ get => type; }
    public Vector3 Pos
    {
        get => pos;
        set => pos = value;
    }
    public Resource Config { get => ConfigMng.E.Resource[id]; }

    public string ResourcePath
    {
        get
        {
            switch (type)
            {
                case EntityType.Resources: return ConfigMng.E.Resource[id].ResourcePath;
                case EntityType.TransferGate: return ConfigMng.E.TransferGate[id].ResourcesPath;
                case EntityType.Craft: return ConfigMng.E.Building[id].ResourceName;
                default: Debug.LogError("not find config " + type); break;
            }

            return "";
        }
    }

    public string ToStringData()
    {
        return string.Format("{0}^{1}^{2}^{3}^{4}", id, (int)type, pos.x, pos.y, pos.z);
    }
}

public enum EntityType
{
    Resources,
    TransferGate,
    Craft,
}
