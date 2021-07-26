using UnityEngine;

public class EntityTreasureBox : EntityBase
{
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        Logger.Warning("EntityTreasureBox cliking end");
    }
}