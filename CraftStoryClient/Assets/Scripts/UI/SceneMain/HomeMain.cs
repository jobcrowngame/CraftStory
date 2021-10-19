using UnityEngine;

public class HomeMain : MonoBehaviour
{
    void Start()
    {
        // �I�u�W�F�N�g�𐶐�
        WorldMng.E.CreateGameObjects();

        // ���ԃA�N�e�B�u�i���A�鍷���ւ��j
        WorldMng.E.GameTimeCtl.Active = true;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        // ���O�C����A�n�߂Ƀz�[���ɓ���ꍇ�A���m�点Window���o��
        if (NoticeLG.E.IsFirst)
        {
            UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
            NoticeLG.E.IsFirst = false;
        }

        AudioMng.E.ShowBGM("bgm_01");
    }
}
