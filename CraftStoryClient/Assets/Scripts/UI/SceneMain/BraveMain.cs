using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BraveMain : MonoBehaviour
{
    void Start()
    {
        // ���ԃA�N�e�B�u�i���A�鍷���ւ��j
        WorldMng.E.GameTimeCtl.Active = false;

        // �I�u�W�F�N�g�𐶐�
        WorldMng.E.CreateGameObjects();

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        AudioMng.E.ShowBGM("bgm_02");
    }
}
