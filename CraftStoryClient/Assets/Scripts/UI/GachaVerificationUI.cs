using UnityEngine.UI;

public class GachaVerificationUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Text Count1 { get => FindChiled<Text>("Count1"); }
    Text Count2 { get => FindChiled<Text>("Count2"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    int mGachaId;
    string des = @"クラフトシードを{0}個消費して、
{1}ガチャを10回実行します。
";

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
        CancelBtn.onClick.AddListener(Close);
    }

    public override void Open(object gachaId)
    {
        base.Open(gachaId);

        var config = ConfigMng.E.Gacha[mGachaId];
        Count1.text = DataMng.E.RuntimeData.Coin1.ToString();
        Count2.text = (DataMng.E.RuntimeData.Coin1 - config.CostCount).ToString();
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
