using LitJson;

/// <summary>
/// 通信データ
/// </summary>
public class NWData
{
    JsonData jd;

    public NWData() 
    { 
        jd = new JsonData();

        if (!string.IsNullOrEmpty(DataMng.E.token))
            Add("token", DataMng.E.token);

        if (!string.IsNullOrEmpty(DataMng.E.UserData.Account))
            Add("acc", DataMng.E.UserData.Account);
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(jd.ToString()))
            return "";

        return jd.ToJson();
    }

    public void Add(string key, string value)
    {
        jd[key] = value;
    }
    public void Add(string key, int value) 
    {
        jd[key] = value;
    }
    public void Add(string key, float value)
    {
        jd[key] = value;
    }
}
