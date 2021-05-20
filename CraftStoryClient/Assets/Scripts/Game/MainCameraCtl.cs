using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MainCameraCtl : MonoBehaviour
{
    // ↓ 「カメラからのレイ」を画面中央の平面座標から飛ばす
    Ray ray;
    // ↓ 当たったオブジェクト情報を格納する変数
    RaycastHit hit;
    // ブロックを設置する位置を一応リアルタイムで格納
    private Vector3 pos;
    // カメラ画面の真ん中
    Vector2 displayCenter;

    private Vector3 cameraOffset = new Vector3(0, 0.7f, 0);

    private void Start()
    {
        displayCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void Update()
    {
        // レザーを画面真ん中にショット
        ray = Camera.main.ScreenPointToRay(displayCenter);

        if (Physics.Raycast(ray, out hit))
        {
            // ↓ 生成位置の変数の値を「ブロックの向き + ブロックの位置」
            pos = hit.normal + hit.collider.transform.position;
        }
    }

    public Vector3 HitPos
    {
        get { return pos; }
    }
    public GameObject HitObj
    {
        get
        {
            if (hit.collider != null)
                return hit.collider.gameObject;

            return null;
        }
    }
    public void SetParent(Transform paretn)
    {
        transform.SetParent(paretn);
        transform.localPosition = cameraOffset;
        transform.localRotation = Quaternion.identity;
    }
}