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

    public override void OnValueChange(int v)
    {
        base.OnValueChange(v);

        AddDamageObj(-v);

        RefreshHP();
    }

    private void RefreshHP()
    {
        float percent = (float)curHP / p.MaxHP;
        if (HomeLG.E.UI != null) HomeLG.E.UI.OnHpChange(percent);
    }

    /// <summary>
    /// ダメージObject
    /// </summary>
    /// <param name="damage"></param>
    private void AddDamageObj(float damage)
    {
        var obj = CommonFunction.Instantiate<Damage>("Prefabs/Battle/DamageRed", DamageObjParent, DamageObjParent.position);
        obj.Set(damage.ToString());
    }
}
