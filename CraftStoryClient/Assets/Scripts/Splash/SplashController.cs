using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    /// <summary>
    ///  変数定義
    /// </summary>

    private float LoadTime;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        LoadTime = 0.0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        // ◯秒後にタイトル画面に遷移させる
        LoadTime += Time.deltaTime;
        if(LoadTime >= time)
        {
            LoadSceneTitle();
        }
    }


    //  Titleシーンへ飛ばす
    private void LoadSceneTitle()
    {
        SceneManager.LoadScene("Title");
    }

}
