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
    Button FriendBtn { get => FindChiled<Button>("FriendBtn"); }

    Button PointExchangeBtn { get => FindChiled<Button>("PointExchangeBtn"); }
    Button MyShopBtn { get => FindChiled<Button>("MyShopBtn"); }
    Button PlayDescriptionBtn2 { get => FindChiled<Button>("PlayDescriptionBtn2"); }
    Button MessageBtn { get => FindChiled<Button>("MessageBtn"); }
    Button Notice { get => FindChiled<Button>("Notice"); }
    Button Debug { get => FindChiled<Button>("Debug"); }

    MapType menuType
    {
        set
        {
            AdventureBtn.gameObject.SetActive(false);
            ShopBtn.gameObject.SetActive(false);
            HomeBtn.gameObject.SetActive(false);
            PlayDescriptionBtn.gameObject.SetActive(false);
            PersonalMessageBtn.gameObject.SetActive(false);
            FriendBtn.gameObject.SetActive(false);

            PointExchangeBtn.gameObject.SetActive(false);
            MyShopBtn.gameObject.SetActive(false);
            PlayDescriptionBtn2.gameObject.SetActive(false);
            MessageBtn.gameObject.SetActive(false);

            Notice.gameObject.SetActive(false);
            Debug.gameObject.SetActive(false);

            switch (value)
            {
                case MapType.Home:
                    AdventureBtn.gameObject.SetActive(true);
                    ShopBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn.gameObject.SetActive(true);
                    PersonalMessageBtn.gameObject.SetActive(true);
                    //FriendBtn.gameObject.SetActive(true);

                    PointExchangeBtn.gameObject.SetActive(true);
                    MyShopBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn2.gameObject.SetActive(true);
                    MessageBtn.gameObject.SetActive(true);
                    //Notice.gameObject.SetActive(true);
                    //Debug.gameObject.SetActive(true);
                    break;

                case MapType.Brave:
                case MapType.FriendHome:
                    HomeBtn.gameObject.SetActive(true);
                    break;

                case MapType.Guide:
                    AdventureBtn.gameObject.SetActive(true);
                    ShopBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn.gameObject.SetActive(true);
                    PersonalMessageBtn.gameObject.SetActive(true);
                    //FriendBtn.gameObject.SetActive(true);

                    PointExchangeBtn.gameObject.SetActive(true);
                    MyShopBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn2.gameObject.SetActive(true);
                    MessageBtn.gameObject.SetActive(true);

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

            GuideLG.E.Next();
        });
        HomeBtn.onClick.AddListener(() =>
        {
            GuideLG.E.Next();
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

            GuideLG.E.Next();
        });
        FriendBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<FriendUI>(UIType.Friend);
            Close();
        });

        PointExchangeBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.RuntimeData.Coin3 < 1000)
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
            if (string.IsNullOrEmpty(DataMng.E.RuntimeData.NickName))
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

            GuideLG.E.Next();
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

    public override void Init()
    {
        base.Init();
        MenuLG.E.Init(this);

        menuType = DataMng.E.RuntimeData.MapType;
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
}