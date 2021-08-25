using UnityEngine;

/// <summary>
/// エフェクトマネージャー
/// </summary>
public class EffectMng : Single<EffectMng>
{
    private GameObject destroyEffect;

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
    public void RemoveDestroyEffect()
    {
        if (destroyEffect != null)
        {
            Logger.Log("Destroy Effect");
            GameObject.Destroy(destroyEffect);
            destroyEffect = null;
        }
    }

    private string GetEffectResourcesPath(EffectType eType)
    {
        switch (eType)
        {
            case EffectType.BlockDestroy: return "Prefabs/Effect/effect_001";
            case EffectType.BlockDestroyEnd: return "Prefabs/Effect/BlockBreackEffect";
            case EffectType.ResourcesDestroy: return "Prefabs/Effect/ResourcesDeleteEffect";
            case EffectType.Gacha: return "Prefabs/Effect/TreasureBoxEffect";
            default: return "";
        }
    }
}
public enum EffectType
{
    BlockDestroy,
    BlockDestroyEnd,
    ResourcesDestroy,
    Gacha,
}
