using System.Collections.Generic;
using UnityEngine;

public class MapEntitysPool : MonoBehaviour
{
    Dictionary<int, Stack<EntityBase>> objMap;

    public void Init()
    {
        objMap = new Dictionary<int, Stack<EntityBase>>();
    }

    public EntityBase Pop(int entityId)
    {
        // 存在しないと、新規追加
        if (!objMap.ContainsKey(entityId))
            return null;

        // プールにあるとプールからゲット
        if (objMap[entityId].Count == 0)
            return null;

        return objMap[entityId].Pop();
    }

    public void Push(int entityId, EntityBase entity)
    {
        try
        {
            if (entity != null)
            {
                entity.gameObject.SetActive(false);
                entity.transform.SetParent(transform);

                if (!objMap.ContainsKey(entityId))
                {
                    objMap[entityId] = new Stack<EntityBase>();
                }

                objMap[entityId].Push(entity);
            }
        }
        catch (System.Exception ex)
        {
            Logger.Error(ex);
        }
    }
}
