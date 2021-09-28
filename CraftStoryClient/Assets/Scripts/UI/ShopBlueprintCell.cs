using UnityEngine.UI;

public class ShopBlueprintCell : UIBase
{
    Image Icon { get => transform.GetComponent<Image>(); }
    Button GoodBtn { get => FindChiled<Button>("Button"); }
    Text GoodNum { get => FindChiled<Text>("Text"); }

    MyShopItem data;

    public void Set(MyShopItem data)
    {
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
    }
}