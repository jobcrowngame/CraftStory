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
            }
            else
            {
                UI.Explanatory.text = UI.ExplanatoryNoSelect;
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
