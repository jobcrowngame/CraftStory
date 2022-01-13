using UnityEngine;
using UnityEngine.UI;

public class MenuUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    MyScrollRect Banner { get => FindChiled<MyScrollRect>("Banner"); }
    Transform PointList { get => FindChiled("PointList"); }

    //Button CraftBtn;
    Button AdventureBtn { get => FindChiled<Button>("AdventureBtn"); }
    Button PlayDescriptionBtn { get => FindChiled<Button>("PlayDescriptionBtn"); }
    Button PersonalMessageBtn { get => FindChiled<Button>("PersonalMessageBtn"); }

    Button PlayDescriptionBtn2 { get => FindChiled<Button>("PlayDescriptionBtn2"); }
    Button MessageBtn { get => FindChiled<Button>("MessageBtn"); }
    Button Notice { get => FindChiled<Button>("Notice"); }
    Button Debug { get => FindChiled<Button>("Debug"); }

    MapType menuType
    {
        set
        {
            Banner.gameObject.SetActive(false);
            PointList.gameObject.SetActive(false);

            AdventureBtn.gameObject.SetActive(false);
            PlayDescriptionBtn.gameObject.SetActive(false);
            PersonalMessageBtn.gameObject.SetActive(false);

            PlayDescriptionBtn2.gameObject.SetActive(false);
            MessageBtn.gameObject.SetActive(false);

            Notice.gameObject.SetActive(false);
            Debug.gameObject.SetActive(false);

            switch (value)
            {
                case MapType.Home:
                    Banner.gameObject.SetActive(true);
                    PointList.gameObject.SetActive(true);

                    //AdventureBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn.gameObject.SetActive(true);
                    PersonalMessageBtn.gameObject.SetActive(true);
                    Notice.gameObject.SetActive(true);
                    //FriendBtn.gameObject.SetActive(true);

                    PlayDescriptionBtn2.gameObject.SetActive(true);
                    MessageBtn.gameObject.SetActive(true);
                    //Debug.gameObject.SetActive(true);
                    break;

                case MapType.Brave:
                case MapType.FriendHome:
                    break;

                case MapType.Guide:
                    //AdventureBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn.gameObject.SetActive(true);
                    PersonalMessageBtn.gameObject.SetActive(true);
                    //FriendBtn.gameObject.SetActive(true);

                    PlayDescriptionBtn2.gameObject.SetActive(true);
                    MessageBtn.gameObject.SetActive(true);

                    break;

                case MapType.Market:
                    Banner.gameObject.SetActive(true);
                    PointList.gameObject.SetActive(true);

                    //AdventureBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn.gameObject.SetActive(true);
                    PersonalMessageBtn.gameObject.SetActive(true);
                    Notice.gameObject.SetActive(true);
                    //FriendBtn.gameObject.SetActive(true);

                    PlayDescriptionBtn2.gameObject.SetActive(true);
                    MessageBtn.gameObject.SetActive(true);
                    //Debug.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void Awake()
    {
        Title.SetTitle("メニュー");
        Title.SetOnClose(() => { Close(); });

        Banner.AddOnIndexChange((index) => { OnIndexChange(index); });
        for (int i = 0; i < Banner.content.childCount; i++)
        {
            var myBtn = Banner.content.GetChild(i).GetComponent<MyButton>();
            if (myBtn == null)
            {
                myBtn = Banner.gameObject.AddComponent<MyButton>();
            }
            myBtn.Index = i;
            myBtn.AddClickListener((index) => { OnClickBanner(index); });
        }

        AdventureBtn.onClick.AddListener(() =>
        {
            CommonFunction.GoToNextScene(1000);
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
        MessageBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<EmailUI>(UIType.Email);
            Close();
        });
        Notice.onClick.AddListener(() => 
        {
            NoticeUI ui = UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
            ui.MoveToTop();
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

        Banner.horizontalNormalizedPosition = 0;
        OnIndexChange(0);
    }
    public void RefreshRedPoint()
    {
        var RedPoint = FindChiled("RedPoint", MessageBtn.gameObject);
        if(RedPoint != null) RedPoint.gameObject.SetActive(CommonFunction.NewMessage());
    }

    private void OnClickBanner(int index)
    {
        Logger.Warning("OnClickBanner");
    }
    private void OnIndexChange(int index)
    {
        foreach (Transform item in PointList)
        {
            item.GetComponent<Image>().color = Color.white;
        }

        var cellPoint = PointList.GetChild(index);
        cellPoint.GetComponent<Image>().color = Color.yellow;
    }
}