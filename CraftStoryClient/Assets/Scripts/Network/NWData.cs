using LitJson;

/// <summary>
/// 通信データ
/// </summary>
public class NWData
{
    JsonData jd;

    public NWData() { jd = new JsonData(); }

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
