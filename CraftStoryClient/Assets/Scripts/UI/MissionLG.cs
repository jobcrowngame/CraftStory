using JsonConfigData;

using System.Collections.Generic;

public class MissionLG : UILogicBase<MissionLG, MissionUI>
{
    public Dictionary<int, MissionServerData> dailyDic = new Dictionary<int, MissionServerData>();
    public Dictionary<int, MissionServerData> mainDic = new Dictionary<int, MissionServerData>();
    public Dictionary<int, MissionServerData> limitDic = new Dictionary<int, MissionServerData>();
    public int loginNumber = 0;

    public UIType UType
    {
        get => mUType;
        set
        {
            mUType = value;

            UI.RefreshCellList();
        }
    }
    private UIType mUType = UIType.None;

    /// <summary>
    /// サーバーからミッション情報をゲット
    /// </summary>
    public void GetMissionInfo(UIType uType)
    {
        NWMng.E.GetMissionInfo((rp) =>
        {
            ParseMission((string)rp["daily"], dailyDic);
            ParseMission((string)rp["main"], mainDic);
            ParseMission((string)rp["limit"], limitDic);
            loginNumber = (int)rp["loginNumber"];

            UType = uType;
        });
    }

    /// <summary>
    /// ミッションをゲット
    /// </summary>
    /// <param name="misstionId">ミッションID</param>
    /// <param name="misstionType">ミッションタイプ</param>
    /// <returns></returns>
    public void ChangeMissionState(int misstionId, int misstionType, int state)
    {
        switch (misstionType)
        {
            case 1: dailyDic[misstionId].state = state; break;
            case 2: mainDic[misstionId].state = state; break;
            default: limitDic[misstionId].state = state; break;
        }
    }

    /// <summary>
    /// ミッション情報を解析
    /// </summary>
    /// <param name="data">データ</param>
    /// <param name="dic">辞書</param>
    private void ParseMission(string data, Dictionary<int, MissionServerData> dic)
    {
        if (string.IsNullOrEmpty(data))
            return;
        
        var list = data.Split(',');
        foreach (var item in list)
        {
            var dicData = item.Split('^');
            dic[int.Parse(dicData[0])] = new MissionServerData() 
            {
                id = int.Parse(dicData[0]),
                number = int.Parse(dicData[1]),
                state = int.Parse(dicData[2])
            };
        }
    }

    /// <summary>
    /// 指定ミッション情報をゲット
    /// </summary>
    /// <param name="type">ミッションタイプ</param>
    /// <param name="id">ミッションID</param>
    /// <returns></returns>
    public MissionServerData GetMissionServerData(int type, int id) 
    {
        var sData = new MissionServerData();
        switch (type)
        {
            case 1: dailyDic.TryGetValue(id, out sData); break;
            case 2: mainDic.TryGetValue(id, out sData); break;
            case 3: limitDic.TryGetValue(id, out sData); break;
        }
        return sData;
    }

    public class MissionServerData 
    {
        public int id { get; set; }
        public int number { get; set; }
        public int state { get; set; }
    }

    public struct MissionCellData
    {
        public Mission mission { get; set; }
        public int curNumber { get; set; }
        public bool isOver { get; set; }
        public bool isGet { get; set; }
    }

    public enum UIType
    {
        None = -1,
        Daily,
        Main,
        LimitedTime,
    }
}
