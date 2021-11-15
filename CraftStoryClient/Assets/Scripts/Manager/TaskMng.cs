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

    /// <summary>
    /// メインタスク数追加
    /// </summary>
    /// <param name="count"></param>
    public void AddMainTaskCount(int count = 1)
    {
        MainTaskClearedCount += count;
    }

    /// <summary>
    /// つぎのタスク開始
    /// </summary>
    public void Next()
    {
        // 次のタスク
        MainTaskId++;

        CheckClearedCount();

        if (IsEnd)
        {
            PlayerCtl.E.Fairy.ShowChatFlg(false);
        }
        else
        {
            PlayerCtl.E.Fairy.ChangeChatFlgImg(false);
        }
    }

    public void CheckClearedCount()
    {
        switch ((TaskType)MainTaskConfig.Type)
        {
            case TaskType.GuideEnd2: MainTaskClearedCount = DataMng.E.RuntimeData.GuideEnd2; break;
            case TaskType.GUideEnd3: MainTaskClearedCount = DataMng.E.RuntimeData.GuideEnd3; break;

            default: MainTaskClearedCount = 0; break;
        }
    }

    public enum TaskType
    {
        End = 0,
        GuideEnd2,
        GUideEnd3,
    }
}