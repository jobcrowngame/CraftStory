using UnityEngine;

/// <summary>
/// エフェクトマネージャー
/// </summary>
public class EffectMng : Single<EffectMng>
{
    private GameObject destroyEffect;

    const string BattEffectRoot = "Prefabs/Effect/Battle/";

    public void AddDestroyEffect(Vector3 pos)
    {
        Logger.Log("Add Effect");

        if (destroyEffect != null)
            RemoveDestroyEffect();

        destroyEffect = CommonFunction.Instantiate(GetEffectResourcesPath(EffectType.BlockDestroy), WorldMng.E.MapCtl.EffectParent, pos);
    }
    public T AddEffect<T>(Vector3 pos, EffectType eType) where T : Component
    {
        Logger.Log("Add Effect");

        var effect = CommonFunction.Instantiate<T>(GetEffectResourcesPath(eType), WorldMng.E.MapCtl.EffectParent, pos);
        return effect;
    }
    public T AddUIEffect<T>(Transform parent, Vector3 pos, EffectType eType) where T : Component
    {
        Logger.Log("Add Effect");

        var effect = CommonFunction.Instantiate<T>(GetEffectResourcesPath(eType), parent, pos);
        return effect;
    }
    public T AddEffect<T>(Vector3 pos, string path) where T : Component
    {
        Logger.Log("Add Effect");

        var effect = CommonFunction.Instantiate<T>(path, WorldMng.E.MapCtl.EffectParent, pos);
        return effect;
    }
    public void RemoveDestroyEffect()
    {
        if (destroyEffect != null)
        {
            Logger.Log("Destroy Effect");
            GameObject.Destroy(destroyEffect);
            destroyEffect = null;
        }
    }

    /// <summary>
    /// Effectを追加
    /// </summary>
    /// <param name="path"></param>
    /// <param name="time"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public EffectBase AddBattleEffect(string path, float time, Transform target)
    {
        return AddBattleEffectHaveParent(path, time, WorldMng.E.EffectParent, target.position, target.rotation);
    }
    public EffectBase AddBattleEffect(string path, float time, Vector3 pos, Quaternion rotation)
    {
        return AddBattleEffectHaveParent(path, time, WorldMng.E.EffectParent, pos, rotation);
    }
    public EffectBase AddBattleEffectHaveParent(string path, float time, Transform parent)
    {
        return AddBattleEffectHaveParent(path, time, parent, parent.position, parent.rotation);
    }
    public EffectBase AddBattleEffectHaveParent(string path, float time, Transform parent, Vector3 pos,  Quaternion rotation)
    {
        var effect = CommonFunction.Instantiate<EffectBase>(BattEffectRoot + path, parent, pos);
        effect.transform.rotation = rotation;
        effect.Init(time);
        return effect;
    }

    private string GetEffectResourcesPath(EffectType eType)
    {
        switch (eType)
        {
            case EffectType.BlockDestroy: return "Prefabs/Effect/effect_001";
            case EffectType.BlockDestroyEnd: return "Prefabs/Effect/BlockBreackEffect";
            case EffectType.ResourcesDestroy: return "Prefabs/Effect/ResourcesDeleteEffect";
            case EffectType.AppraisalEquipment: return "Prefabs/Effect/ResourcesDeleteEffect";
            case EffectType.Gacha: return "Prefabs/Effect/TreasureBoxEffect";
            case EffectType.FairyCreate: return "Prefabs/Effect/effect_2d_011";
            case EffectType.TreasureBox: return "Prefabs/Effect/TreasureBox";
            default: return "";
        }
    }
}
public enum EffectType
{
    BlockDestroy,
    BlockDestroyEnd,
    ResourcesDestroy,
    AppraisalEquipment,
    Gacha,
    FairyCreate,
    TreasureBox,
}
