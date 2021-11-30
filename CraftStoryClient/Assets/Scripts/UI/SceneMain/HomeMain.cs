using UnityEngine;

public class HomeMain : MonoBehaviour
{
    void Start()
    {
        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.ResetTime();
        WorldMng.E.GameTimeCtl.Active = true;

        // オブジェクトを生成
        WorldMng.E.CreateGameObjects();

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        // ログイン後、始めにホームに入る場合、お知らせWindowを出す
        if (NoticeLG.E.IsFirst)
        {
            NoticeLG.E.GetNoticeList();
        }

        AudioMng.E.ShowBGM("bgm_01");
    }
}
