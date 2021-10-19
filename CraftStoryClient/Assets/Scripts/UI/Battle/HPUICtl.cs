
using UnityEngine;
using UnityEngine.UI;

public class HPUICtl : UIBase
{
    Slider hpBar { get => FindChiled<Slider>("HPBar"); }
    Text hpText { get => FindChiled<Text>("Text"); }

    Transform DamageObjParent { get => FindChiled("DamageObjParent"); }

    int maxHP;
    int curHP;

    bool lockupCamera;

    void LateUpdate()
    {
        if (lockupCamera)
        {
            //　カメラと同じ向きに設定
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="maxHP">最大HP</param>
    public override void Init(object data)
    {
        maxHP = (int)data;
        curHP = maxHP;

        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        RefreshHP();
    }

    /// <summary>
    /// HP 数値が変更
    /// </summary>
    /// <param name="v"></param>
    public void ValueChange(int v)
    {
        curHP += v;

        if (curHP < 0)
        {
            curHP = 0;
        }

        RefreshHP();
    }

    /// <summary>
    /// HP 更新
    /// </summary>
    private void RefreshHP()
    {
        hpBar.value = curHP;
        hpText.text = curHP + "/" + maxHP;
    }

    /// <summary>
    /// HPバーを隠れる
    /// </summary>
    public void HideHpBar()
    {
        hpBar.gameObject.SetActive(false);
    }

    /// <summary>
    /// カメラを向かう
    /// </summary>
    /// <param name="b"></param>
    public void IsLockUpCamera(bool b)
    {
        lockupCamera = b;
    }

    /// <summary>
    /// ダメージObject
    /// </summary>
    /// <param name="damage"></param>
    public void AddDamageObj(float damage)
    {
        var obj = CommonFunction.Instantiate<Damage>("Prefabs/UI/Common/Damage", DamageObjParent, DamageObjParent.position);
        obj.Set(damage.ToString());
    }
}