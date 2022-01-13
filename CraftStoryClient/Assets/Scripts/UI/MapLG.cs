
public class MapLG : UILogicBase<MapLG, MapUI>
{
    private bool mLockBtn;
    public bool BtnIsLocked { get => mLockBtn; }

    /// <summary>
    /// 連打を制限する為、ボタンをロック
    /// </summary>
    /// <param name="b"></param>
    public void LockBtn(bool b = true)
    {
        mLockBtn = b;
    }
}