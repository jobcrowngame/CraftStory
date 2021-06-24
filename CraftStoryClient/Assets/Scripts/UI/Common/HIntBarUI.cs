using UnityEngine;
using UnityEngine.UI;

public class HintBarUI : UIBase
{
    Text MsgText;
    Animation anim;

    private void Awake()
    {
        MsgText = FindChiled<Text>("Text");
        anim = GetComponent<Animation>();
    }

    public void SetMsg(string msg)
    {
        MsgText.text = msg;
        anim.Play();
    }

    public void Over()
    {
        Close();
    }
}
