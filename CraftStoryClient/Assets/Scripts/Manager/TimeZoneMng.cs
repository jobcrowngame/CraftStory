using System;
using System.Collections;
using System.Timers;

using UnityEngine;
using UnityEngine.Events;

public class TimeZoneMng : MonoBehaviour
{
    UnityEvent SecondTimerEvent01;
    UnityEvent SecondTimerEvent02;

    public static TimeZoneMng E
    {
        get
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<TimeZoneMng>();

            return entity;
        }
    }
    private static TimeZoneMng entity;

    public void Init()
    {
        SecondTimerEvent01 = new UnityEvent();
        SecondTimerEvent02 = new UnityEvent();

        StartCoroutine(SecondTimer01());
        StartCoroutine(SecondTimer02());
    }

    private IEnumerator SecondTimer01()
    {
        while (true)
        {
            SecondTimerEvent01.Invoke();
            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator SecondTimer02()
    {
        while (true)
        {
            SecondTimerEvent02.Invoke();
            yield return new WaitForSeconds(1f);
        }
    }

    public void AddSecondTimerEvent01(UnityAction ac)
    {
        SecondTimerEvent01.AddListener(ac);
    }
    public void AddSecondTimerEvent02(UnityAction ac)
    {
        SecondTimerEvent02.AddListener(ac);
    }
}