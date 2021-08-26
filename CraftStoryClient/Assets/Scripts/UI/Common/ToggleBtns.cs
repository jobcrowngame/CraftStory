using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// �{�^���O���[�v
/// </summary>
public class ToggleBtns : UIBase
{
    /// <summary>
    /// �T�u���X�g
    /// </summary>
    MyToggle[] cells;

    /// <summary>
    /// �N���b�N�C�x���g
    /// </summary>
    UnityAction<int> callback;

    /// <summary>
    /// ������
    /// </summary>
    public override void Init()
    {
        base.Init();

        cells = new MyToggle[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            cells[i] = transform.GetChild(i).GetComponent<MyToggle>();
            cells[i].Index = i;
        }
        cells[0].isOn = true;
    }

    /// <summary>
    /// �I�����ύX�����ꍇ�̃C�x���g
    /// </summary>
    /// <param name="index"></param>
    public void OnValueChange(int index)
    {
        if (callback != null)
        {
            callback(index);
        }
    }

    /// <summary>
    /// �I�����ύX�����C�x���g�̐ݒ�
    /// </summary>
    /// <param name="callback">�C�x���g</param>
    public void OnValueChangeAddListener(UnityAction<int> callback)
    {
        this.callback = callback;
    }

    public void SetBtnText(int index, string msg)
    {
        if (cells != null && cells.Length > index)
        {
            CommonFunction.FindChiledByName<Text>(cells[index].transform, "Text").text = msg;
        }
    }
    public void SetValue(int index)
    {
        if (cells.Length > index)
        {
            cells[index].isOn = true;
        }
    }
}
