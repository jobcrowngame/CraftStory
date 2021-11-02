using UnityEngine;
using UnityEngine.UI;

public class HpUIPlayer : HpUIBase
{
    Transform DamageObjParent { get => FindChiled("DamageObjParent"); }

    public override void Init<T>(T data)
    {
        base.Init<T>(data);

        RefreshHP();
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
            AddDamageObj(-v, DamageObjParent, "Prefabs/Battle/DamageGreen");
        }
    }

    public override void OnResurrection()
    {
        base.OnResurrection();

        if (HomeLG.E.UI != null) HomeLG.E.UI.OnHpChange(1);
    }

    private void RefreshHP()
    {
        float percent = (float)p.CurHP / p.MaxHP;
        if (HomeLG.E.UI != null) HomeLG.E.UI.OnHpChange(percent);
    }
}
