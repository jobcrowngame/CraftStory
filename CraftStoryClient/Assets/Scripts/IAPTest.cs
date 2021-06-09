using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPTest : MonoBehaviour
{
    MyIAPManager iapMng;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        iapMng = new MyIAPManager();
        iapMng.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GrantCredits(string credits)
    {
        //userCredits = userCredits + credits;
        Debug.Log("You received " + credits + " Credits!");
        iapMng.OnPurchaseClicked(credits);
    }

    public void OnInitializeFailed()
    {
        text.text = "OnInitializeFailed";
    }
    public void ProcessPurchase()
    {
        text.text = "ProcessPurchase";
    }
    public void OnPurchaseFailed()
    {
        text.text = "OnPurchaseFailed";
    }
}
