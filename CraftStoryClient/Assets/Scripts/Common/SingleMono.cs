using System;
using UnityEngine;

/// <summary>
/// 唯一のエンティティ親
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleMono<T> : MonoBehaviour where T : Component
{
    [NonSerialized]
    private static T entity;

    public static T E
    {
        get
        {
            if (entity == null)
            {
                var obj = new GameObject();
                obj.transform.parent = Main.E.transform;
                entity = obj.AddComponent<T>();
                obj.name = typeof(T).ToString();
            }

            return entity;
        }
    }

    public virtual void Init() { }
}
