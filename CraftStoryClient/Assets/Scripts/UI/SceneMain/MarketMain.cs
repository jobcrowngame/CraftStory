using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketMain : MonoBehaviour
{
    void Start()
    {
        // �I�u�W�F�N�g�𐶐�
        WorldMng.E.CreateGameObjects();

        // ���ԃA�N�e�B�u�i���A�鍷���ւ��j
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        AudioMng.E.ShowBGM("bgm_01");
    }
}