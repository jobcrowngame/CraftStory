using System;
using System.Collections;
using System.Timers;

using UnityEngine;
using UnityEngine.Events;

public class TimeZoneMng : MonoBehaviour
{
    UnityEvent SecondTimerEvent01;

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
        StartCoroutine(SecondTimer01());
    }

    private IEnumerator SecondTimer01()
    {
        while (true)
        {
            SecondTimerEvent01.Invoke();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void AddSecondTimer01Event(UnityAction ac)
    {
        SecondTimerEvent01.AddListener(ac);
    }
}