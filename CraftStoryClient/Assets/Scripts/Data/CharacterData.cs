using System;
using UnityEngine;

[Serializable]
public class CharacterData
{
    private float posX { get; set; }
    private float posY { get; set; }
    private float posZ { get; set; }

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
}