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
                    UI.EatButton.gameObject.SetActive(true);
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
            if (classification == value)
                return;

            classification = value;
            UI.RefreshItems();
            UI.ChangeSelectBtn((int)value);
        }
    }
    private BagClassification classification = BagClassification.None;

    /// <summary>
    /// 分類が変更イベント
    /// </summary>
    /// <param name="index"></param>
    public void OnClickClassificationBtn(int index)
    {
        Classification = (BagClassification)index;
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
        /// 他
        /// </summary>
        Orther = 2,

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
        Cocooking = 5,

        /// <summary>
        /// 装備
        /// </summary>
        Ecuipment = 6,
    }
}
