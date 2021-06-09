using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPTest : MonoBehaviour
{
    MyIAPManager iapMng;
    public Text text;
    public Button InitBtn;

    private void Awake()
    {
        InitBtn.onClick.AddListener(() => 
        {
            iapMng = new MyIAPManager();
            iapMng.Init(this);
        });
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    ///

    public void GrantCredits(string credits)
    {
        //userCredits = userCredits + credits;
        Debug.Log("You received " + credits + " Credits!");
        iapMng.OnPurchaseClicked(credits);
    }

    public void ShowMsg(string msg)
    {
        text.text += msg + "\n";
    }
}
