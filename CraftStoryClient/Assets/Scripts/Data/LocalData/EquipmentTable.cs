using System;
using System.Collections.Generic;


[Serializable]
public class EquipmentTable
{
    public List<Row> list = new List<Row>();

    [Serializable]
    public struct Row
    {
        public int id { get; set; }
        public int item_guid { get; set; }
        public string skills { get; set; }
    }
}