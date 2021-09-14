using UnityEngine;
using UnityEngine.UI;

public class MissionCell : UIBase
{
    Image ItemIcon { get => FindChiled<Image>("ItemIcon"); }
    Text Count { get => FindChiled<Text>("Count"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Button Button { get => FindChiled<Button>("Button"); }
    Slider Slider { get => FindChiled<Slider>("Slider"); }
    Text SliderText { get => FindChiled<Text>("SliderText"); }
    Transform Mask { get => FindChiled("Mask"); }


    public void Set(MissionLG.MissionCellData data)
    {
        var bonus = ConfigMng.E.Bonus[data.mission.Bonus];
        string iconPath = ConfigMng.E.Item[bonus.Bonus1].IconResourcesPath;
        int targetNum = data.mission.EndNumber;
        ItemIcon.sprite = ReadResources<Sprite>(iconPath);
        Count.text = "X" + bonus.BonusCount1;

        Des.text = data.mission.Des;

        float sliderV = data.curNumber / (float)targetNum;
        if (sliderV > 1) sliderV = 1;
        Slider.value = data.curNumber / (float)targetNum;

        SliderText.text = data.curNumber > targetNum ? targetNum + "/" + targetNum : data.curNumber + "/" + targetNum;

        string pathName = data.isOver ? "mission_2d_014" : "mission_2d_015";
        if (data.isGet) pathName = "mission_2d_016";
        Button.GetComponent<Image>().sprite = ReadResources<Sprite>("Textures/" + pathName);

        Mask.gameObject.SetActive(data.isGet);

        Button.onClick.AddListener(() => 
        {
            // 達成した場合
            if (data.isOver)
            {
                NWMng.E.GetMissionBonus((rp) => 
                {
                    // ミッション状態更新
                    MissionLG.E.ChangeMissionState(data.mission.ID, data.mission.Type, 1);

                    // UI更新
                    MissionLG.E.UI.RefreshCellList();

                    CommonFunction.ShowHintBar(29);

                    // アイテム更新
                    DataMng.E.AddItem(bonus.Bonus1, bonus.BonusCount1);
                }, data.mission.ID, data.mission.Type);
            }
            // 達成するチュートリアルに遷移
            else
            {
                Logger.Warning("go to Guide");
            }
        });
    }
}