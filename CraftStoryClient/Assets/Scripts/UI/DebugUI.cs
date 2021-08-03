using UnityEngine;
using UnityEngine.UI;

public class DebugUI : UIBase
{
    Transform Parent { get => FindChiled("Content"); }
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button ClearBtn { get => FindChiled<Button>("ClearBtn"); }

    public override void Init()
    {
        base.Init();
        DebugLG.E.Init(this);

        CloseBtn.onClick.AddListener(Close);
        ClearBtn.onClick.AddListener(Clear);
    }

    public void Add(string msg)
    {
        var cell = AddCell<DebugCell>("Prefabs/UI/Common/DebugCell", Parent);
        cell.Set(msg);
    }
    public void Clear()
    {
        ClearCell(Parent);
    }
}