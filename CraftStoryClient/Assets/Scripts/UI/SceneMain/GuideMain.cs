using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideMain : MonoBehaviour
{
    void Start()
    {
        GuideLG.E.ReStart();

        // オブジェクトを生成
        WorldMng.E.CreateWorld();

        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);
        if (DataMng.E.RuntimeData.GuideId == 4)
        {
            UICtl.E.OpenUI<EquipUI>(UIType.Equip);
        }
        if (DataMng.E.RuntimeData.GuideId == 5)
        {
            HomeLG.E.UI.Battle.gameObject.SetActive(true);
            HomeLG.E.UI.Items.gameObject.SetActive(false);
        }
        if (DataMng.E.RuntimeData.GuideId == 6)
        {
            HomeLG.E.UI.Battle.gameObject.SetActive(true);
            HomeLG.E.UI.Items.gameObject.SetActive(false);
        }

        AudioMng.E.ShowBGM("bgm_01");
    }
}
