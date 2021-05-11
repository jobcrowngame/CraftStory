using System;
using UnityEngine;

public class UILogicBase<T, K> where T : class, new()
{
    private static T entity;
    protected K ui;

    public static T E
    {
        get
        {
            if (entity == null)
                entity = new T();

            return entity;
        }
    }

    public virtual void Init(K ui)
    {
        this.ui = ui;
    }
}