using System;

[Serializable]
public class Statistics_userTable
{
    public int id { get; set; }
    public int maxArrivedFloor { get; set; }
    public int lastFloorCount { get; set; }
    public int totalSetBlockCount { get; set; }
    public int totalUploadBlueprintCount { get; set; }
}