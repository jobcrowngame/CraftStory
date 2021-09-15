using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

public class MissionUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns"); }
    Transform Parent { get => FindChiled("Content"); }

    public override void Init()
    {
        base.Init();

        MissionLG.E.Init(this);

        Title.SetTitle("ミッション");
        Title.SetOnClose(Close);

        ToggleBtns.Init();
        ToggleBtns.Enable(2);
        ToggleBtns.OnValueChangeAddListener((index) => 
        {
            MissionLG.E.UType = (MissionLG.UIType)index;
        });
    }

    public override void Open()
    {
        base.Open();
        ToggleBtns.SetValue(0);
        MissionLG.E.GetMissionInfo(MissionLG.UIType.Daily);
    }

    public void RefreshCellList()
    {
        ClearCell(Parent);

        List<MissionLG.MissionCellData> overList = new List<MissionLG.MissionCellData>();
        List<MissionLG.MissionCellData> endList = new List<MissionLG.MissionCellData>();
        List<MissionLG.MissionCellData> orderList = new List<MissionLG.MissionCellData>();

        foreach (var item in ConfigMng.E.Mission.Values)
        {
            if (item.Type == (int)MissionLG.E.UType + 1)
            {
                var cellData = new MissionLG.MissionCellData();
                CheckMissionState(item, ref cellData);

                if (cellData.isOver && !cellData.isGet)
                {
                    overList.Add(cellData);
                }
                else if (cellData.isOver && cellData.isGet)
                {
                    endList.Add(cellData);
                }
                else
                {
                    orderList.Add(cellData);
                }
            }
        }

        // 達成したミッションサブを追加
        foreach (var item in overList)
        {
            AddCell(item);
        }

        // 進んでいるミッションサブを追加
        foreach (var item in orderList)
        {
            AddCell(item);
        }

        // 完成したミッションサブを追加
        foreach (var item in endList)
        {
            AddCell(item);
        }
    }
    private void AddCell(MissionLG.MissionCellData data)
    {
        var cell = AddCell<MissionCell>("Prefabs/UI/MissionCell", Parent);
        if (cell != null)
        {
            cell.Set(data);
        }
    }

    /// <summary>
    /// ミッションの状態をチェック
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="celldata"></param>
    private void CheckMissionState(Mission mission, ref MissionLG.MissionCellData celldata)
    {
        celldata.mission = mission;

        var curMissionInfo = MissionLG.E.GetMissionServerData(mission.Type, mission.ID);
        if (curMissionInfo != null)
        {
            celldata.curNumber = curMissionInfo.id == 0 ? 0 : curMissionInfo.number;
            celldata.isOver = curMissionInfo.number >= mission.EndNumber;
            celldata.isGet = curMissionInfo.state == 1;
        }
        else
        {
            celldata.curNumber = 0;
            celldata.isOver = false;
            celldata.isGet = false;
        }
    }
}
