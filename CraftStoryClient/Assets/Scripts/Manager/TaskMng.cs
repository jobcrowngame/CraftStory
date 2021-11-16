using JsonConfigData;

public class TaskMng : Single<TaskMng>
{
    /// <summary>
    /// メインタスクId
    /// </summary>
    private int mMainTaskId;
    public int MainTaskId
    {
        get => mMainTaskId;
        set
        {
            mMainTaskId = value;
            CheckClearedCount();
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
                PlayerCtl.E.Fairy.ChangeChatFlgImg(false);
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
    /// <param name="count"></param>
    public void AddMainTaskCount(int taskId, int count = 1)
    {
        // 今のタスクと違う場合スキップ
        if (MainTaskId != taskId)
            return;

        NWMng.E.AddMainTaskClearCount((rp) =>
        {
            MainTaskClearedCount += count;
        }, count);
    }

    /// <summary>
    /// つぎのタスク開始
    /// </summary>
    public void Next()
    {
        MainTaskClearedCount = 0;

        // 次のタスク
        MainTaskId++;

        if (IsEnd)
        {
            PlayerCtl.E.Fairy.ShowChatFlg(false);
        }
        else
        {
            PlayerCtl.E.Fairy.ChangeChatFlgImg(false);
        }

        IsReaded = false;
    }

    public void CheckClearedCount()
    {
        switch ((TaskType)MainTaskConfig.Type)
        {
            case TaskType.GuideEndShop: MainTaskClearedCount = DataMng.E.RuntimeData.GuideEnd2; break;
            case TaskType.GUideEndEquipment: MainTaskClearedCount = DataMng.E.RuntimeData.GuideEnd4; break;
            case TaskType.GuideEndBlueprint: MainTaskClearedCount = DataMng.E.RuntimeData.GuideEnd; break;

            default: MainTaskClearedCount = 0; break;
        }
    }

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
    }
}