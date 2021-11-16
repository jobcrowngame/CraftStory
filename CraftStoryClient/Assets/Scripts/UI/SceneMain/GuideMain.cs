using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideMain : MonoBehaviour
{
    void Start()
    {
        GuideLG.E.ReStart();

        // �I�u�W�F�N�g�𐶐�
        WorldMng.E.CreateGameObjects();

        // ���ԃA�N�e�B�u�i���A�鍷���ւ��j
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);
        if(DataMng.E.RuntimeData.GuideId == 4)
        {
            UICtl.E.OpenUI<EquipUI>(UIType.Equip);
        }

        AudioMng.E.ShowBGM("bgm_01");
    }
}
