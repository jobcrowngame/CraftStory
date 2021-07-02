using UnityEngine;

public class BlueprintPreviewCtl : MonoBehaviour
{
    Transform Parent { get => CommonFunction.FindChiledByName(transform, "Parent").transform; }
    Transform BlueprintCamera { get => CommonFunction.FindChiledByName(transform, "Camera").transform; }
    Transform cameraRotateX { get => CommonFunction.FindChiledByName(transform, "X").transform; }
    Transform cameraRotateY { get => CommonFunction.FindChiledByName(transform, "Y").transform; }

    private float CameraPosZ
    {
        get => cameraPosZ;
        set
        {
            cameraPosZ = value;
            BlueprintPreviewLG.E.UI.SetBarValue(Mathf.Abs((cameraPosZ - cameraPosMaxZ) / (cameraPosMaxZ - cameraPosMinZ)));
        }
    }
    private float cameraPosZ = -30;
    private float cameraPosMinZ = -100;
    private float cameraPosMaxZ = -10;
    private float mX, mY;
    private float lookUpAngle;

    bool isActive = false;

    public void Deforuto()
    {
        CameraPosZ = -30;
        RefreshCameraPos();

        cameraRotateY.transform.rotation = Quaternion.Euler(0, 0, 0);
        cameraRotateX.transform.rotation = Quaternion.Euler(30, 0, 0);
        BlueprintPreviewLG.E.UI.SetBarValue(Mathf.Abs((cameraPosZ - cameraPosMaxZ) / (cameraPosMaxZ - cameraPosMinZ)));
    }
    public void ChangeCameraPos(float v)
    {
        CameraPosZ += v * 0.01f;

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
    public void OnClickPlussBtn()
    {
        if (CameraPosZ < cameraPosMaxZ)
        {
            CameraPosZ += 5;
            RefreshCameraPos();
        }
    }
    public void OnClickMinusBtn()
    {
        if (CameraPosZ > cameraPosMinZ)
        {
            CameraPosZ -= 5;
            RefreshCameraPos();
        }
    }
    public void RefreshCameraPos()
    {
        BlueprintCamera.localPosition = new Vector3(0, 0, CameraPosZ);
    }

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

    public static BlueprintPreviewCtl Instantiate()
    {
        var obj = CommonFunction.Instantiate<BlueprintPreviewCtl>("Prefabs/Game/Scene/BlueprintPreview", null, new Vector3(0, -10000, 0));
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void Show(bool b = true)
    {
        isActive = b;
        gameObject.SetActive(b);

        if (b)
        {
            Deforuto();
        }
    }

    public void CreateBlock(BlueprintData data)
    {
        CommonFunction.ClearCell(Parent);

        foreach (var item in data.BlockList)
        {
            item.Pos = new Vector3Int(item.Pos.x, (int)transform.position.y + item.Pos.y, item.Pos.z);
            item.ActiveBlock(Parent);
        }
    }
}