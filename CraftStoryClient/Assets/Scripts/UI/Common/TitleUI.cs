using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ʃ^�C�g��
/// </summary>
public class TitleUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Text Title { get => FindChiled<Text>("TitleText"); }

    Action action;

    private void Awake()
    {
        CloseBtn.onClick.AddListener(()=> 
        {
            if (action != null)
                action();
        });
    }

    /// <summary>
    /// �^�C�g�������Z�b�g
    /// </summary>
    /// <param name="title">�^�C�g��</param>
    public void SetTitle(string title)
    {
        Title.text = title;
    }

    /// <summary>
    /// Close�{�^�����N���b�N����ꍇ�̃C�x���g
    /// </summary>
    /// <param name="action">�C�x���g</param>
    public void SetOnClose(Action action)
    {
        this.action = action;
    }
}
