using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EntityBed : EntityBase
{
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

        CommonFunction.ShowHintBox(@"寝ると朝になります。
寝ますか？", () =>
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

        if (WorldPos.x == DataMng.E.UserData.PlayerSpawnPosX && WorldPos.z == DataMng.E.UserData.PlayerSpawnPosZ)
        {
            DataMng.E.UserData.PlayerSpawnPosX = 0;
            DataMng.E.UserData.PlayerSpawnPosZ = 0;
        }
    }
}
