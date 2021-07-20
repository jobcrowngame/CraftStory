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
                curTime = 0.01f;

            if (Active)
            {
                RefreshLight();
            }
        }
    }
    private float curTime;

    public GameTimeCtl()
    {
        Active = false;
        CurTime = 0;
    }

    public void Update(float time)
    {
        CurTime += time;
    }
    private void RefreshLight()
    {
        float percent = curTime / SettingMng.E.GameDaySeconds;
        DirectionalLight.transform.rotation = Quaternion.Euler(360 * percent, 0, 0);
    }
}