using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class DebugUI : UIBase
{
    Transform Parent { get => FindChiled("Content"); }
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button ClearBtn { get => FindChiled<Button>("ClearBtn"); }
    Button MemoryBtn { get => FindChiled<Button>("MemoryBtn"); }
    

    public override void Init()
    {
        base.Init();
        DebugLG.E.Init(this);

        CloseBtn.onClick.AddListener(Close);
        ClearBtn.onClick.AddListener(Clear);
        MemoryBtn.onClick.AddListener(ShowMemoryInfo);
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

    private void ShowMemoryInfo()
    {
        var total = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024);
        var used = Profiler.usedHeapSizeLong / (1024f * 1024);
        var unused = Profiler.GetTotalUnusedReservedMemoryLong() / (1024f * 1024);

        Logger.Warning("Memory use info:");
        Logger.Warning("Total:{0}, Used:{1}, Unused:{2}", total, used, unused);
    }
}