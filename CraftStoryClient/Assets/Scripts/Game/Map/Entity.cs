using UnityEngine;

public class Entity : MonoBehaviour
{
    public int EntityID { get; set; }
    private EntityData data;

    public void Init(EntityData data)
    {
        this.data = data;
    }
}