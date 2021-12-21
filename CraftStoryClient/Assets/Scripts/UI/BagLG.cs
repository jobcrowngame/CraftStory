using JsonConfigData;
/// <summary>
/// 持ち物ロジック
/// </summary>
public class BagLG : UILogicBase<BagLG, BagUI>
{
    /// <summary>
    /// 選択したサブ
    /// </summary>
    public BagItemCell SelectItem 
    {
        get => selectItem;
        set
        {
            if (selectItem != null)
            {
                selectItem.IsSelected(false);
            }

            selectItem = value;
            if (selectItem != null)
            {
                selectItem.IsSelected(true);
                UI.Explanatory.text = ConfigMng.E.Item[selectItem.ItemData.itemId].Explanatory;

                // 食べ物の場合
                if (ConfigMng.E.Food.ContainsKey(selectItem.ItemData.itemId))
                {
                    Food food = ConfigMng.E.Food[selectItem.ItemData.itemId];
                    string recval = food.Amount != -1 ? food.Amount.ToString() : $"{food.Percent}%";
                    UI.FoodEffect.text =
                        food.Type == 1 ? $"満腹度：{recval}回復" :
                        "";
                    UI.EatButton.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home); // ショップ広場では出さない
                }
                else
                {
                    UI.FoodEffect.text = "";
                    UI.EatButton.gameObject.SetActive(false);
                }
            }
            else
            {
                UI.ResetExplanatory();
            }
        }
    }
    private BagItemCell selectItem;

    /// <summary>
    /// 持ち物の分類
    /// </summary>
    public BagClassification Classification 
    {
        get => classification;
        set
        {
            //if (classification == value)
            //    return;

            classification = value;
            UI.RefreshItems();
            UI.ChangeSelectBtn(BagClassfication2Index(value));
        }
    }
    private BagClassification classification = BagClassification.None;

    /// <summary>
    /// 分類が変更イベント
    /// </summary>
    /// <param name="index"></param>
    public void OnClickClassificationBtn(int index)
    {
        Classification = Index2BagClassification(index);
    }

    public static BagClassification Index2BagClassification(int index)
    {
        switch (index)
        {
            case 0: return BagClassification.All; ;
            case 1: return BagClassification.Tool;
            case 2: return BagClassification.Block;
            case 3: return BagClassification.Decoration;
            case 4: return BagClassification.Blueprint;
            case 5: return BagClassification.Food;
            case 6: return BagClassification.Material;
            case 7: return BagClassification.Equipment;

            default: Logger.Error("知らない持ち物カテゴリ " + index); return BagClassification.None;
        }
    }
    public static int BagClassfication2Index(BagClassification bclassification)
    {
        switch (bclassification)
        {
            case BagClassification.All: return 0;
            case BagClassification.Block: return 2;
            case BagClassification.Decoration: return 3;
            case BagClassification.Material: return 6;
            case BagClassification.Blueprint: return 4;
            case BagClassification.Food: return 5;
            case BagClassification.Equipment: return 7;
            case BagClassification.Tool: return 1;
            default: return -1;
        }
    }

    /// <summary>
    /// 分類
    /// </summary>
    public enum BagClassification 
    {
        None = -1,

        /// <summary>
        /// 全部
        /// </summary>
        All = 0,

        /// <summary>
        /// ブロック
        /// </summary>
        Block = 1,

        /// <summary>
        /// 装飾
        /// </summary>
        Decoration = 2,

        /// <summary>
        /// 素材
        /// </summary>
        Material = 3,

        /// <summary>
        /// 設計図
        /// </summary>
        Blueprint = 4,

        /// <summary>
        /// 料理
        /// </summary>
        Food = 5,

        /// <summary>
        /// 装備
        /// </summary>
        Equipment = 6,

        /// <summary>
        /// 道具
        /// </summary>
        Tool = 7,
    }
}
