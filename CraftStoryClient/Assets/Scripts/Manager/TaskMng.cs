﻿using JsonConfigData;

public class TaskMng : Single<TaskMng>
{
    /// <summary>
    /// メインタスクId
    /// </summary>
    public int MainTaskId
    {
        get => LocalDataMng.E.Data.UserDataT.main_task;
        set
        {
            LocalDataMng.E.Data.UserDataT.main_task = value;
            //CheckClearedCount();
        }
    }

    /// <summary>
    /// 今のメインタスク内容の達成した数
    /// </summary>
    private int mMainTaskClearedCount;
    public int MainTaskClearedCount
    {
        get => mMainTaskClearedCount;
        set 
        {
            mMainTaskClearedCount = value;

            if (PlayerCtl.E.Fairy != null && IsClear)
            {
                PlayerCtl.E.Fairy.ShowChatFlg();
            }
        }
    }

    public MainTask MainTaskConfig { get => ConfigMng.E.MainTask[MainTaskId]; }
    public bool IsEnd { get => MainTaskConfig.Type == 0; }
    public bool IsClear { get => MainTaskClearedCount >= MainTaskConfig.ClearCount; }
    public bool IsReaded { get; set; }

    /// <summary>
    /// メインタスク数追加
    /// </summary>
    /// <param name="taskType">タスクタイプ</param>
    public void AddMainTaskCount(int taskType, int count = 1)
    {
        // 今のタスクと違う場合スキップ
        if (MainTaskConfig.Type != taskType)
            return;

        Logger.Log("タスク {0} 進捗変更 、+{1}", MainTaskConfig.ID, count);

        NWMng.E.AddMainTaskClearCount(() =>
        {
            MainTaskClearedCount += count;

            // タスクの条件に達成すると、妖精の吹き出しを出す
            if (MainTaskConfig.ClearCount <= MainTaskClearedCount)
            {
                if (PlayerCtl.E.Fairy != null)
                {
                    PlayerCtl.E.Fairy.ShowChatFlg();
                    HomeLG.E.UI.RefreshTaskOverview();
                }
            }
        }, count);
    }

    /// <summary>
    /// つぎのタスク開始
    /// </summary>
    public void Next()
    {
        MainTaskClearedCount = 0;

        // 次のタスク
        MainTaskId = MainTaskConfig.Next;

        if (IsEnd)
        {
            PlayerCtl.E.Fairy.ShowChatFlg(false);
        }
        else
        {
            PlayerCtl.E.Fairy.ShowChatFlg(true);
        }
        IsReaded = false;
    }

    //public void CheckClearedCount()
    //{
    //    switch ((TaskType)MainTaskConfig.Type)
    //    {
    //        case TaskType.GuideEndShop: MainTaskClearedCount = LocalDataMng.E.Data.limitedT.guide_end2; break;
    //        case TaskType.GUideEndEquipment: MainTaskClearedCount = LocalDataMng.E.Data.limitedT.guide_end4; break;
    //        case TaskType.GuideEndBlueprint: MainTaskClearedCount = LocalDataMng.E.Data.limitedT.guide_end; break;
    //        case TaskType.GuideBrave: MainTaskClearedCount = LocalDataMng.E.Data.limitedT.guide_end5; break;

    //        default: MainTaskClearedCount = 0; break;
    //    }
    //}

    public enum TaskType
    {
        End = 0,
        GuideEndShop,// ショップチュートリアル完了
        GUideEndEquipment,// 武器チュートリアル完了
        CreateMission,// 掲示板作る
        UseCamado,// かまどを使う
        GuideEndBlueprint,// 設計図チュートリアル完了
        UploadBlueprint,// マイショップアップロード
        AddGood,// 他のユーザーいいねする
        UseCoin4,// ロイヤルコイン使う
        BraveLv10,// 冒険エリア１０階
        UserLv10,// レベル10
        GuideBrave,// 冒険エリアチュートリアルが完了
        Brave,// 冒険完了
    }
}