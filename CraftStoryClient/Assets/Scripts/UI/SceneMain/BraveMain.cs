using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BraveMain : MonoBehaviour
{
    void Start()
    {
        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.Active = false;

        // オブジェクトを生成
        WorldMng.E.CreateGameObjects();

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        AudioMng.E.ShowBGM("bgm_02");

        // 冒険エリア最大レベル統計
        if (DataMng.E.RuntimeData.MapType == MapType.Brave)
            NWMng.E.ArriveFloor(null, DataMng.E.MapData.Config.Floor);
    }
}
