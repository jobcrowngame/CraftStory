using UnityEngine;
using UnityEngine.UI;

public class MapUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button BraveBtn { get => FindChiled<Button>("BraveBtn"); }
    Button AreaMapBtn { get => FindChiled<Button>("AreaMapBtn"); }
    Button AreaMapSettingBtn { get => FindChiled<Button>("AreaMapSettingBtn"); }
    Button EquipBtn { get => FindChiled<Button>("EquipBtn"); }

    public override void Init()
    {
        base.Init();

        CloseBtn.onClick.AddListener(Close);
        HomeBtn.onClick.AddListener(OnClickHomeBtn);
        BraveBtn.onClick.AddListener(OnClickBraveBtn);
        AreaMapBtn.onClick.AddListener(OnClickAreaMapBtn);
        AreaMapSettingBtn.onClick.AddListener(OnClickAreaMapSettingBtn);
        EquipBtn.onClick.AddListener(OnClickEquipBtn);
    }

    public override void Open()
    {
        base.Open();

        EnActiveBtn(HomeBtn, DataMng.E.RuntimeData.MapType == MapType.Home);
        EnActiveBtn(BraveBtn, DataMng.E.RuntimeData.MapType == MapType.Brave);
        EnActiveBtn(AreaMapBtn, DataMng.E.RuntimeData.MapType == MapType.AreaMap);
        EnActiveBtn(AreaMapSettingBtn, DataMng.E.RuntimeData.MapType == MapType.AreaMap);

        // 冒険エリアの場合、時間を停止
        if (DataMng.E.RuntimeData.MapType == MapType.Brave)
        {
            PlayerCtl.E.Pause();
        }

        MapLG.E.LockBtn(false);
    }
    public override void Close()
    {
        base.Close();

        PlayerCtl.E.Pause(false);
    }

    private void OnClickHomeBtn()
    {
        if (MapLG.E.BtnIsLocked)
            return;

        MapLG.E.LockBtn();

        GuideLG.E.Next();

        // ホームからホーム遷移できません
        if (DataMng.E.RuntimeData.MapType == MapType.Home)
            return;

        // 冒険途中でホーム戻る場合、ボーナス計算します
        if (DataMng.E.RuntimeData.MapType == MapType.Brave ||
            DataMng.E.RuntimeData.MapType == MapType.Event)
        {
            AdventureCtl.E.GetBonus(() =>
            {
                CommonFunction.GoToNextScene(100);
            });
        }
        else
        {
            CommonFunction.GoToNextScene(100);
        }
    }
   
    private void OnClickBraveBtn ()
    {
        if (!PlayerCtl.E.Character.IsEquipedEquipment())
        {
            OnClickEquipBtn();
            return;
        }

        if (DataMng.E.RuntimeData.MapType != MapType.Brave)
        {
            UICtl.E.OpenUI<BraveSelectLevelUI>(UIType.BraveSelectLevel);
        }
    }
    private void OnClickAreaMapBtn()
    {
        // 冒険途中でエリアマップに遷移する場合、ボーナス計算します
        if (DataMng.E.RuntimeData.MapType == MapType.Brave ||
            DataMng.E.RuntimeData.MapType == MapType.Event)
        {
            AdventureCtl.E.GetBonus(() =>
            {
                CommonFunction.GotoAreaMap();
            });
        }
        else
        {
            CommonFunction.GotoAreaMap();
        }
    }
    private void OnClickAreaMapSettingBtn()
    {
        UICtl.E.OpenUI<AreaMapSettingUI>(UIType.AreaMapSetting);
    }
    private void OnClickEquipBtn()
    {
        UICtl.E.OpenUI<EquipUI>(UIType.Equip, UIOpenType.BeforeClose);
    }

    private void EnActiveBtn(Button btn, bool b = true)
    {
        btn.GetComponent<Image>().color = b ?
            Color.gray :
            Color.white;
        btn.enabled = !b;
    }
}
