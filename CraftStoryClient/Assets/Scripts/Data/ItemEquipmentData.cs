using JsonConfigData;

public class ItemEquipmentData
{
    public ItemEquipmentData(ItemData itemdata)
    {
        itemData = itemdata;
    }

    ItemData itemData;

    public int id { get => itemData.id; set => itemData.id = value; }
    public int itemId { get => itemData.itemId; set => itemData.itemId = value; }
    public int islocked { get => itemData.islocked; set => itemData.islocked = value; }
    public bool IsLocked { get => itemData.IsLocked; }
    public int equipSite { get => itemData.equipSite; set => itemData.equipSite = value; }

    public Item Config { get => itemData.Config; }
    public Equipment equipmentConfig { get => ConfigMng.E.Equipment[itemData.Config.ReferenceID]; }

    public string[] AttachSkills { get => string.IsNullOrEmpty(itemData.skills) ? null : itemData.skills.Split(','); }

    public string AttachSkillsStr { get => itemData.skills; }

    public void SetAttachSkills(string attachSkills)
    {
        itemData.skills = attachSkills;
    }
}