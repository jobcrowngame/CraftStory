using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BraveCell : UIBase
{
    public Text text { get => FindChiled<Text>("Text"); }
    public Image icon { get => FindChiled<Image>("Image"); }

    float timer = 0;
    float deleteTime = 5;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > deleteTime)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void Set(BraveLG.BraveCellItem item)
    {
        var config = ConfigMng.E.Item[item.itemId];
        text.text = config.Name + "X" + item.count;
        icon.sprite = ReadResources<Sprite>(config.IconResourcesPath);
    }
}
