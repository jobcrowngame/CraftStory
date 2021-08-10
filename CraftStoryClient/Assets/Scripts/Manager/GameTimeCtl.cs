using System;
using System.Collections;
using UnityEngine;

public class GameTimeCtl
{
    Light DirectionalLight { get => GameObject.Find("Directional Light").GetComponent<Light>(); }

    public bool Active 
    {
        get => mActive;
        set
        {
            mActive = value;

            if (!mActive)
            {
                RenderSettings.skybox.SetFloat("_Exposure", 1);
                RenderSettings.ambientIntensity = 1;
            }
        }
    }
    private bool mActive;

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
                float percent = curTime / SettingMng.E.GameDaySeconds;
                float angle = 360 * percent;

                RefreshLight(angle);
                RefreshSkyBox(angle);
            }
            else
            {
                RefreshLight(120);
                RefreshSkyBox(120);
            }
        }
    }
    private float curTime;

    public GameTimeCtl()
    {
        CurTime = SettingMng.E.GameDaySeconds * 0.2f;
        TimeZoneMng.E.AddSecondTimerEvent02(() => { CurTime += 0.02f; });
        Active = false;
    }

    private void RefreshLight(float angle)
    {
        DirectionalLight.transform.rotation = Quaternion.Euler(angle, 30, 0);
    }

    private void RefreshSkyBox(float angle)
    {
        float newV = 0;
        if (angle < 180)
        {
            newV = 1;
        }
        else if (angle >= 180 && angle < 210)
        {
            newV = 1 - ((angle - 180) / 30);
        }else if (angle >= 330 && angle < 360)
        {
            newV = 1 - ((360 - angle) / 30);
        }
        else
        {
            newV = 0;
        }

        RenderSettings.skybox.SetFloat("_Exposure", newV);
        float skyboxAmbientIntensity = newV;
        if (skyboxAmbientIntensity < SettingMng.E.MinAmbientIntensity)
            skyboxAmbientIntensity = SettingMng.E.MinAmbientIntensity;

        RenderSettings.ambientIntensity = skyboxAmbientIntensity;
    }
}