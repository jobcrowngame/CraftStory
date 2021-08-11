using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RuntimeData
{
    public int NewEmailCount { get; set; }
    public int SubscriptionLv { get; set; }
    public int GuideEnd { get; set; }
    public bool IsReceivedSubscription { get; set; }
    public DateTime SubscriptionUpdateTime { get; set; }

    public MapType MapType { get; set; }
    public int GuideId { get; set; }
}