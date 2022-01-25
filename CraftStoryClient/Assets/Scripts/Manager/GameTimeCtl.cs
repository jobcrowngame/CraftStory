using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム時間コンソール
/// </summary>
public class GameTimeCtl
{
    Light DirectionalLight { get => GameObject.Find("Directional Light").GetComponent<Light>(); }
    Image ClockHand 
    {
        get 
        {
            GameObject obj = GameObject.Find("ClockHand");
            return obj != null ? obj.GetComponent<Image>() : null; 
        } 
    }

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

            if (curTime > SettingMng.GameDaySeconds)
                curTime = 0.01f;

            if (Active)
            {
                float percent = curTime / SettingMng.GameDaySeconds;
                float angle = 360 * percent;

                RefreshClock(angle);
                RefreshLight(angle);
                RefreshSkyBox(angle);
                CheckIsNight(angle);
            }
            else
            {
                RefreshLight(35);
                RefreshSkyBox(35);

                mIsNight = false;
            }
        }
    }
    private float curTime;

    /// <summary>
    /// 今、夜かの判定
    /// </summary>
    public bool IsNight { get => mIsNight; }
    private bool mIsNight;

    public GameTimeCtl()
    {
        ResetTime(); 
        Active = false;
    }

    float timer = 1;
    public void FixedUpdate()
    {
        timer += 0.02f;

        if (timer > 1)
        {
            timer -= 1;
            CurTime += 1;
        }
    }

    /// <summary>
    /// 時間を朝にリセット
    /// </summary>
    public void ResetTime()
    {
        CurTime = SettingMng.GameDaySeconds * 0.0f;
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
        if (skyboxAmbientIntensity < SettingMng.MinAmbientIntensity)
            skyboxAmbientIntensity = SettingMng.MinAmbientIntensity;

        RenderSettings.ambientIntensity = skyboxAmbientIntensity;
    }

    /// <summary>
    /// ClockHandの角度を変更
    /// </summary>
    /// <param name="angle"></param>
    private void RefreshClock(float angle)
    {
        if (ClockHand != null)
        {
            ClockHand.transform.rotation = Quaternion.Euler(0, 0, 120 - angle);
        }
    }

    /// <summary>
    /// 夜のチェック
    /// </summary>
    /// <param name="angle"></param>
    private void CheckIsNight(float angle)
    {
        mIsNight = angle > 240;
    }

    /// <summary>
    /// 角度によって今の明るさパーセントをゲット
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float GetPercentByAngle(float angle)
    {
        canSleep = false;
        float newV = 0;
        if (angle < 220)
        {
            newV = 1;
        }
        else if (angle >= 220 && angle < 240)
        {
            newV = 1 - ((angle - 220) / 20);
            canSleep = true;
        }
        else if (angle >= 340 && angle < 360)
        {
            newV = 1 - ((360 - angle) / 20);
        }
        else
        {
            newV = 0;
            canSleep = true;
        }

        return newV;
    }

    public bool canSleep;
    public bool CanSleep { get => canSleep; }
}