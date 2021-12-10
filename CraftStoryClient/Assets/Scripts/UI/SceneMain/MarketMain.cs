using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketMain : MonoBehaviour
{
    void Start()
    {
        // オブジェクトを生成
        WorldMng.E.CreateGameObjects();

        // 時間アクティブ（昼、夜差し替え）
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.OpenUI<HomeUI>(UIType.Home);

        // 設計図アップロード案内の表示
        NWMng.E.GetTotalUploadBlueprintCount((rp) =>
        {
            int count = int.Parse(rp.ToString());
            Image img = GameObject.Find("BluePrintHintImage").GetComponent<Image>();
            img.color = new Color(1f, 1f, 1f, count == 0 ? 1 : 1 / 256f);
        });

        AudioMng.E.ShowBGM("bgm_01");
    }
}
