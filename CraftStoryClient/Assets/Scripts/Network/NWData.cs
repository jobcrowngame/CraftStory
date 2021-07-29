using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

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
