using System;
using UnityEngine;

[Serializable]
public class CharacterData
{
    private float posX { get; set; }
    private float posY { get; set; }
    private float posZ { get; set; }

    private float qX { get; set; }
    private float qY { get; set; }
    private float qZ { get; set; }
    private float qW { get; set; }

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

    public Quaternion Quaternion
    {
        get { return new Quaternion(qX, qY, qZ, qW); }
        set
        {
            qX = value.x;
            qY = value.y;
            qZ = value.z;
            qW = value.w;
        }
    }
}