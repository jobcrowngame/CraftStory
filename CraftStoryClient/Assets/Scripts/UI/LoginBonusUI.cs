
using UnityEngine;
using UnityEngine.UI;

public class LoginBonusUI : UIBase
{
    public LoginBonusCell[] cells1;
    public LoginBonusCell[] cells2;

    Transform Plan1 { get => FindChiled("Plan1"); }
    Transform Plan2 { get => FindChiled("Plan2"); }
    Image Icon1 { get => FindChiled<Image>("Icon1"); }
    Image Icon2 { get => FindChiled<Image>("Icon2"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button ClickBtn { get => FindChiled<Button>("ClickBtn"); }

    bool isEnd = false;
    bool bonusIsGeted = false;
    int curId = 0;
    int step = 0;
    int type = 1;

    public override void Init()
    {
        base.Init();

        LoginBonusLG.E.Init(this);

        ClickBtn.onClick.AddListener(OnClick);
    }

    public override void Open()
    {
        base.Open();

        bonusIsGeted = false;
    }

    public void Set(LoginBonusLG.LoginBonusInfoCellRP info, int step)
    {
        curId = info.id;
        this.step = step;
        type = info.type;

        string[] items = info.items.Split(',');
        string[] counts = info.itemCounts.Split(',');

        // 全部のアイテムをゲットした場合、当該画面を閉じて次の画面に遷移
        if (step >= items.Length)
        {
            bonusIsGeted = true;
            OnClick();
            return;
        }

        Plan1.gameObject.SetActive(info.type == 1);
        Plan2.gameObject.SetActive(info.type == 2);
       
        // テーマアイコンセット
        AWSS3Mng.E.LoadLoginBonusTexture2D(Icon1, info.themeTexture);

        // アイテムアイコン
        var curStepItemConfig = ConfigMng.E.Item[int.Parse(items[step])];
        Icon2.sprite = ReadResources<Sprite>(curStepItemConfig.IconResourcesPath);

        // アイテム説明
        Des.text = curStepItemConfig.Explanatory;

        // イベント時間
        Time.text = string.Format("{0}～{1}", info.start_at.ToString("D"), info.end_at.ToString("D"));

        if (info.type == 1)
        {
            SetCell(items, counts, cells1, step);
        }
        else
        {
            SetCell(items, counts, cells2, step);
        }
    }

    private void SetCell(string[] items, string[] counts, LoginBonusCell[] cells, int step)
    {
        if (items.Length != cells.Length || counts.Length != cells.Length)
        {
            Logger.Error("bad DB items or itemsCount");
            return;
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Set(items[i], counts[i], i, i < step);
        }
    }

    private void OnClick()
    {
        if (bonusIsGeted)
        {
            Close();
            LoginBonusLG.E.OpenNextUI();
        }
        else
        {
            ClickBtn.enabled = false;
            NWMng.E.GetLoginBonus((rp) =>
            {
                NWMng.E.GetItems();
                NWMng.E.GetCoins(null);
                bonusIsGeted = true;
                ClickBtn.enabled = true;

                if (type == 1)
                {
                    cells1[step].IsGeted();
                }
                else
                {
                    cells2[step].IsGeted();
                }

            }, curId, step);
        }
    }
}
