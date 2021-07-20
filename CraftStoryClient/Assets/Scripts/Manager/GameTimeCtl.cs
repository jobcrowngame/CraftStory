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
                RefreshSkyBox();
            }
        }
    }
    private float curTime;

    public GameTimeCtl()
    {
        Active = false;
        CurTime = SettingMng.E.GameDaySeconds * 0.1f;
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

    private void RefreshSkyBox()
    {
        float percent = curTime / SettingMng.E.GameDaySeconds;
        float newV = (percent < 0.5f) ? 1 - Mathf.Abs(percent - 0.25f) * 4f : 0;
        RenderSettings.skybox.SetFloat("_Exposure", newV);
    }
}