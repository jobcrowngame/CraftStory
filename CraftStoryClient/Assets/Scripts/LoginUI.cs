using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public InputField id;
    public InputField pw;
    public Button newAcc;
    public Button login;

    // Start is called before the first frame update
    void Start()
    {
        LoginLg.E.Init(this);

        newAcc.onClick.AddListener(() => { LoginLg.E.CreateNewAccount(); });
        login.onClick.AddListener(() => { LoginLg.E.Login(id.text, pw.text); });
    }

    public void CreateNewAccountBtnResponse(string id, string pw)
    {
        Debug.Log("�V�����A�J�E���g�쐬�������܂����B");
        Debug.LogFormat("ID:{0}, PW:{1}", id, pw);

        this.id.text = id;
        this.pw.text = pw;
    }

    public void LoginBtnResponse(string token)
    {
        Debug.Log("���O�C���������܂����B");
        Debug.Log("token:" + token);
    }
}
