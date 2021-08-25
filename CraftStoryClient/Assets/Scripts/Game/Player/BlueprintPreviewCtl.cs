using UnityEngine;

/// <summary>
/// 設計図プレイビューコンソール
/// </summary>
public class BlueprintPreviewCtl : MonoBehaviour
{
    Transform Parent { get => CommonFunction.FindChiledByName(transform, "Parent").transform; } // エンティティ親
    Transform BlueprintCamera { get => CommonFunction.FindChiledByName(transform, "Camera").transform; } // プレビュー用カメラ
    Transform cameraRotateX { get => CommonFunction.FindChiledByName(transform, "X").transform; } // カメラ角度X
    Transform cameraRotateY { get => CommonFunction.FindChiledByName(transform, "Y").transform; } // カメラ角度Y

    /// <summary>
    /// カメラ座標Z
    /// </summary>
    private float CameraPosZ
    {
        get => cameraPosZ;
        set
        {
            cameraPosZ = value;

            // プレビューバーの値を設定
            BlueprintPreviewLG.E.UI.SetBarValue(Mathf.Abs((cameraPosZ - cameraPosMaxZ) / (cameraPosMaxZ - cameraPosMinZ)));
        }
    }

    private float cameraPosZ = -30; // デフォルトZ
    private float cameraPosMinZ = -100; // 最小Z
    private float cameraPosMaxZ = -10; // 最大Z
    private float mX, mY; // カメラ角度 X, Y
    private float lookUpAngle; // ロック角度

    bool isActive = false;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        CameraPosZ = -30;
        RefreshCameraPos();

        cameraRotateY.transform.rotation = Quaternion.Euler(0, 0, 0);
        cameraRotateX.transform.rotation = Quaternion.Euler(30, 0, 0);
        BlueprintPreviewLG.E.UI.SetBarValue(Mathf.Abs((cameraPosZ - cameraPosMaxZ) / (cameraPosMaxZ - cameraPosMinZ)));
    }

    /// <summary>
    /// カメラ座標Z変化
    /// </summary>
    /// <param name="v"></param>
    public void ChangeCameraPos(float v)
    {
        CameraPosZ += v * 0.5f;

        if (CameraPosZ < cameraPosMinZ)
        {
            CameraPosZ = cameraPosMinZ;
        }

        if (CameraPosZ > cameraPosMaxZ)
        {
            CameraPosZ = cameraPosMaxZ;
        }

        RefreshCameraPos();
    }

    /// <summary>
    /// カメラ座標変化
    /// </summary>
    public void RefreshCameraPos()
    {
        BlueprintCamera.localPosition = new Vector3(0, 5, CameraPosZ);
    }

    /// <summary>
    /// カメラ角度変化
    /// </summary>
    /// <param name="mx"></param>
    /// <param name="my"></param>
    public void CameraRotate(float mx, float my)
    {
        if (!isActive)
            return;

        mX = mx * SettingMng.E.CamRotSpeed;
        mY = my * SettingMng.E.CamRotSpeed;

        lookUpAngle = cameraRotateX.transform.localRotation.eulerAngles.x - mY;

        if (lookUpAngle > SettingMng.E.LookUpAngleMax && lookUpAngle < SettingMng.E.LookUpAngleMin)
            return;

        cameraRotateX.transform.Rotate(new Vector3(-mY, 0, 0));
        cameraRotateY.transform.Rotate(new Vector3(0, mX, 0));
    }

    /// <summary>
    /// 自体をインスタンス
    /// </summary>
    /// <returns></returns>
    public static BlueprintPreviewCtl Instantiate()
    {
        var obj = CommonFunction.Instantiate<BlueprintPreviewCtl>("Prefabs/Game/Scene/BlueprintPreview", null, new Vector3(0, -10000, 0));
        obj.gameObject.SetActive(false);
        return obj;
    }

    /// <summary>
    /// アクティブ
    /// </summary>
    /// <param name="b"></param>
    public void Show(bool b = true)
    {
        isActive = b;
        gameObject.SetActive(b);

        if (b)
        {
            Init();
        }
    }

    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="data"></param>
    public void CreateEntity(BlueprintData data)
    {
        CommonFunction.ClearCell(Parent);

        Debug.Log(data.ToJosn());

        foreach (var item in data.blocks)
        {
            var entity = MapData.InstantiateEntity(new MapData.MapCellData() { entityID = item.id, direction = item.direction }, Parent, Vector3Int.zero);
            entity.transform.localPosition = item.GetPos();
        }
    }
}