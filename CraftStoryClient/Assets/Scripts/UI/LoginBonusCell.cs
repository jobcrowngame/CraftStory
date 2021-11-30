using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginBonusCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Image Day { get => FindChiled<Image>("Day"); }
    Text Text { get => FindChiled<Text>("Text"); }
    Image Get { get => FindChiled<Image>("Get"); }

    Animation anim { get => GetComponent<Animation>();}

    public void Set(string id, string count, int index, bool isGeted)
    {
        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[int.Parse(id)].IconResourcesPath);
        Day.sprite = ReadResources<Sprite>(GetDayImagePath(index));
        Text.text = "x" + count;

        Get.gameObject.SetActive(false);

        if(isGeted) IsGeted(false);
    }

    private string GetDayImagePath(int index)
    {
        return "Textures/Loginbonus_Texture_00" + (index + 1);
    }

    public void IsGeted(bool showAnim = true)
    {
        Get.gameObject.SetActive(true);

        if (showAnim)
        {
            anim.Play("LoginBonusCell");
        }
    }
}
