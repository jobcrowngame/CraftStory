using UnityEngine;
using UnityEngine.UI;

public class SkillExplanationUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Name { get => FindChiled<Text>("Name"); }
    //Text Damage { get => FindChiled<Text>("Damage"); }
    Text Des { get => FindChiled<Text>("Des"); }

    int skillId;

    public override void Init(object data)
    {
        base.Init(data);

        SkillExplanationLG.E.Init(this);

        Title.SetTitle("�X�L������");
        Title.SetOnClose(Close);
    }

    public override void Open(object data)
    {
        base.Open(data);

        skillId = (int)data;

        var config = ConfigMng.E.Skill[skillId];

        Icon.sprite = ReadResources<Sprite>(config.Icon);
        Name.text = config.Name;
        Des.text = config.Des;
    }
}