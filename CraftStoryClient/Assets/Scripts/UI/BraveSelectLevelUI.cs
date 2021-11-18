using JsonConfigData;
using UnityEngine;

public class BraveSelectLevelUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Transform Paretn { get => FindChiled("Content"); }

    public override void Init()
    {
        base.Init();
        BraveSelectLevelLG.E.Init(this);

        Title.SetTitle("冒険段階選択");
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
}