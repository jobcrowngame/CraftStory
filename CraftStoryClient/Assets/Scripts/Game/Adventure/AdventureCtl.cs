using System.Collections.Generic;

/// <summary>
/// 冒険コンソル
/// </summary>
public class AdventureCtl : Single<AdventureCtl>
{
    // 一時的手にいれたボーナス
    private List<int> bonusList;
    public List<int> BonusList { get => bonusList; }

    public override void Init()
    {
        base.Init();

        bonusList = new List<int>();
    }

    // ボーナス追加
    public void AddBonus(int id)
    {
        bonusList.Add(id);
        BraveLG.E.AddBonus(id);
    }

    // ボーナスリストクリア
    public void Clear()
    {
        bonusList.Clear();
    }
}