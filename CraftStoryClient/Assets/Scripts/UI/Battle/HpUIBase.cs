
using UnityEngine;
using UnityEngine.UI;

public class HpUIBase : UIBase
{
    protected Parameter p;
    protected int curHP;
    protected bool lockupCamera;

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
    public override void Init<T>(T data)
    {
        p = data as Parameter;
        curHP = p.MaxHP;
    }

    /// <summary>
    /// HP 数値が変更
    /// </summary>
    /// <param name="v"></param>
    public virtual void OnValueChange(int v)
    {
        curHP += v;

        if (curHP < 0)
            curHP = 0;
    }

    /// <summary>
    /// 死んだ場合
    /// </summary>
    public virtual void OnDide() { }

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
    protected void AddDamageObj(int damage, Transform parent, string path)
    {
        var obj = CommonFunction.Instantiate<Damage>(path, parent, parent.position);
        obj.Set(damage.ToString());
    }

    /// <summary>
    /// ダメージObject
    /// </summary>
    /// <param name="damage"></param>
    protected void AddDamageRedObj(int damage, Transform parent)
    {
        var obj = CommonFunction.Instantiate<Damage>("Prefabs/Battle/DamageWhit", parent, parent.position);
        obj.Set(damage.ToString());
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="damage"></param>
    protected void AddRecoveryObj(int damage, Transform parent)
    {
        var obj = CommonFunction.Instantiate<Damage>("Prefabs/Battle/DamageGreen", parent, parent.position);
        obj.Set(damage.ToString());
    }
}