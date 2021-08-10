using JsonConfigData;
using UnityEngine;

class GuideLG : UILogicBase<GuideLG, GuideUI>
{
    private Guide config { get => ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId]; }
    private string[] guideSteps;
    public bool end { get; set; }

    private string CurStep
    {
        set
        {
            UI.NextStep(int.Parse(value));
        }
    }
    private string mCurStep;
    private int stepIndex = 0;

    public override void Init(GuideUI ui)
    {
        base.Init(ui);

        end = false;
        stepIndex = 0;
        guideSteps = config.StepList.Split(',');
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
}