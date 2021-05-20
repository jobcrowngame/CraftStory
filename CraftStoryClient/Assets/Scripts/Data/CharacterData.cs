﻿using System;
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
}