using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EntityBed : EntityBase
{
    string msg1 = @"「寝ると朝になります。
また、新たなスポーン地点に設定されます。
寝ますか？
※スポーン地点に設定したベッドを壊すと初期のスポーン地点がリセットされます」";

    string msg2 = @"「寝ると朝になります。
寝ますか？";


    /// <summary>
    /// クリックイベント
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();

        if (!WorldMng.E.GameTimeCtl.CanSleep)
        {
            CommonFunction.ShowHintBar(36);
            return;
        }

        string msg = IsSpawn() ? msg2 : msg1;

        CommonFunction.ShowHintBox(msg, () =>
        {
            if (!WorldMng.E.GameTimeCtl.CanSleep)
            {
                CommonFunction.ShowHintBar(36);
                return;
            }

            DataMng.E.UserData.PlayerSpawnPosX = WorldPos.x;
            DataMng.E.UserData.PlayerSpawnPosZ = WorldPos.z;

            HomeLG.E.UI.FadeOutAndIn();
        }, () => { });
    }

    /// <summary>
    /// 長い時間クリック終了場合のロジック
    /// </summary>
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        OnDestroyEntity();

        if (IsSpawn())
        {
            DataMng.E.UserData.PlayerSpawnPosX = 0;
            DataMng.E.UserData.PlayerSpawnPosZ = 0;
        }
    }

    private bool IsSpawn()
    {
        return WorldPos.x == DataMng.E.UserData.PlayerSpawnPosX && WorldPos.z == DataMng.E.UserData.PlayerSpawnPosZ;
    }
}
