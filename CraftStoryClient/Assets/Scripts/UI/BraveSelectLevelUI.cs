using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class BraveSelectLevelUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Transform Paretn { get => FindChiled("Content"); }
    Text MaxFloor { get => FindChiled<Text>("MaxFloor"); }

    public override void Init()
    {
        base.Init();
        BraveSelectLevelLG.E.Init(this);

        Title.SetTitle("冒険フロア");
        Title.SetOnClose(Close);
    }

    public override void Open()
    {
        base.Open();

        BraveSelectLevelLG.E.AddCells();
    }
    public override void Close()
    {
        base.Close();

        ClearCell(Paretn);
    }

    public void AddCell(Map map)
    {
        var cell = AddCell<BraveSelectLeveCell>("Prefabs/UI/BraveSelectLeveCell", Paretn);
        if (cell != null)
        {
            cell.Set(map);
        }
    }

    public void SetMaxFloor(int floor)
    {
        MaxFloor.text = string.Format("現在の最大到達階層：{0}F", floor);
    }
}