using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AdventureCtl : Single<AdventureCtl>
{
    private List<int> bonusList;
    public List<int> BonusList { get => bonusList; }

    public override void Init()
    {
        base.Init();

        bonusList = new List<int>();
    }

    public void AddBonus(int id)
    {
        bonusList.Add(id);
    }

    public void Clear()
    {
        bonusList.Clear();
    }
}