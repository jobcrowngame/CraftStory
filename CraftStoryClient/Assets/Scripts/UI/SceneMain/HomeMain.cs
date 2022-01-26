using UnityEngine;

public class HomeMain : MonoBehaviour
{
    void Start()
    {
        // ���ԃA�N�e�B�u�i���A�鍷���ւ��j
        WorldMng.E.GameTimeCtl.Active = true;

        // �I�u�W�F�N�g�𐶐�
        WorldMng.E.CreateWorld();

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        AudioMng.E.ShowBGM("bgm_01");
    }
}
