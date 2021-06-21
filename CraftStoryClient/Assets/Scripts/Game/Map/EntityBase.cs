using UnityEngine;

public class EntityBase : MonoBehaviour
{
    private EntityData data;

    public EntityData Data { get => data; }

    public void Init(EntityData data)
    {
        this.data = data;
    }

    public ItemType Type { get => data.Type; }
}