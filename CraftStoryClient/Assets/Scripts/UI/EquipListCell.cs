using UnityEngine;
using UnityEngine.UI;

public class EquipListCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text HP { get => FindChiled<Text>("HP"); }
    Text Damage { get => FindChiled<Text>("Damage"); }
    Text Defence { get => FindChiled<Text>("Defence"); }

    Transform Skills { get => FindChiled("Skills"); }
    EquipSkillCell[] cells;

    Button EquipBtn { get => FindChiled<Button>("EquipBtn"); }
    Button AppraisalBtn { get => FindChiled<Button>("AppraisalBtn"); }

    ItemEquipmentData data;

    private void Awake()
    {
        cells = new EquipSkillCell[Skills.childCount];
        for (int i = 0; i < Skills.childCount; i++)
        {
            var cell = Skills.GetChild(i).GetComponent<EquipSkillCell>();
            if (cell == null)
            {
                cell = Skills.GetChild(i).gameObject.AddComponent<EquipSkillCell>();
            }
            cells[i] = cell;
        }

        EquipBtn.onClick.AddListener(() =>
        {
            PlayerCtl.E.EquipEquipment(data);
            EquipListLG.E.UI.Close();
        });
        AppraisalBtn.onClick.AddListener(() =>
        {
            EquipListLG.E.AppraisalEquipment(data, this);
        });
    }

    public void Set(ItemEquipmentData data)
    {
        this.data = data;

        Icon.sprite = ReadResources<Sprite>(data.Config().IconResourcesPath);

        HP.text = string.Format("HP：{0}", data.equipmentConfig.HP);
        if (data.equipmentConfig.HP < 0) HP.gameObject.SetActive(false);

        Damage.text = string.Format("攻撃力：{0}", data.equipmentConfig.Damage);
        if (data.equipmentConfig.Damage < 0) Damage.gameObject.SetActive(false);

        Defence.text = string.Format("防御力：{0}", data.equipmentConfig.Defnse);
        if (data.equipmentConfig.Defnse < 0) Defence.gameObject.SetActive(false);

        RefreshUI();
    }

    public void AppraisalEquipment(string skills)
    {
        data.islocked = 1;
        data.SetAttachSkills(skills);

        RefreshUI();
    }

    private void RefreshUI()
    {
        var attachSkills = data.AttachSkills;
        for (int i = 0; i < cells.Length; i++)
        {
            if (attachSkills != null && attachSkills.Length > i)
            {
                cells[i].Set(int.Parse(attachSkills[i]));
            }
            else
            {
                cells[i].Set(-1);
            }
        }

        EquipBtn.gameObject.SetActive(data.IsLocked);
        AppraisalBtn.gameObject.SetActive(!data.IsLocked);

        //　装備してるEquipment
        var equipedEquipment = PlayerCtl.E.GetEquipByItemType((ItemType)data.Config().Type);
        if (equipedEquipment == null)
        {
            EneableEquipBtn(true);
        }
        else
        {
            EneableEquipBtn(PlayerCtl.E.GetEquipByItemType((ItemType)data.Config().Type).id != data.id);
        }
    }

    private void EneableEquipBtn(bool b)
    {
        EquipBtn.enabled = b;
        EquipBtn.GetComponent<Image>().color = b ? Color.white : Color.grey;
    }
}