using UnityEngine.UI;

/// <summary>
/// �{�^���O���[�v�̃T�u
/// </summary>
public class MyToggle : Toggle
{
    /// <summary>
    /// �C���f�b�N�X
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// �{�^���T�u�e
    /// </summary>
    public ToggleBtns toggleBtns { get; set; }

    protected override void Awake()
    {
        if (group != null)
        {
            toggleBtns = group.GetComponent<ToggleBtns>();

            onValueChanged.AddListener((b) => 
            {
                if (b)
                    toggleBtns.OnValueChange(Index);
            });
        }
    }
}
