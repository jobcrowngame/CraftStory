using UnityEngine.UI;

public class ShopBlueprintUserCell : UIBase
{
    Image Icon { get => transform.GetComponent<Image>(); }
    Button GoodBtn { get => FindChiled<Button>("Button"); }
    Text GoodNum { get => FindChiled<Text>("Text"); }
    Button ShowDetailsBtn { get => gameObject.GetComponent<Button>(); }

    public string TargetAcc { get => data.targetAcc; }
    MyShopItem data;

    private void Awake()
    {
        GoodBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.RuntimeData.MyShopGoodNum >= 3)
            {
                CommonFunction.ShowHintBar(1047001);
                return;
            }

            NWMng.E.MyShopGoodEvent((rp) =>
            {
                ShopBlueprintLG.E.RefreshGoodNum(data.targetAcc);
                DataMng.E.RuntimeData.MyShopGoodNum++;
            }, data.targetAcc);
        });

        ShowDetailsBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<ShopBlueprintDetailsUI>(UIType.ShopBlueprintDetails, UIOpenType.None, data);
        });
    }

    public void Set(MyShopItem data)
    {
        this.data = data;

        if (data.itemId == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(data.icon)) AWSS3Mng.E.DownLoadTexture2D(Icon, data.icon);
        GoodNum.text = data.goodNum.ToString();
    }

    /// <summary>
    /// いいね数追加
    /// </summary>
    public void GoodNumberAdd()
    {
        data.goodNum++;
        GoodNum.text = data.goodNum.ToString();
    }
}