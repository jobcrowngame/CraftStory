using JsonConfigData;

public class ItemEquipmentData : ItemData
{
    string attachSkills;

    public ItemEquipmentData() { }
    public ItemEquipmentData(ItemData itemdata)
    {
        id = itemdata.id;
        itemId = itemdata.itemId;
        islocked = itemdata.islocked;
        count = 1;
    }
    public ItemEquipmentData(EquipListLG.EquipListRP rp) : base(rp.itemId, 1)
    {
        id = rp.id;
        itemId = rp.itemId;
        islocked = rp.islocked;
        if (islocked == 1) attachSkills = rp.skills;
        count = 1;

        equipSite = DataMng.E.GetItemByGuid(rp.id).equipSite;
    }

    public Equipment equipmentConfig { get => ConfigMng.E.Equipment[Config().ReferenceID]; }

    public string[] AttachSkills { get => string.IsNullOrEmpty(attachSkills) ? null : attachSkills.Split(','); }

    public string AttachSkillsStr { get => attachSkills; }

    public void SetAttachSkills(string attachSkills)
    {
        this.attachSkills = attachSkills;
    }
}