
public class MapLG : UILogicBase<MapLG, MenuUI>
{
    public bool IsEquipTutorial()
    {
        return DataMng.E.RuntimeData.GuideEnd4 == 0 &&
            PlayerCtl.E.GetEquipByItemType(ItemType.Weapon) == null;
    }
}