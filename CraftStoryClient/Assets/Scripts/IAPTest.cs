using UnityEngine;
using UnityEngine.UI;

public class IAPTest : MonoBehaviour
{
    IAPManager iapMng;
    public Text text;
    public Button InitBtn;

    private void Awake()
    {
        InitBtn.onClick.AddListener(() => 
        {
            iapMng = new IAPManager();
            iapMng.Init();
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
        Logger.Log("You received " + credits + " Credits!");
        iapMng.OnPurchaseClicked(credits);
    }

    public void ShowMsg(string msg)
    {
        text.text += CommonFunction.ToUTF8Bom(msg) + "\n";
        Logger.Error(msg);
    }
}
