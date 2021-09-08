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

    int mGachaId;
    string des = @"クラフトシードを{0}個消費して、
{1}を10回実行します。";

    public override void Init(object gachaId)
    {
        base.Init(gachaId);
        mGachaId = (int)gachaId;

        Title.SetTitle("ガチャ");
        Title.SetOnClose(() => { Close(); });
        Title.EnActiveCoin(1);
        Title.EnActiveCoin(2);
        Title.EnActiveCoin(3);

        var config = ConfigMng.E.Gacha[mGachaId];
        Des.text = string.Format(des, config.CostCount, config.Title);

        OkBtn.onClick.AddListener(() => 
        {
            if (DataMng.E.RuntimeData.Coin1 < config.CostCount)
            {
                CommonFunction.ShowHintBar(1010001);
                return;
            }

            StartGacha(mGachaId);
            Close();
        });
        ChageBtn.onClick.AddListener(() => 
        {
            UICtl.E.OpenUI<ShopUI>(UIType.Shop, UIOpenType.None, 0);
            Close();
        });
        CancelBtn.onClick.AddListener(Close);
    }

    public override void Open(object gachaId)
    {
        base.Open(gachaId);

        var config = ConfigMng.E.Gacha[mGachaId];
        GachaVerificationLG.UIType uiType = DataMng.E.RuntimeData.Coin1 >= config.CostCount
            ? GachaVerificationLG.UIType.Type1
            : GachaVerificationLG.UIType.Type2;

        CostWindow1.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type1);
        CostWindow2.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type2);

        OkBtn.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type1);
        ChageBtn.gameObject.SetActive(uiType == GachaVerificationLG.UIType.Type2);


        Count1.text = DataMng.E.RuntimeData.Coin1.ToString();
        Count2.text = (DataMng.E.RuntimeData.Coin1 - config.CostCount).ToString();
        Count3.text = DataMng.E.RuntimeData.Coin1.ToString();
    }

    private void StartGacha(int gachaId)
    {
        int costId = ConfigMng.E.Gacha[gachaId].Cost;
        int costCount = ConfigMng.E.Gacha[gachaId].CostCount;
        if (DataMng.E.GetCoinByID(costId) < costCount)
        {
            if (costId == 9000) CommonFunction.ShowHintBar(1010001);
            if (costId == 9001) CommonFunction.ShowHintBar(1010002);
            if (costId == 9002) CommonFunction.ShowHintBar(1017001);

            return;
        }

        NWMng.E.Gacha10((rp) =>
        {
            if (string.IsNullOrEmpty(rp.ToString()))
            {
                Logger.Error("Bad gacha result");
            }

            var result = LitJson.JsonMapper.ToObject<ShopLG.GachaResponse>(rp.ToJson());
            var ui = UICtl.E.OpenUI<GachaBonusUI>(UIType.GachaBonus);
            if (ui != null)
            {
                ui.Set(result, gachaId);
            }

            DataMng.E.ConsumableCoin(costId, costCount);
            ShopLG.E.UI.RefreshCoins();
        }, gachaId);
    }
}
