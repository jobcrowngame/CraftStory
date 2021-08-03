using UnityEngine;
using SimpleInputNamespace;

public class PlayerEntity : CharacterEntity
{
    private CharacterController controller;

    private Vector3 moveDirection = Vector3.zero;
    private float x, y;

    private PlayerBehaviorType beforBehaveior = PlayerBehaviorType.Waiting;

    public override void Init()
    {
        base.Init();
        
        controller = GetComponent<CharacterController>();
    }

    public void Update()
    {
        if (Behavior.Type == PlayerBehaviorType.Jump && controller.isGrounded)
            Behavior.Type = beforBehaveior;
    }

    public void Move(float x, float y)
    {
        if (Behavior.Type == PlayerBehaviorType.Breack)
            return;

        var angle1 = GetAngleFromV2(new Vector2(x, y).normalized);
        var angle2 = PlayerCtl.E.CameraCtl.GetEulerAngleY;
        var newVec = GetV2FromAngle(angle1 + angle2);

        //キャラクターの移動と回転
        if (controller.isGrounded && Behavior.Type != PlayerBehaviorType.Jump)
        {
            if (x != 0 || y != 0)
            {
                moveDirection.x = newVec.x * SettingMng.E.MoveSpeed;
                moveDirection.z = newVec.y * SettingMng.E.MoveSpeed;

                if (MoveBoundaryCheckPosX(moveDirection.x))
                    moveDirection.x = 0;
                if (MoveBoundaryCheckPosZ(moveDirection.z))
                    moveDirection.z = 0;

                Behavior.Type = PlayerBehaviorType.Run;
            }
            else
            {
                moveDirection.x = 0;
                moveDirection.z = 0;
                Behavior.Type = PlayerBehaviorType.Waiting;
            }

            moveDirection.y = 0;
        }

        if (x != 0 || y != 0)
            Model.rotation = Quaternion.Euler(new Vector3(0, angle1 + angle2, 0));

        moveDirection.y -= SettingMng.E.Gravity * Time.deltaTime;

        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);

        if (transform.position.y < -10)
        {
            transform.position = MapCtl.GetGroundPos(DataMng.E.HomeData, DataMng.E.HomeData.Config.PlayerPosX, DataMng.E.HomeData.Config.PlayerPosZ, 5);
        }
    }
    public void Jump()
    {
        if (Behavior.Type != PlayerBehaviorType.Jump)
        {
            beforBehaveior = Behavior.Type;

            moveDirection.y = SettingMng.E.JumpSpeed;
            Behavior.Type = PlayerBehaviorType.Jump;
        }
    }

    private Vector2 GetV2FromAngle(float angle)
    {
        float radian = Mathf.PI * angle / 180.0f; // 度数法(度)から弧度法(ラジアン)に変換
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }
    private float GetAngleFromV2(Vector2 v)
    {
        return -Vector2.SignedAngle(new Vector2(0, 1), v);
    }

    /// <summary>
    /// 移動範囲境界判断　X
    /// </summary>
    private bool MoveBoundaryCheckPosX(float posX)
    {
        if (transform.position.x < SettingMng.E.MoveBoundaryOffset 
            && transform.position.x + posX < transform.position.x)
                return true;

        if (transform.position.x > DataMng.E.MapData.Config.SizeX - SettingMng.E.MoveBoundaryOffset
            && transform.position.x + posX > transform.position.x)
            return true;

        return false;
    }
    /// <summary>
    /// 移動範囲境界判断　Y
    /// </summary>
    private bool MoveBoundaryCheckPosZ(float posZ)
    {
        if (transform.position.z < SettingMng.E.MoveBoundaryOffset
           && transform.position.z + posZ < transform.position.z)
            return true;

        if (transform.position.z > DataMng.E.MapData.Config.SizeZ - SettingMng.E.MoveBoundaryOffset
            && transform.position.z + posZ > transform.position.z)
            return true;

        return false;
    }

    public void IsModelActive(bool b)
    {
        controller.radius = b ? 0.45f : 0.01f;
    }
}