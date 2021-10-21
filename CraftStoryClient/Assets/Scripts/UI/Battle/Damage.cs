using UnityEngine;
using UnityEngine.UI;

public class Damage : UIBase
{
    Text Text { get => FindChiled<Text>("Text"); }

    float DestroyTime = 0;

    private void Update()
    {
        DestroyTime += Time.deltaTime;

        if (DestroyTime > SettingMng.DamageDestroyTime)
        {
            Destroy(gameObject);
        }
    }

    public void Set(string damage)
    {
        Text.text = damage;
    }
}