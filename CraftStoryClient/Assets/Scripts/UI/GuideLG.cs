using JsonConfigData;
using UnityEngine;

class GuideLG : UILogicBase<GuideLG, GuideUI>
{
    private Guide config { get => ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId]; }
    private string[] guideSteps;
    public bool end { get; set; }

    private int itemGuid = 1;
    private int createBlockCount = 0;

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
    }
    public void Next(int step)
    {
        if (step != stepIndex)
            return;
        Next();
    }
    public void Next()
    {
        if (end)
            return;

        if (DataMng.E.RuntimeData.MapType != MapType.Guide)
            return;

        CurStep = guideSteps[stepIndex];
        stepIndex++;
    }
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
            AddGuideItem(int.Parse(items[i]), int.Parse(counts[i]));
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
}