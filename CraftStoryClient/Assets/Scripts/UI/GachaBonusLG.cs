using System.Collections.Generic;

public class GachaBonusLG : UILogicBase<GachaBonusLG, GachaBonusUI>
{
    public List<GachaBonusCell> cellList = new List<GachaBonusCell>();
    private int index;

    public void OnOpen()
    {
        index = 0;
    }

    /// <summary>
    /// サブをリストに追加
    /// </summary>
    public void AddCell(GachaBonusCell cell)
    {
        cellList.Add(cell);
    }

    /// <summary>
    /// 次のボックスを開ける
    /// </summary>
    public void OpenNext()
    {
        // 全部開けたら、終わり
        if (index == cellList.Count)
        {
            UI.ShowOkBtn();
            return;
        }

        var cell = cellList[index];
        if (cell.IsLvUp)
        {
            cell.StartLvUpAnim();
        }
        else
        {
            cell.StartOpenAnim();
        }

        index++;
    }
}