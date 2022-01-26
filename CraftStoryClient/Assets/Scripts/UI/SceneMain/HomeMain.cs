using UnityEngine;

public class HomeMain : MonoBehaviour
{
    void Start()
    {
        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.Active = true;

        // オブジェクトを生成
        WorldMng.E.CreateWorld();

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        AudioMng.E.ShowBGM("bgm_01");
    }
}
