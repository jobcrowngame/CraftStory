using UnityEngine;

public class EntityData
{
    private int id;
    private EntityType type;
    private Vector3 pos;

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
}

public enum EntityType
{
    Tree,
    Rock,
}
