
public class MapLG : UILogicBase<MapLG, MapUI>
{
    private bool mLockBtn;
    public bool BtnIsLocked { get => mLockBtn; }

    public bool IsEquipTutorial()
    {
        return LocalDataMng.E.Data.limitedT.guide_end4 == 0 &&
            PlayerCtl.E.GetEquipByItemType(ItemType.Weapon) == null;
    }

    /// <summary>
    /// 連打を制限する為、ボタンをロック
    /// </summary>
    /// <param name="b"></param>
    public void LockBtn(bool b = true)
    {
        mLockBtn = b;
    }
}