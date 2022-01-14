using System;

[Serializable]
public class UserDataTable
{
    public UserDataTable()
    {
        lv = 1;
        main_task = 1;
    }

    public int lv { get; set; }
    public int exp { get; set; }
    public int maxArrivedFloor { get; set; }
    public int guide_end3 { get; set; }
    public int main_task { get; set; }
    public int main_task_count { get; set; }
}