using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class AttObjC : MonoBehaviour
{
    void Start()
    {
#if UNITY_IOS
        int status = AttObjC.GetTrackingAuthorizationStatus();
        if (status == 0)
        {
            AttObjC.RequestTrackingAuthorization();
        }
        Debug.Log("ATT状況：" + status);
#endif
    }

#if UNITY_IOS
    private const string DLL_NAME = "__Internal";
    [DllImport(DLL_NAME)]
    private static extern int Sge_Att_getTrackingAuthorizationStatus();
    private static int GetTrackingAuthorizationStatus()
    {
        if (Application.isEditor)
        {
            return -1;
        }
        return Sge_Att_getTrackingAuthorizationStatus();
    }
    [DllImport(DLL_NAME)]
    private static extern void Sge_Att_requestTrackingAuthorization(OnCompleteCallback callback);

    private delegate void OnCompleteCallback(int status);
    private static SynchronizationContext _context;
    private static Action _onComplete;

    private static void RequestTrackingAuthorization()
    {
        if (Application.isEditor)
        {
            return;
        }
#if UNITY_IOS
        _context = SynchronizationContext.Current;
        Sge_Att_requestTrackingAuthorization(OnRequestComplete);
#endif
    }
    [AOT.MonoPInvokeCallback(typeof(OnCompleteCallback))]
    private static void OnRequestComplete(int status)
    {
        if (_onComplete != null)
        {
            _context.Post(_ => {
                _onComplete = null;
            }, null);
        }
    }
#endif
}