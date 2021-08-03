using UnityEngine;
using UnityEngine.UI;

public class MenuUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    //Button CraftBtn;
    Button AdventureBtn { get => FindChiled<Button>("AdventureBtn"); }

    Button ShopBtn { get => FindChiled<Button>("ShopBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button PlayDescriptionBtn { get => FindChiled<Button>("PlayDescriptionBtn"); }
    Button PersonalMessageBtn { get => FindChiled<Button>("PersonalMessageBtn"); }

    Button PointExchangeBtn { get => FindChiled<Button>("PointExchangeBtn"); }
    Button MyShopBtn { get => FindChiled<Button>("MyShopBtn"); }
    Button PlayDescriptionBtn2 { get => FindChiled<Button>("PlayDescriptionBtn2"); }
    Button MessageBtn { get => FindChiled<Button>("MessageBtn"); }
    Button Notice { get => FindChiled<Button>("Notice"); }
    Button Debug { get => FindChiled<Button>("Debug"); }

    MenuUIType menuType
    {
        set
        {
            AdventureBtn.gameObject.SetActive(false);
            ShopBtn.gameObject.SetActive(false);
            HomeBtn.gameObject.SetActive(false);
            PlayDescriptionBtn.gameObject.SetActive(false);
            PersonalMessageBtn.gameObject.SetActive(false);

            PointExchangeBtn.gameObject.SetActive(false);
            MyShopBtn.gameObject.SetActive(false);
            PlayDescriptionBtn2.gameObject.SetActive(false);
            MessageBtn.gameObject.SetActive(false);

            Notice.gameObject.SetActive(false);
            Debug.gameObject.SetActive(false);

            switch (value)
            {
                case MenuUIType.Home:
                    AdventureBtn.gameObject.SetActive(true);
                    ShopBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn.gameObject.SetActive(true);
                    PersonalMessageBtn.gameObject.SetActive(true);

                    PointExchangeBtn.gameObject.SetActive(true);
                    MyShopBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn2.gameObject.SetActive(true);
                    MessageBtn.gameObject.SetActive(true);
                    Notice.gameObject.SetActive(true);
                    Debug.gameObject.SetActive(true);
                    break;

                case MenuUIType.Brave:
                    HomeBtn.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void Awake()
    {
        CloseBtn.onClick.AddListener(() => { Close(); });
        AdventureBtn.onClick.AddListener(() =>
        {
            CommonFunction.GoToNextScene(1000);
            Close();
        });

        ShopBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<ShopUI>(UIType.Shop);
            Close();
        });
        HomeBtn.onClick.AddListener(() =>
        {
            //CommonFunction.GoToHome();
            CommonFunction.GoToNextScene(100);
            Close();
        });
        PlayDescriptionBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<PlayDescriptionUI>(UIType.PlayDescription);
            Close();
        });
        PersonalMessageBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<PersonalMessageUI>(UIType.PersonalMessage);
            Close();
        });

        PointExchangeBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.UserData.Coin3 < 1000)
            {
                CommonFunction.ShowHintBar(18);
                return;
            }

            string msg = @"ポイント交換ページへ遷移します。

　　※1000ポイントから交換することが可能です。";

            CommonFunction.ShowHintBox(msg, () =>
            {
                Application.OpenURL("https://www.craftstory.jp/exchangepoints/");
                Close();
            }, () => { });
        });
        PlayDescriptionBtn2.onClick.AddListener(() =>
        {
            string msg = "詳しい遊び方を紹介している、" +
"公式のホームページに遷移します。";

            CommonFunction.ShowHintBox(msg, () =>
            {
                Application.OpenURL("https://www.craftstory.jp/howtoplay/home/");
                Close();
            }, () => { });
        });
        MyShopBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(DataMng.E.UserData.NickName))
            {
                string msg = @"マイショップを利用するには、
ニックネームを登録する必要があります。";

                CommonFunction.ShowHintBox(msg, ()=> 
                {
                    UICtl.E.OpenUI<PersonalMessageUI>(UIType.PersonalMessage);
                    Close();
                });
                return;
            }

            UICtl.E.OpenUI<MyShopUI>(UIType.MyShop);
            Close();
        });
        MessageBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<EmailUI>(UIType.Email);
            Close();
        });
        Notice.onClick.AddListener(() => 
        {
            UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
            Close();
        });
        Debug.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<DebugUI>(UIType.Debug);
            Close();
        });
    }

    public void Init(MenuUIType uiType)
    {
        base.Init();
        MenuLG.E.Init(this);

        menuType = uiType;
    }
    public override void Open()
    {
        base.Open();
        RefreshRedPoint();
    }
    public void RefreshRedPoint()
    {
        var RedPoint = FindChiled("RedPoint", MessageBtn.gameObject);
        if(RedPoint != null) RedPoint.gameObject.SetActive(CommonFunction.NewMessage());
    }

    public enum MenuUIType
    {
        Home,
        Brave,
    }
}