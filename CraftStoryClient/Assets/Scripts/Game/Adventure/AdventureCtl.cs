using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AdventureCtl : Single<AdventureCtl>
{
    private List<int> getResourcesList;

    public override void Init()
    {
        base.Init();

        getResourcesList = new List<int>();
    }

    public void BreackResources(int id)
    {
        getResourcesList.Add(id);
    }
}