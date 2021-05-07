using System;

public class Single<T> where T : class, new()
{
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
}