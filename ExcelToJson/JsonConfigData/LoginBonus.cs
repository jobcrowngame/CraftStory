using System;

namespace JsonConfigData
{
    public class LoginBonus : ConfigBase
    {
        public int LoginBonusId { get; set; }
        public string Name { get; set; }
        public int MailId { get; set; }
        public string Time { get; set; }
    }
}
