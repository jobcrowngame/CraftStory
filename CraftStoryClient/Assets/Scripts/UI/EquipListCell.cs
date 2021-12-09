using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EquipListCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Name { get => FindChiled<Text>("Name"); }
    Text Rare { get => FindChiled<Text>("Rare"); }
    Button OnClickBtn { get => GetComponent<Button>(); }

    Transform Equiped { get => FindChiled("EquipTag"); }
    Transform SelectedMask { get => FindChiled("SelectedMask"); }

    Transform Skills { get => FindChiled("Skills"); }
    EquipSkillCell[] cells;

    public ItemEquipmentData Data { get; private set; }

    private bool IsSelected { get => SelectedMask.gameObject.activeSelf == true; }
    public bool IsEquiped { get => Data.equipSite > 0; }

    private void Awake()
    {
        OnClickBtn.onClick.AddListener(() =>
        {
            if (IsSelected)
                return;

            if (EquipListLG.E.SelectedItem != null)
                EquipListLG.E.SelectedItem.Selected(false);

            EquipListLG.E.SelectedItem = this;
            EquipListLG.E.UI.RefreshParameter();

            Selected();

            if (DataMng.E.MapData.Config.MapType == (int)MapType.Guide)
            {
                GuideLG.E.Next();
            }
        });

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
    }

    public void Set(ItemEquipmentData data)
    {
        this.Data = data;

        Icon.sprite = ReadResources<Sprite>(data.Config.IconResourcesPath);

        Name.text = data.Config.Name;

        string rareText = "";
        for (int i = 0; i < data.equipmentConfig.RareLevel; i++)
        {
            rareText += "★";
        }
        Rare.text = rareText;

        var attachSkills = data.AttachSkills;
        for (int i = 0; i < cells.Length; i++)
        {
            if (attachSkills != null && attachSkills.Length > i)
            {
                cells[i].Set(int.Parse(attachSkills[i]));
            }
            else
            {
                cells[i].Set(data.islocked == 1 ? -1 : -2);
            }
        }

        Selected(false);
        ShowEquipedTag(data.equipSite > 0);
    }

    /// <summary>
    /// 装備マック
    /// </summary>
    /// <param name="b"></param>
    private void ShowEquipedTag(bool b = true)
    {
        Equiped.gameObject.SetActive(b);
    }

    /// <summary>
    /// 選択マック
    /// </summary>
    /// <param name="b"></param>
    public void Selected(bool b = true)
    {
        SelectedMask.gameObject.SetActive(b);
    }

    public void AppraisalEquipment(string skills)
    {
        Data.islocked = 1;
        Data.SetAttachSkills(skills);

        StartCoroutine(AppraisalEquipmentIE());
    }
    IEnumerator AppraisalEquipmentIE()
    {
        var attachSkills = Data.AttachSkills;
        for (int i = 0; i < cells.Length; i++)
        {
            if (attachSkills != null && attachSkills.Length > i)
            {
                cells[i].ShowNewAddAnimation(int.Parse(attachSkills[i]));
            }
            else
            {
                cells[i].ShowNewAddAnimation(-1);
            }

            yield return new WaitForSeconds(1f);
        }

        EquipListLG.E.UI.RefreshParameter();
    }
}