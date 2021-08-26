using UnityEngine;

/// <summary>
/// ゲーム時間コンソール
/// </summary>
public class GameTimeCtl
{
    Light DirectionalLight { get => GameObject.Find("Directional Light").GetComponent<Light>(); }

    /// <summary>
    /// ゲーム時間変化をアクティブ
    /// </summary>
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

    /// <summary>
    /// 今の時間
    /// </summary>
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

    /// <summary>
    /// 角度によってLightを更新
    /// </summary>
    /// <param name="angle">太陽角度</param>
    private void RefreshLight(float angle)
    {
        DirectionalLight.transform.rotation = Quaternion.Euler(angle, 30, 0);
        var newV = GetPercentByAngle(angle);
        DirectionalLight.intensity = newV;
    }

    /// <summary>
    /// SkyBoxの明るさを角度によって更新
    /// </summary>
    /// <param name="angle"></param>
    private void RefreshSkyBox(float angle)
    {
        var newV = GetPercentByAngle(angle);

        RenderSettings.skybox.SetFloat("_Exposure", newV);
        float skyboxAmbientIntensity = newV;
        if (skyboxAmbientIntensity < SettingMng.E.MinAmbientIntensity)
            skyboxAmbientIntensity = SettingMng.E.MinAmbientIntensity;

        RenderSettings.ambientIntensity = skyboxAmbientIntensity;
    }

    /// <summary>
    /// 角度によって今の明るさパーセントをゲット
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float GetPercentByAngle(float angle)
    {
        float newV = 0;
        if (angle < 180)
        {
            newV = 1;
        }
        else if (angle >= 180 && angle < 210)
        {
            newV = 1 - ((angle - 180) / 30);
        }
        else if (angle >= 330 && angle < 360)
        {
            newV = 1 - ((360 - angle) / 30);
        }
        else
        {
            newV = 0;
        }

        return newV;
    }
}