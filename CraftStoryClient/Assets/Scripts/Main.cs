using System.Collections;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private GameObject uiRoot;

    // Start is called before the first frame update
    void Start()
    {
        DataMng.E.Load();

        WorldMng.E = gameObject.GetComponent<WorldMng>();

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return GS2.E.InitCoroutine();
        yield return UICtl.E.Init(gameObject, uiRoot);
    }

    private void OnApplicationQuit()
    {
        if (WorldMng.E != null) WorldMng.E.OnQuit();
        if (DataMng.E != null) DataMng.E.Save();
    }
}
