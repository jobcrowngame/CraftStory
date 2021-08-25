using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �q���g�o�[UI
/// </summary>
public class HintBarUI : UIBase
{
    Text MsgText;
    Animation anim;

    private void Awake()
    {
        MsgText = FindChiled<Text>("Text");
        anim = GetComponent<Animation>();
    }

    /// <summary>
    /// ���b�Z�[�W���Z�b�g
    /// </summary>
    /// <param name="msg"></param>
    public void SetMsg(string msg)
    {
        MsgText.text = msg;
        anim.Play();
    }

    public void Over()
    {
        Close();
        GameObject.Destroy(gameObject);
    }
}
