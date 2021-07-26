using System;
using UnityEngine;
using UnityEngine.UI;

public class TermsUI : UIBase
{
    Button Terms01Btn;
    Button Terms02Btn;
    Button AgreeBtn;
    Button DisagreeBtn;

    public override void Init()
    {
        base.Init();

        TermsLG.E.Init(this);

        Terms01Btn = FindChiled<Button>("Terms01Btn");
        Terms01Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms01UI>(UIType.Terms01); });

        Terms02Btn = FindChiled<Button>("Terms02Btn");
        Terms02Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms02UI>(UIType.Terms02); });

        AgreeBtn = FindChiled<Button>("AgreeBtn");
        AgreeBtn.onClick.AddListener(() => 
        {
            LoginLg.E.CreateNewAccount();
            Close();
        });

        DisagreeBtn = FindChiled<Button>("DisagreeBtn");
        DisagreeBtn.onClick.AddListener(() => { CommonFunction.QuitGame(); });
    }
}
