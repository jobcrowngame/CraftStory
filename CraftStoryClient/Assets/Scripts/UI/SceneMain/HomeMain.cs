using UnityEngine;

public class HomeMain : MonoBehaviour
{
    void Start()
    {
        // オブジェクトを生成
        WorldMng.E.CreateGameObjects();

        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.Active = true;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        // ログイン後、始めにホームに入る場合、お知らせWindowを出す
        if (NoticeLG.E.IsFirst)
        {
            UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
            NoticeLG.E.IsFirst = false;
        }

        AudioMng.E.ShowBGM("bgm_01");
    }
}
