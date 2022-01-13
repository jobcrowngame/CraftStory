using System;

[Serializable]
public class limitedTable
{
    public limitedTable()
    {
        main_task = 1;
    }

    public int guide_end { get; set; }
    public int guide_end2 { get; set; }
    public int guide_end3 { get; set; }
    public int guide_end4 { get; set; }
    public int guide_end5 { get; set; }

    public int main_task { get; set; }
    public int main_task_count { get; set; }
}