using System;
using System.Collections;
using UnityEngine;

public class GameTimeCtl
{
    Light DirectionalLight { get => GameObject.Find("Directional Light").GetComponent<Light>(); }

    public bool Active { get; set; }

    private float CurTime
    {
        get => curTime;
        set
        {
            curTime = value;

            if (curTime > SettingMng.E.GameDaySeconds)
            {
                curTime = 1;
            }

            RefreshLight();
        }
    }
    private float curTime = 0;

    public GameTimeCtl()
    {
        CurTime = 300;
    }

    public void Update(float time)
    {
        CurTime += time;
    }
    private void RefreshLight()
    {
        //if (Active)
        {
            float percent = curTime / SettingMng.E.GameDaySeconds;
            DirectionalLight.transform.rotation = Quaternion.Euler(360 * percent, 0, 0);
        }
    }
}