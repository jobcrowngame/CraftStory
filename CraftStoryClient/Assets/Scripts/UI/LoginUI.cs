using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UIBase
{
    Image Start;
    Text Ver;
    Button BGBtn;
    Button Terms01Btn;
    Button Terms02Btn;

    bool Lock = false;

    public override void Init()
    {
        base.Init();

        LoginLg.E.Init(this);

        Start = FindChiled<Image>("Start");

        Ver = FindChiled<Text>("Ver");
        Ver.text = "Ver:" + Application.version;

        BGBtn = FindChiled<Button>("BG");
        BGBtn.onClick.AddListener(()=> 
        {
            if (Lock)
                return;

            Lock = true;

            NowLoadingLG.E.FixtTips = 0;

            if (DataMng.E.RuntimeData.GuideEnd3 == 1)
            {
                CommonFunction.GoToNextScene(100);
            }
            else
            {
                DataMng.E.RuntimeData.GuideId = 3;
                CommonFunction.GoToNextScene(101);
            }
        });
        BGBtn.enabled = false;

        Terms01Btn = FindChiled<Button>("Terms01Btn");
        Terms01Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms01UI>(UIType.Terms01); });

        Terms02Btn = FindChiled<Button>("Terms02Btn");
        Terms02Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms02UI>(UIType.Terms02); });
    }

    public void LoginResponse()
    {
        Logger.Log("ログイン成功しました。");
        Logger.Log("ようこそ、{0}", DataMng.E.UserData.Account);

        Start.sprite = ReadResources<Sprite>("Textures/button_2d_003");

        BGBtn.enabled = true;

        UICtl.E.LockUI(false);
    }
}
