using UnityEngine.UI;

public class AreaMapSettingUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Button ResetAreaMap { get => FindChiled<Button>("ResetAreaMap"); }
    Button ResetAreaMapDeterioration { get => FindChiled<Button>("ResetAreaMapDeterioration"); }

    public override void Init()
    {
        base.Init();

        AreaMapSettingLG.E.Init(this);

        Title.SetTitle("サバイバルエリア設定");
        Title.SetOnClose(() => { Close(); });

        ResetAreaMap.onClick.AddListener(() =>
        {
            AreaMapSettingLG.E.AreaMapDeterioration(false);
        });
        ResetAreaMapDeterioration.onClick.AddListener(() =>
        {
            AreaMapSettingLG.E.AreaMapDeterioration(true);
        });
    }
}
