using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private float posX { get; set; }
    private float posY { get; set; }
    private float posZ { get; set; }

    private float qX { get; set; }
    private float qY { get; set; }
    private float qZ { get; set; }

    public int[] EquipedItems 
    {
        get
        {
            if (equipedItems == null)
                equipedItems = new int[6];
            
            return equipedItems; 
        }
    }
    private int[] equipedItems;

    public Vector3 Pos
    {
        get { return new Vector3(posX, posY, posZ); }
        set
        {
            posX = value.x;
            posY = value.y;
            posZ = value.z;
        }
    }

    public Vector3 EulerAngles
    {
        get { return new Vector3(qX, qY, qZ); }
        set
        {
            qX = value.x;
            qY = value.y;
            qZ = value.z;
        }
    }

    public void EquipItem(int index, int itemGuid)
    {
        for (int i = 0; i < equipedItems.Length; i++)
        {
            if (equipedItems[i] == itemGuid)
                equipedItems[i] = 0;
        }

        equipedItems[index] = itemGuid;
    }
}