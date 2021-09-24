using UnityEngine;

public class BlueprintReNameLG : UILogicBase<BlueprintReNameLG, BlueprintReNameUI>
{
    public Texture2D PhotographTexture
    {
        get => mPhotographTexture;
        set
        {
            mPhotographTexture = value;
            UIStep++;
        }
    }
    private Texture2D mPhotographTexture;

    public int UIStep
    {
        get => mUIStep;
        set
        {
            mUIStep = value;
            UI.OnStepChange(value);
        }
    }
    private int mUIStep = 0;
}
