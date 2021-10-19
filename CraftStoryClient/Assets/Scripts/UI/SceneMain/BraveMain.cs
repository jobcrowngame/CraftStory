using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BraveMain : MonoBehaviour
{
    void Start()
    {
        // オブジェクトを生成
        WorldMng.E.CreateGameObjects();

        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        AudioMng.E.ShowBGM("bgm_02");
    }
}
