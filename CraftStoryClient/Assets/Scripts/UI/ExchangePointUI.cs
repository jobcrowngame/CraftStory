using UnityEngine;
using UnityEngine.UI;

public class ExchangePointUI : UIBase
{
    ScrollRect ScrollRect { get => FindChiled<ScrollRect>("Scroll View"); }
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    InputField MailInput { get => FindChiled<InputField>("MailInput"); }
    Text MailInputCheck { get => FindChiled<Text>("MailInputCheck"); }
    InputField PointInput { get => FindChiled<InputField>("PointInput"); }
    Text MyPointCount { get => FindChiled<Text>("MyPointCount"); }
    Text ExchangeCount { get => FindChiled<Text>("ExchangeCount"); }
    Toggle Toggle { get => FindChiled<Toggle>("Toggle"); }
    Button LinkBtn1 { get => FindChiled<Button>("LinkBtn (1)"); }
    Button LinkBtn2 { get => FindChiled<Button>("LinkBtn (2)"); }
    Button SubmitBtn { get => FindChiled<Button>("SubmitBtn"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    const string exchangeText = @"{0}円分のギフト券を
お送りします
";

//    const string echangeStartTitle = "ポイント交換申請の内容確認";
//    const string exchangeStartText = @"締切日後、５営業日以内に以下メールアドレス宛へ
//Amazonギフト券をお送りいたします。

//・メールアドレス
//{0}

//・交換ポイント
//{1}

//・Amazonギフト券{2}円分をお送りします。

//毎月5日と20日が締め切り日です。
//※申請状況により、
//お送りする日にちが前後する場合がございます。
//何卒ご理解、ご了承の程お願いいたします。
//";

    const string echangeOverTitle = "ポイント交換申請の受付完了";
    const string exchangeOverText = @"ポイント交換申請を受付けました。

・受付No
{0}

締切日後、５営業日以内に以下メールアドレス宛へ
Amazonギフト券をお送りいたします。

・メールアドレス
{1}

毎月5日と20日が締め切り日です。
※申請状況により、
お送りする日にちが前後する場合がございます。
何卒ご理解、ご了承の程お願いいたします。";

    public override void Init()
    {
        base.Init();
        ExchangePointLG.E.Init(this);

        Title.SetOnClose(() => { Close(); });

        MailInput.onEndEdit.AddListener((msg)=> 
        {
            CheckMailAdd();
        });
        PointInput.onEndEdit.AddListener((msg) =>
        {
            OnPointInputValueChange();
            
        });
        Toggle.onValueChanged.AddListener((b) => 
        {
            CheckAll();
        });

        LinkBtn1.onClick.AddListener(() => { Application.OpenURL("https://www.craftstory.jp/termsofuse/"); });
        LinkBtn2.onClick.AddListener(() => { Application.OpenURL("https://www.craftstory.jp/privacypolicy/"); });
        SubmitBtn.onClick.AddListener(Submit);

        OkBtn.onClick.AddListener(OnClickOk);
        CancelBtn.onClick.AddListener(InputMode);
    }

    public override void Open()
    {
        base.Open();

        InputMode();

        MailInputCheck.gameObject.SetActive(false);
        EnableSubmitBtn(false);
        Toggle.isOn = false;

        //CheckAll();

        MailInput.text = string.IsNullOrEmpty(DataMng.E.RuntimeData.Email) ? "" : DataMng.E.RuntimeData.Email;

        string point = string.Format("所持：{0}ポイント", DataMng.E.RuntimeData.Coin3.ToString("#,0"));
        MyPointCount.text = point;
        SetPoint(1000);
        OnPointInputValueChange();
    }

    private void InputMode()
    {
        Title.SetTitle("ポイント交換申請");

        MailInput.interactable = true;
        MailInput.GetComponent<Image>().color = Color.white;

        PointInput.interactable = true;
        PointInput.GetComponent<Image>().color = Color.white;

        Toggle.interactable = true;
        FindChiled<Image>("Background", Toggle.transform).color = Color.white;

        SubmitBtn.gameObject.SetActive(true);
        OkBtn.gameObject.SetActive(false);
        CancelBtn.gameObject.SetActive(false);

        ScrollRect.verticalNormalizedPosition = 1;
    }
    private void CheckMode()
    {
        Title.SetTitle("ポイント交換確認");

        MailInput.interactable = false;
        MailInput.GetComponent<Image>().color = Color.grey;

        PointInput.interactable = false;
        PointInput.GetComponent<Image>().color = Color.grey;

        Toggle.interactable = false;
        FindChiled<Image>("Background", Toggle.transform).color = Color.gray;

        SubmitBtn.gameObject.SetActive(false);
        OkBtn.gameObject.SetActive(true);
        CancelBtn.gameObject.SetActive(true);

        ScrollRect.verticalNormalizedPosition = 1;
    }

    /// <summary>
    /// 全体入力チェック
    /// </summary>
    /// <returns></returns>
    private int CheckAll()
    {
        int result = 0;
        result += CheckMailAdd();
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

        CheckMode();
    }
    private void OnClickOk()
    {
        int point = int.Parse(PointInput.text);
        int money = (int)(point * 0.3f);

        NWMng.E.ExchangePoints((rp) =>
        {
            DataMng.E.RuntimeData.Coin3 -= int.Parse(PointInput.text);
            Close();

            string guid = (string)rp["guid"];
            CommonFunction.ShowHintBox(echangeOverTitle, "", string.Format(exchangeOverText, guid, MailInput.text),
                () => { Close(); }, null);

            DataMng.E.RuntimeData.NewEmailCount++;
            DataMng.E.RuntimeData.Email = MailInput.text;
            HomeLG.E.UI.RefreshRedPoint();
            HomeLG.E.UI.RefreshCoins();
        }, point, money, MailInput.text);
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
    /// 契約のチェック
    /// </summary>
    /// <returns></returns>
    private int CheckAgreement()
    {
        return Toggle.isOn ? 0 : 1;
    }

    /// <summary>
    /// Pointの入力が変化する場合
    /// </summary>
    private void OnPointInputValueChange()
    {
        int inputCount = 0;
        if (!string.IsNullOrEmpty(PointInput.text))
        {
            inputCount = int.Parse(PointInput.text);

            // 持ってるポイントより大きい数を入力すると、持ってる数と同じにする
            if (inputCount > DataMng.E.RuntimeData.Coin3)
            {
                inputCount = DataMng.E.RuntimeData.Coin3;
            }

            // 最大 100000　にする
            if (inputCount > 100000)
            {
                inputCount = 100000;
            }

            // 1000より小さい数を入力すると、1000を設定する
            if (inputCount < 1000)
            {
                inputCount = 1000;
            }

            // 最小入力ステップは10にする
            int offset = inputCount % 10;
            if (offset > 0)
            {
                inputCount -= offset;
            }

            SetPoint(inputCount);
        }

        int exchangeCount = (int)(inputCount * 0.3f);
        ExchangeCount.text = string.Format(exchangeText, exchangeCount.ToString("#,0"));
    }

    private void SetPoint(int count)
    {
        PointInput.text = count.ToString("#,0");
    }
}
