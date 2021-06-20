using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NWData
{
    private string data;

    public void Add(string msg, string split = "^")
    {
        if (!string.IsNullOrEmpty(data))
            data += split;

        data += msg;
    }
    public void Add(int msg, string split = "^")
    {
        Add(msg.ToString(), split);
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(data))
            return "";

        return data;
    }
}
