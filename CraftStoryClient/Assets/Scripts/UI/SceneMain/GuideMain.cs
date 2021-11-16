using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideMain : MonoBehaviour
{
    void Start()
    {
        GuideLG.E.ReStart();

        // オブジェクトを生成
        WorldMng.E.CreateGameObjects();

        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);
        if(DataMng.E.RuntimeData.GuideId == 4)
        {
            UICtl.E.OpenUI<EquipUI>(UIType.Equip);
        }

        AudioMng.E.ShowBGM("bgm_01");
    }
}
