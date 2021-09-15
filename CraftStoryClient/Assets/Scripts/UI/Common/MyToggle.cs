using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �{�^���O���[�v�̃T�u
/// </summary>
public class MyToggle : Toggle
{
    Image On { get => CommonFunction.FindChiledByName<Image>(transform, "Checkmark"); }
    Image Off { get => CommonFunction.FindChiledByName<Image>(transform, "Background"); }

    /// <summary>
    /// �C���f�b�N�X
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// �{�^���T�u�e
    /// </summary>
    public MyToggleGroupCtl toggleBtns { get; set; }

    protected override void Awake()
    {
        if (group != null)
        {
            toggleBtns = group.GetComponent<MyToggleGroupCtl>();

            onValueChanged.AddListener((b) => 
            {
                if (b)
                    toggleBtns.OnValueChange(Index);
            });
        }
    }

    /// <summary>
    /// �C���A�N�e�B�u
    /// </summary>
    public void Enable()
    {
        interactable = false;
        CommonFunction.FindChiledByName<Image>(transform, "Background").color = Color.grey;
    }

    /// <summary>
    /// �{�^���摜�ݒ�
    /// </summary>
    /// <param name="on"></param>
    /// <param name="off"></param>
    public void SetToggleImage(string on, string off)
    {
        On.sprite = ResourcesMng.E.ReadResources<Sprite>(on);
        Off.sprite = ResourcesMng.E.ReadResources<Sprite>(off);
    }
}
