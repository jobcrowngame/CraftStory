using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class HpUIPlayer : HpUIBase
{
    Transform DamageObjParent { get => FindChiled("DamageObjParent"); }
    Text Behavior { get => FindChiled<Text>("Behavior"); }

    Stack<int> addedExpStack;

    float expAddStep = 0.2f;
    float curExpAddTimer = 0;

    public override void Init<T>(T data)
    {
        base.Init<T>(data);

        addedExpStack = new Stack<int>();

        RefreshHP();
    }

    private void Update()
    {
        curExpAddTimer += Time.deltaTime;

        if (curExpAddTimer > expAddStep)
        {
            curExpAddTimer = 0;
            AddExpInstance();
        }
    }

    public override void RefreshHpBar()
    {
        base.RefreshHpBar();
        RefreshHP();
    }

    public override void OnValueChange(int v)
    {
        base.OnValueChange(v);

        RefreshHP();

        if (v < 0)
        {
            AddDamageObj(-v, DamageObjParent, "Prefabs/Battle/DamageRed");
        }
        else
        {
            AddDamageObj(v, DamageObjParent, "Prefabs/Battle/DamageGreen");
        }
    }

    public override void OnResurrection()
    {
        base.OnResurrection();

        if (HomeLG.E.UI != null) HomeLG.E.UI.OnHpChange(1);
    }

    private void RefreshHP()
    {
        var percent = (float)p.CurHP / p.AllHP;

        if (percent > 0 && percent < 0.01f)
            percent = 0.01f;

        if (HomeLG.E.UI != null) HomeLG.E.UI.OnHpChange(percent);
    }

    /// <summary>
    /// Exp追加
    /// </summary>
    /// <param name="exp"></param>
    public override void AddExp(int exp)
    {
        addedExpStack.Push(exp);
    }
    private void AddExpInstance()
    {
        if (addedExpStack.Count == 0)
            return;

        int addExp = addedExpStack.Pop();
        var obj = CommonFunction.Instantiate<ExpAdd>("Prefabs/Battle/ExpAdd", transform, transform.position);
        obj.Set(addExp);
    }
}
