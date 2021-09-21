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

    public override void Open()
    {
        base.Open();

        GachaBonusLG.E.OnOpen();
    }

    public void Set(ShopLG.GachaResponse result, int gachaId)
    {
        OkBtn.gameObject.SetActive(false);

        ClearCell(Parent);
        GachaBonusLG.E.cellList.Clear();

        foreach (var item in result.bonusList)
        {
            var cell = AddCell<GachaBonusCell>("Prefabs/UI/GachaBonusCell", Parent);
            if (cell != null)
            {
                var config = ConfigMng.E.Bonus[item.bonusId];

                // ���x���A�b�v���邩�̔��f
                var random = Random.Range(0, 100);

                cell.Init();
                cell.Add(ConfigMng.E.Item[config.Bonus1], config.BonusCount1, item.rare, random < 100);
                GachaBonusLG.E.AddCell(cell);
            }
        }

        index = result.index;
        this.gachaId = gachaId;

        StartCoroutine(IEStartAnim1());
    }

    public void ShowOkBtn()
    {
        OkBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// ��{�b�N�X������A�j���V����
    /// </summary>
    IEnumerator IEStartAnim1()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (var item in GachaBonusLG.E.cellList)
        {
            item.ShowAnim("GachaBonusCell1");
            yield return new WaitForSeconds(0.2f);
        }

        // �S���I�������A�����҂��܂��B
        yield return new WaitForSeconds(0.8f);

        // ���̃{�b�N�X���I�[�v�����܂��B
        GachaBonusLG.E.OpenNext();
    }
}