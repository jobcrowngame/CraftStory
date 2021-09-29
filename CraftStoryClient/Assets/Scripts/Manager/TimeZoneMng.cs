using System.Collections;

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 時計機能
/// </summary>
public class TimeZoneMng : MonoBehaviour
{
    UnityEvent SecondTimerEvent01;
    UnityEvent SecondTimerEvent02;
    UnityEvent SecondTimerEvent03;

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
        SecondTimerEvent03 = new UnityEvent();

        StartCoroutine(SecondTimer01());
        StartCoroutine(SecondTimer02());
        StartCoroutine(SecondTimer03());
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
            yield return new WaitForSeconds(0.02f);
        }
    }
    private IEnumerator SecondTimer03()
    {
        while (true)
        {
            SecondTimerEvent03.Invoke();
            yield return new WaitForSeconds(1f);
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

    /// <summary>
    /// 毎0.02秒
    /// </summary>
    public void AddTimerEvent02(UnityAction ac)
    {
        SecondTimerEvent02.AddListener(ac);
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
}