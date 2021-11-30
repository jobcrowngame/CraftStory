using UnityEngine;

public class HomeMain : MonoBehaviour
{
    void Start()
    {
        // ���ԃA�N�e�B�u�i���A�鍷���ւ��j
        WorldMng.E.GameTimeCtl.ResetTime();
        WorldMng.E.GameTimeCtl.Active = true;

        // �I�u�W�F�N�g�𐶐�
        WorldMng.E.CreateGameObjects();

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        // ���O�C����A�n�߂Ƀz�[���ɓ���ꍇ�A���m�点Window���o��
        if (NoticeLG.E.IsFirst)
        {
            NoticeLG.E.GetNoticeList();
        }

        AudioMng.E.ShowBGM("bgm_01");
    }
}
