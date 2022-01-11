using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConfigData
{
    public class Character : ConfigBase
    {
        public string Name { get; set; }
        public string Prefab { get; set; }
        public int Type { get; set; }
        public int Race { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public int Damage { get; set; }
        public int Defense { get; set; }
        public int LvUpExp { get; set; }
        public string Skills { get; set; }
        public string PondIds { get; set; }
        public int AddExp { get; set; }
        public int SecurityRange { get; set; }
        public int CallForHelpRange { get; set; }
        public int RespondToHelp { get; set; }
        public int RandomMoveOnWait { get; set; }
        public float DazeTime { get; set; }
        public float MoveSpeed { get; set; }
        public int ChaseRange { get; set; }
    }
}
