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
    Camera renderCakera1 { get => CommonFunction.FindChiledByName<Camera>(transform, "RenderCamera1"); } // カメラ
    Camera renderCakera2 { get => CommonFunction.FindChiledByName<Camera>(transform, "RenderCamera2"); } // カメラ

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
    /// レンダリングカメラをゲット
    /// </summary>
    /// <param name="type">1=64x64 2=360x166</param>
    /// <returns></returns>
    public RenderTexture GetRenderTexture(int type)
    {
        if (type == 1)
        {
            return renderCakera1.targetTexture;
        }
        else
        {
            return renderCakera2.targetTexture;
        }
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

        mX = mx * SettingMng.CamRotSpeed;
        mY = my * SettingMng.CamRotSpeed;

        lookUpAngle = cameraRotateX.transform.localRotation.eulerAngles.x - mx;

        if (lookUpAngle > SettingMng.LookUpAngleMax && lookUpAngle < SettingMng.LookUpAngleMin)
            return;

        cameraRotateX.transform.Rotate(new Vector3(-mX, 0, 0));
        cameraRotateY.transform.Rotate(new Vector3(0, mY, 0));
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

        foreach (var item in data.blocks)
        {
            if (item.id == 10000)
                continue;

            MapData.InstantiateEntity(new MapData.MapCellData() { entityID = item.id, direction = item.direction }, Parent, item.GetPos());
        }

        Parent.GetComponent<CombineMeshObj>().Combine();
    }
}