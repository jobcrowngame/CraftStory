namespace JsonConfigData
{
    public class Gacha : ConfigBase
    {
        public int PondId {get;set;}
        public string Title { get; set; }
        public string Des { get;set;}
        public int Cost { get;set;}
        public int CostCount { get;set; }
        public int Roulette { get; set; }
        public int AddBonusPercent { get; set; }
    }
}
