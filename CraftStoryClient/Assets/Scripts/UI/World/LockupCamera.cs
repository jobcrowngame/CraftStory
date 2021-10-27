
using UnityEngine;

public class LockupCamera : MonoBehaviour
{
    private void Update()
    {
        //　カメラと同じ向きに設定
        transform.rotation = Camera.main.transform.rotation;
    }
}