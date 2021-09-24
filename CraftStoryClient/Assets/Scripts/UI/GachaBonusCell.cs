using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class GachaBonusCell : UIBase
{
    Transform Item { get => FindChiled("Item"); }
    Text Count { get => FindChiled<Text>("Count"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Image TreasureBoxIcon { get => FindChiled<Image>("TreasureBoxIcon"); }
    Image TreasureBoxIconOpen { get => FindChiled<Image>("TreasureBoxIconOpen"); }
    Animation anim { get => GetComponent<Animation>(); }

    private int rare;
    int LvUpCount;

    public bool IsLvUp { get => LvUpCount > 0; }

    public void Add(Item item, int count, int rare, bool lvUp)
    {
        Count.text = "x" + count;
        Icon.sprite = ReadResources<Sprite>(item.IconResourcesPath);

        if (lvUp)
        {
            LvUpCount = rare - 1;
            this.rare = 1;
        }
        else
        {
            LvUpCount = 0;
            this.rare = rare;
        }

        ImageChange(this.rare);
    }

    public void ShowAnim(string animName)
    {
        anim.Play(animName);
    }
    public void ShowItem()
    {
        Item.gameObject.SetActive(true);
    }
    public void ImageChange(int rare)
    {
        TreasureBoxIcon.sprite = ReadResources<Sprite>("Textures/treasurebox" + rare);
        TreasureBoxIconOpen.sprite = ReadResources<Sprite>("Textures/treasurebox_open" + rare);
    }

    /// <summary>
    /// レベルアップアニメション開始
    /// </summary>
    public void StartLvUpAnim()
    {
        StartCoroutine(StartLvUpAnimIE());
    }
    IEnumerator StartLvUpAnimIE()
    {
        while (LvUpCount > 0)
        {
            var effect = EffectMng.E.AddUIEffect<EffectBase>(transform, transform.position, EffectType.Gacha);
            effect.Init(0.5f);

            AudioMng.E.ShowSE("jumpupSE");

            yield return new WaitForSeconds(0.5f);

            rare++;
            LvUpCount--;
            ImageChange(rare);

            yield return new WaitForSeconds(0.5f);
        }

        StartOpenAnim();
    }

    /// <summary>
    /// ボックスを開けるアニメション
    /// </summary>
    public void StartOpenAnim()
    {
        StartCoroutine(StartOpenAnimIE());
    }
    IEnumerator StartOpenAnimIE()
    {
        ShowAnim("GachaBonusCell2");
        yield return new WaitForSeconds(0.2f);

        var effect = EffectMng.E.AddUIEffect<EffectBase>(transform, transform.position, EffectType.Gacha);
        effect.Init(0.5f);

        AudioMng.E.ShowSE("TreasureOpenSE");

        yield return new WaitForSeconds(0.3f);
        ShowItem();

        if (rare == 3)
        {
            GachaBonusLG.E.UI.CutInImage();
        }
        else
        {
            GachaBonusLG.E.OpenNext();
        }
    }
}