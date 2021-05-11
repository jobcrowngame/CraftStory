using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject uiRoot;

    // Start is called before the first frame update
    void Start()
    {
        UICtl.E.Init(this.gameObject, uiRoot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
