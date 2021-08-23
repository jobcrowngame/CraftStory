using System;

/// <summary>
/// 唯一のエンティティ親
/// </summary>
/// <typeparam name="T"></typeparam>
public class Single<T> where T : class, new()
{
    [NonSerialized]
    private static T entity;

    public static T E 
    {
        get 
        {
            if (entity == null)
                entity = new T();

            return entity; 
        }
    }

    public virtual void Init()
    {

    }
}