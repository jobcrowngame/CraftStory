using JsonConfigData;
using UnityEngine;

class GuideLG : UILogicBase<GuideLG, GuideUI>
{
    private Guide config { get => ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId]; }
    private string[] guideSteps;

    /// <summary>
    /// チュートリアルが完了してるかのフラグ
    /// </summary>
    public bool end { get; set; }

    private int itemGuid = 1;
    private int createBlockCount = 0;
    private bool Lock 
    {
        get => mLock; 
        set
        {
            if (DataMng.E.RuntimeData.MapType != MapType.Guide)
                return;

            mLock = value;
            UI.ShowFullMask(mLock);
        }
    }
    private bool mLock;

    private string CurStep
    {
        set
        {
            UI.NextStep(int.Parse(value));
        }
    }
    private string mCurStep;

    public int Step { get => stepIndex; }
    private int stepIndex = 0;

    public override void Init(GuideUI ui)
    {
        base.Init(ui);

        end = false;
        guideSteps = config.StepList.Split(',');
    }
    public void ReStart()
    {
        end = false;
        stepIndex = 0;
        itemGuid = 1;
        createBlockCount = 0;
    }
    public void UnLock()
    {
        Lock = false;
    }
    public void Next(int step)
    {
        if (step != stepIndex)
            return;
        Next();
    }
    public void Next()
    {
        if (end || Lock)
            return;

        if (DataMng.E.RuntimeData.MapType != MapType.Guide)
            return;

        CurStep = guideSteps[stepIndex];
        stepIndex++;
        Lock = true;
    }
    public void GoTo(int index)
    {
        stepIndex = index - 1;
        Next();
    }

    /// <summary>
    /// クリアする
    /// </summary>
    public void Clear()
    {
        end = false;
        Lock = false;
        stepIndex = 0;
        createBlockCount = 0;
    }


    /// <summary>
    /// チュートリアル中作成したブロック数を記録
    /// </summary>
    public void CreateBlock()
    {
        createBlockCount++;
        if (createBlockCount == 3)
        {
            Next();
        }
    }
    public void SetGuideItems()
    {
        var items = ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId].ItemList.Split(',');
        var counts = ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId].ItemCount.Split(',');

        DataMng.E.GuideItems.Clear();
        for (int i = 0; i < items.Length; i++)
        {
            if (int.Parse(items[i]) == 3002)
            {
                AddBlueprint();
            }
            else
            {
                AddGuideItem(int.Parse(items[i]), int.Parse(counts[i]));
            }
        }
    }
    public void AddGuideItem(int itemId, int count)
    {
        AddGuideItem(new ItemData(itemId, count));
    }
    public void AddGuideItem(ItemData itemData)
    {
        itemData.id = itemGuid++;
        DataMng.E.GuideItems.Add(itemData);
    }
    private void AddBlueprint()
    {
        var config = ConfigMng.E.Blueprint[1];
        AddGuideItem(new ItemData()
        {
            itemId = 3002,
            count = 1,
            newName = "ハウス",
            relationData = config.Data,
            equipSite = 1,
        });
    }
}