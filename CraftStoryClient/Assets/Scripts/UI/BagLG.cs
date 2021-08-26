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
            if (selectItem != null) selectItem.IsSelected(true);
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
    private BagClassification classification = BagClassification.All;

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
        Block,

        /// <summary>
        /// 他
        /// </summary>
        Orther,

        /// <summary>
        /// 素材
        /// </summary>
        Material,

        /// <summary>
        /// 設計図
        /// </summary>
        Blueprint,
    }
}
