using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConfigData
{
    public class AdventureBuff : ConfigBase
    {
        public string Name { get; set; }
        public string ResourcesPath { get; set; }
        public int TargetGroup { get; set; }
        public int Skill { get; set; }
    }
}
