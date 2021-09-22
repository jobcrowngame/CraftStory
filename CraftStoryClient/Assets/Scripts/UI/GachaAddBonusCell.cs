using System.Collections;
using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class GachaAddBonusCell : UIBase
{
    Transform Item { get => FindChiled("Item"); }
    Text Count { get => FindChiled<Text>("Count"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Button onClick { get => GetComponent<Button>(); }
    Animation anim { get => GetComponent<Animation>(); }

    public void Add(Item item, int count)
    {
        Count.text = "x" + count;
        Icon.sprite = ReadResources<Sprite>(item.IconResourcesPath);

        onClick.onClick.AddListener(() => 
        {
            if (GachaAddBonusLG.E.IsFlipping)
                return;

            anim.Play();
            GachaAddBonusLG.E.CardFlipping();

            AudioMng.E.ShowSE("");

            StartCoroutine(AnimOver());
        });
    }

    private IEnumerator AnimOver()
    {
        yield return new WaitForSeconds(1);

        GachaAddBonusLG.E.UI.ShowOkBtn();
    }
}