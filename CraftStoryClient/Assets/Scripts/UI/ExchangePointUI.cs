using UnityEngine;
using UnityEngine.UI;

public class ExchangePointUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    InputField MailInput { get => FindChiled<InputField>("MailInput"); }
    Text MailInputCheck { get => FindChiled<Text>("MailInputCheck"); }
    InputField PointInput { get => FindChiled<InputField>("PointInput"); }
    Text MyPointCount { get => FindChiled<Text>("MyPointCount"); }
    Text ExchangeCount { get => FindChiled<Text>("ExchangeCount"); }
    Text PointInputCheck { get => FindChiled<Text>("PointInputCheck"); }
    Toggle Toggle { get => FindChiled<Toggle>("Toggle"); }
    Button LinkBtn1 { get => FindChiled<Button>("LinkBtn (1)"); }
    Button LinkBtn2 { get => FindChiled<Button>("LinkBtn (2)"); }
    Button SubmitBtn { get => FindChiled<Button>("SubmitBtn"); }

    string exchangeText = @"{0}円分のギフト券を
お送りします
";

    public override void Init()
    {
        base.Init();
        ExchangePointLG.E.Init(this);

        Title.SetTitle("メニュー");
        Title.SetOnClose(() => { Close(); });
        Title.EnActiveCoin(1);
        Title.EnActiveCoin(2);
        Title.EnActiveCoin(3);

        MailInput.onEndEdit.AddListener((msg)=> 
        {
            CheckMailAdd();
        });
        PointInput.onEndEdit.AddListener((msg) =>
        {
            int inputCount = 0;
            if (!string.IsNullOrEmpty(PointInput.text))
            {
                inputCount = int.Parse(PointInput.text);
                if (inputCount > DataMng.E.RuntimeData.Coin3)
                {
                    inputCount = DataMng.E.RuntimeData.Coin3;
                    PointInput.text = inputCount.ToString();
                }

                int offset = inputCount % 10;
                if (offset > 0)
                {
                    inputCount -= offset;
                    PointInput.text = inputCount.ToString();
                }
            }

            if (CheckPointInput() == 0)
            {
                int exchangeCount = (int)(inputCount * 0.3f);
                ExchangeCount.text = string.Format(exchangeText, exchangeCount);
            }
        });
        Toggle.onValueChanged.AddListener((b) => 
        {
            CheckAll();
        });

        LinkBtn1.onClick.AddListener(() => { Application.OpenURL("https://www.craftstory.jp/termsofuse/"); });
        LinkBtn2.onClick.AddListener(() => { Application.OpenURL("https://www.craftstory.jp/privacypolicy/"); });
        SubmitBtn.onClick.AddListener(Submit);
    }

    public override void Open()
    {
        base.Open();

        MailInputCheck.gameObject.SetActive(false);
        PointInputCheck.gameObject.SetActive(false);
        EnableSubmitBtn(false);

        //CheckAll();

        if (!string.IsNullOrEmpty(DataMng.E.RuntimeData.Email))
        {
            MailInput.text = DataMng.E.RuntimeData.Email;
        }

        MyPointCount.text = string.Format("所持：{0}ポイント", DataMng.E.RuntimeData.Coin3);
        ExchangeCount.text = string.Format(exchangeText, 0);
    }

    /// <summary>
    /// 全体入力チェック
    /// </summary>
    /// <returns></returns>
    private int CheckAll()
    {
        int result = 0;
        result += CheckMailAdd();
        result += CheckPointInput();
        result += CheckAgreement();

        EnableSubmitBtn(result == 0);

        return result;
    }

    /// <summary>
    /// 申請ボタンのアクティブ
    /// </summary>
    /// <param name="b"></param>
    private void EnableSubmitBtn(bool b)
    {
        SubmitBtn.enabled = b;
        SubmitBtn.image.color = b ? Color.white : Color.grey;
    }

    /// <summary>
    /// 申請
    /// </summary>
    private void Submit()
    {
        if (CheckAll() > 0)
        {
            CommonFunction.ShowHintBar(27);
            return;
        }

        NWMng.E.ExchangePoints((rp)=> 
        {
            DataMng.E.RuntimeData.Coin3 -= int.Parse(PointInput.text);
            CommonFunction.ShowHintBar(5);
            Close();
        }, int.Parse(PointInput.text), MailInput.text);
    }

    /// <summary>
    /// メールアドレスのチェック
    /// </summary>
    /// <returns></returns>
    private int CheckMailAdd()
    {
        if (string.IsNullOrEmpty(MailInput.text))
        {
            MailInputCheck.gameObject.SetActive(true);
            MailInputCheck.text = "必須項目をご記入ください。";
            return 1;
        }

        if (CommonFunction.IsValidMailAddress(MailInput.text))
        {
            MailInputCheck.gameObject.SetActive(false);
            return 0;
        }
        else
        {
            MailInputCheck.gameObject.SetActive(true);
            MailInputCheck.text = "入力されたメールアドレスに間違いがあります。";
            return 1;
        }
    }

    /// <summary>
    /// ポイント入力のチェック
    /// </summary>
    /// <returns></returns>
    private int CheckPointInput()
    {
        if (string.IsNullOrEmpty(PointInput.text))
        {
            PointInputCheck.gameObject.SetActive(true);
            PointInputCheck.text = "交換ポイントを入力してください。";
            return 1;
        }

        if (int.Parse(PointInput.text) <= 0)
        {
            PointInputCheck.gameObject.SetActive(true);
            PointInputCheck.text = "正しい数を入力してください。";
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// 契約のチェック
    /// </summary>
    /// <returns></returns>
    private int CheckAgreement()
    {
        return Toggle.isOn ? 0 : 1;
    }
}
