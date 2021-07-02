using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EffectMng : Single<EffectMng>
{
    private GameObject destroyEffect;

    public void AddDestroyEffect(Vector3 pos)
    {
        Logger.Log("Add Effect");

        if (destroyEffect != null)
            RemoveDestroyEffect();

        destroyEffect = CommonFunction.Instantiate("Prefabs/Effect/effect_001", WorldMng.E.MapCtl.EffectParent, pos);
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
}
