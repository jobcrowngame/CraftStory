
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

        hpBar.maxValue = p.AllHP;
        hpBar.value = p.AllHP;

        Name.text = p.Name;
        Level.text = "Lv." + p.Level;

        RefreshHP();
    }

    public override void RefreshHpBar()
    {
        base.RefreshHpBar();

        hpBar.maxValue = p.AllHP;
        RefreshHP();
    }

    public override void OnValueChange(int v)
    {
        base.OnValueChange(v);

        RefreshHP();

        if (v < 0)
        {
            AddDamageObj(-v, DamageObjParent, "Prefabs/Battle/DamageWhit");
        }
        else
        {
            AddDamageObj(-v, DamageObjParent, "Prefabs/Battle/DamageGreen");
        }
    }

    public override void OnDide()
    {
        base.OnDide();

        hpBar.gameObject.SetActive(false);
    }

    private void RefreshHP()
    {
        hpBar.value = p.CurHP;
        var percent = (float)p.CurHP / p.AllHP;

        if (percent > 0 && percent < 0.01f)
            percent = 0.01f;

        if (HPText != null) HPText.text = (percent * 100).ToString("F0") + "%";
    }
}