
using UnityEngine;
using UnityEngine.UI;

public class HpUIMonster : HpUIBase
{
    Slider hpBar { get => FindChiled<Slider>("HPBar"); }
    Text Name { get => FindChiled<Text>("Name"); }
    Text Level { get => FindChiled<Text>("Level"); }
    Text HPText { get => FindChiled<Text>("HPText"); }
    Transform DamageObjParent { get => FindChiled("DamageObjParent"); }

    public override void Init<T>(T data)
    {
        base.Init<T>(data);

        hpBar.maxValue = p.MaxHP;
        hpBar.value = p.MaxHP;

        Name.text = p.Name;
        Level.text = "Lv." + p.Level;

        RefreshHP();
    }

    public override void OnValueChange(int v)
    {
        base.OnValueChange(v);

        RefreshHP();

        AddDamageObj(-v);
    }

    public override void OnDide()
    {
        base.OnDide();

        hpBar.gameObject.SetActive(false);
    }

    private void RefreshHP()
    {
        hpBar.value = curHP;
        if (HPText != null) HPText.text = ((curHP / p.MaxHP) * 100) + "%";
    }

    /// <summary>
    /// ダメージObject
    /// </summary>
    /// <param name="damage"></param>
    private void AddDamageObj(float damage)
    {
        var obj = CommonFunction.Instantiate<Damage>("Prefabs/Battle/DamageWhit", DamageObjParent, DamageObjParent.position);
        obj.Set(damage.ToString());
    }
}