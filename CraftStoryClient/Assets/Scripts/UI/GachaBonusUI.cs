using JsonConfigData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaBonusUI : UIBase
{
    Transform Parent { get => FindChiled("Content"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
   

    private int gachaId;
    private int index;
    List<GachaBonusCell> cellList = new List<GachaBonusCell>();

    public override void Init()
    {
        base.Init();
        GachaBonusLG.E.Init(this);

        OkBtn.onClick.AddListener(()=> 
        {
            var ui = UICtl.E.OpenUI<GachaAddBonusUI>(UIType.GachaAddBonus);
            ui.Set(index, gachaId);
            Close();
        });
    }
    public void Set(ShopLG.GachaResponse result, int gachaId)
    {
        OkBtn.gameObject.SetActive(false);

        ClearCell(Parent);
        cellList.Clear();

        foreach (var item in result.bonusList)
        {
            var cell = AddCell<GachaBonusCell>("Prefabs/UI/GachaBonusCell", Parent);
            if (cell != null)
            {
                var config = ConfigMng.E.Bonus[item.bonusId];

                cell.Init();
                cell.Add(ConfigMng.E.Item[config.Bonus1], config.BonusCount1, item.rare);
                cellList.Add(cell);
            }
        }

        index = result.index;
        this.gachaId = gachaId;

        StartAnim();
    }

    private void StartAnim()
    {
        StartCoroutine(IEStartAnim1());
        StartCoroutine(IEStartAnim2());
    }
    IEnumerator IEStartAnim1()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (var item in cellList)
        {
            item.ShowAnim("GachaBonusCell1");
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator IEStartAnim2()
    {
        yield return new WaitForSeconds(3.4f);

        foreach (var item in cellList)
        {
            item.ShowAnim("GachaBonusCell2");
            yield return new WaitForSeconds(0.2f);

            var effect = EffectMng.E.AddUIEffect<EffectBase>(transform, item.transform.position, EffectType.Gacha);
            effect.Init(0.5f);

            yield return new WaitForSeconds(0.3f);
            item.ShowItem();
        }

        OkBtn.gameObject.SetActive(true);
    }
}