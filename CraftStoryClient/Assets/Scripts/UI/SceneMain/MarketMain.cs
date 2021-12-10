using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketMain : MonoBehaviour
{
    void Start()
    {
        // �I�u�W�F�N�g�𐶐�
        WorldMng.E.CreateGameObjects();

        // ���ԃA�N�e�B�u�i���A�鍷���ւ��j
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        // �݌v�}�A�b�v���[�h�ē��̕\��
        NWMng.E.GetTotalUploadBlueprintCount((rp) =>
        {
            int count = int.Parse(rp.ToString());
            Image img = GameObject.Find("BluePrintHintImage").GetComponent<Image>();
            img.color = new Color(1f, 1f, 1f, count == 0 ? 1 : 1 / 256f);
        });

        AudioMng.E.ShowBGM("bgm_01");
    }
}
