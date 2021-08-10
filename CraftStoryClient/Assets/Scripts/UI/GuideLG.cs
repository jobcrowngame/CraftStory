using JsonConfigData;
using UnityEngine;

class GuideLG : UILogicBase<GuideLG, GuideUI>
{
    private Guide config { get => ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId]; }
    private string[] guideSteps;

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

        stepIndex = 0;
        guideSteps = config.StepList.Split(',');
    }
    public void Next()
    {
        if (DataMng.E.RuntimeData.MapType != MapType.Guide)
            return;

        CurStep = guideSteps[stepIndex];
        stepIndex++;
    }
}