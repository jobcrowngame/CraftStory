using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NWData
{
    private string data;

    public void Add(string msg)
    {
        if (!string.IsNullOrEmpty(data))
            data += "^";

        data += msg;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(data))
            return "";

        return data;
    }
}
