using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class GachaVerificationUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text Des { get => FindChiled<Text>("Des"); }

    Transform CostWindow1 { get => FindChiled("CostWindow1"); }
    Text Count1 { get => FindChiled<Text>("Count1"); }
    Text Count2 { get => FindChiled<Text>("Count2"); }

    Transform CostWindow2 { get => FindChiled("CostWindow2"); }
    Text Count3 { get => FindChiled<Text>("Count3"); }

    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }
    Button ChageBtn { get => FindChiled<Button>("ChageBtn"); }

    Text LackText { get => FindChiled<Text>("LackText"); }

    Image CostImage1 { get => FindChiled<Image>("CostImage1"); } // ガチャボタン内のコストのアイコン(一番右)
    Image CostImage2 { get => FindChiled<Image>("CostImage2"); } // ガチャボタン内のコストのアイコン(その左隣)
    Image CostImage3 { get => FindChiled<Image>("CostImage3"); } // ガチャボタン内のコストのアイコン(不足時)

    int mGachaId;
    int mGachaGroup;
    Gacha gachaConfig;
    string des = @"{0}を{1}個消費して、
{2}を実行します。";
    string lackTextTmpl = "{0}が不足しています。";

    public override void Init()
    {
        base.Init();

        Title.SetTitle("ガチャ");
        Title.SetOnClose(() => { Close(); });

        OkBtn.onClick.AddListener(() =>
        {
            int costId = gachaConfig.Cost;
            int coinRemain =
                costId == 9000 ? DataMng.E.RuntimeData.Coin1 :
                costId == 9001 ? DataMng.E.RuntimeData.Coin2 :
                costId == 9002 ? DataMng.E.RuntimeData.Coin3 :
                costId == 9003 ? (DataMng.E.GetItemByItemId(costId) != null ? DataMng.E.GetItemByItemId(costId).count : 0) :
                0;


            if (coinRemain < gachaConfig.CostCount)
            {
                CommonFunction.ShowHintBar(1010001);
                return;
            }

            // タスク13のチェック
            if (gachaConfig.ID == 15)
                TaskMng.E.AddMainTaskCount(12);

            StartGacha(mGachaId);
            Close();
        });
        ChageBtn.onClick.AddListener(() => 
        {
            UICtl.E.CloseUI(UIType.ShopGacha);
            UICtl.E.OpenUI<ShopChargeUI>(UIType.ShopCharge);
            Close();
        });
        CancelBtn.onClick.AddListener(Close);
    }

    public void Set(int gachaId, int gachaGroup)
    {
        mGachaId = gachaId;
        mGachaGroup = gachaGroup;

        gachaConfig = ConfigMng.E.Gacha[mGachaId];
        string confItemName = ConfigMng.E.Item[gachaConfig.Cost].Name;
        Des.text = string.Format(des, confItemName, gachaConfig.CostCount, gachaConfig.Title);

        int costId = gachaConfig.Cost;
        int coinRemain =
            costId == 9000 ? DataMng.E.RuntimeData.Coin1 :
            costId == 9001 ? DataMng.E.RuntimeData.Coin2 :
            costId == 9002 ? DataMng.E.RuntimeData.Coin3 :
            costId == 9003 ? (DataMng.E.GetItemByItemId(costId) != null ? DataMng.E.GetItemByItemId(costId).count : 0) :
            0;

        GachaVerificationLG.UIType uiType = coinRemain >= gachaConfig.CostCount
            ? GachaVerificationLG.UIType.Type1
            : GachaVerificationLG.UIType.Type2;

        CostWindow1.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type1);
        CostWindow2.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type2);
        CostImage1.sprite = ReadResources<Sprite>(ConfigMng.E.Item[gachaConfig.Cost].IconResourcesPath);
        CostImage2.sprite = ReadResources<Sprite>(ConfigMng.E.Item[gachaConfig.Cost].IconResourcesPath);
        CostImage3.sprite = ReadResources<Sprite>(ConfigMng.E.Item[gachaConfig.Cost].IconResourcesPath);

        OkBtn.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type1);
        ChageBtn.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type2 && costId == 9000);

        LackText.text = string.Format(lackTextTmpl, confItemName);

        Count1.text = coinRemain.ToString();
        Count2.text = (coinRemain - gachaConfig.CostCount).ToString();
        Count3.text = coinRemain.ToString();
    }

    private void StartGacha(int gachaId)
    {
        int costId = gachaConfig.Cost;
        int costCount = gachaConfig.CostCount;
        int gachaCount = gachaConfig.GachaCount;
        if (costId != 9003)
        {
            if (DataMng.E.GetCoinByID(costId) < costCount)
            {
                if (costId == 9000) CommonFunction.ShowHintBar(1010001);
                if (costId == 9001) CommonFunction.ShowHintBar(1010002);
                if (costId == 9002) CommonFunction.ShowHintBar(1017001);

                return;
            }
        }
        else
        {
            if (DataMng.E.GetItemByItemId(costId) == null || DataMng.E.GetItemByItemId(costId).count < costCount)
            {
                if (costId == 9003) CommonFunction.ShowHintBar(1030001);
                return;
            }
        }

        NWMng.E.Gacha((rp) =>
        {
            if (string.IsNullOrEmpty(rp.ToString()))
            {
                Logger.Error("Bad gacha result");
            }

            var result = LitJson.JsonMapper.ToObject<ShopGachaLG.GachaResponse>(rp.ToJson());
            var ui = UICtl.E.OpenUI<GachaBonusUI>(UIType.GachaBonus);
            if (ui != null)
            {
                ui.Set(result, gachaId);
            }

            if (costId != 9003)
            {
                DataMng.E.ConsumableCoin(costId, costCount);
            }
            ShopGachaLG.E.UI.RefreshCoins();
            ShopGachaLG.E.UI.RefreshGachaUI();
        }, gachaId, mGachaGroup);
    }
}
