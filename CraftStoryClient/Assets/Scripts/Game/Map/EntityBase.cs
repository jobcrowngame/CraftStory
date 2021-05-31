using UnityEngine;

public class EntityBase : MonoBehaviour
{
    public int EntityID { get; set; }
    private EntityData data;

    public EntityData Data { get => data; }

    public void Init(EntityData data)
    {
        this.data = data;
    }
}