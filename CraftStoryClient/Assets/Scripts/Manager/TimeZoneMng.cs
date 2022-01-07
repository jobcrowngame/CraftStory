using System.Collections;

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 時計機能
/// </summary>
public class TimeZoneMng : MonoBehaviour
{
    // 毎0.2秒
    UnityEvent SecondTimerEvent01;

    // 毎0.02秒
    UnityEvent SecondTimerEvent02;

    // 毎1秒
    UnityEvent SecondTimerEvent03;

    public static TimeZoneMng E
    {
        get
        {
            if (entity == null)
                entity = CommonFunction.CreateGlobalObject<TimeZoneMng>();

            return entity;
        }
    }
    private static TimeZoneMng entity;

    public void Init()
    {
        Stopping = false;
        SecondTimerEvent01 = new UnityEvent();
        SecondTimerEvent02 = new UnityEvent();
        SecondTimerEvent03 = new UnityEvent();
    }

    private bool Stopping;
    private int Timer01Counter = 0;
    private int Timer03Counter = 0;
    private readonly int Timer01Ratio = 10;
    private readonly int Timer03Ratio = 50;

    public void FixedUpdate()
    {
        if (Stopping) return;

        Timer01Counter++;
        if (Timer01Counter >= Timer01Ratio)
        {
            Timer01Counter %= Timer01Ratio;
            SecondTimerEvent01.Invoke();
        }

        //SecondTimerEvent02.Invoke();

        Timer03Counter++;
        if (Timer03Counter >= Timer03Ratio)
        {
            Timer03Counter %= Timer03Ratio;
            SecondTimerEvent03.Invoke();
        }
    }

    /// <summary>
    /// 毎0.2秒
    /// </summary>
    /// <param name="ac"></param>
    public void AddTimerEvent01(UnityAction ac)
    {
        SecondTimerEvent01.AddListener(ac);
    }
    public void RemoveTimerEvent01(UnityAction ac)
    {
        SecondTimerEvent01.RemoveListener(ac);
    }

    /// <summary>
    /// 毎0.02秒
    /// </summary>
    public void AddTimerEvent02(UnityAction ac)
    {
        SecondTimerEvent02.AddListener(ac);
    }
    public void RemoveTimerEvent02(UnityAction ac)
    {
        SecondTimerEvent02.RemoveListener(ac);
    }

    /// <summary>
    /// 毎秒
    /// </summary>
    public void AddTimerEvent03(UnityAction ac)
    {
        SecondTimerEvent03.AddListener(ac);
    }
    public void RemoveTimerEvent03(UnityAction ac)
    {
        SecondTimerEvent03.RemoveListener(ac);
    }

    public void Stop()
    {
        Stopping = true;
    }

    public void Resume()
    {
        Stopping = false;
    }
}