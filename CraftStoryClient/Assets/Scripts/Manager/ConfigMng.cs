using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonConfigData;
using LitJson;

/// <summary>
/// 設定ファイルマネージャー
/// </summary>
class ConfigMng : Single<ConfigMng>
{
    Dictionary<int, Bonus> bonusConfig = new Dictionary<int, Bonus>();
    Dictionary<int, Map> mapConfig = new Dictionary<int, Map>();
    Dictionary<int, Mountain> mountainConfig = new Dictionary<int, Mountain>();
    Dictionary<int, Resource> resourceConfig = new Dictionary<int, Resource>();
    Dictionary<int, TransferGate> transferGateConfig = new Dictionary<int, TransferGate>();
    Dictionary<int, Item> itemConfig = new Dictionary<int, Item>();
    Dictionary<int, Craft> craftConfig = new Dictionary<int, Craft>();
    Dictionary<int, Building> buildingConfig = new Dictionary<int, Building>();
    Dictionary<int, Shop> shopConfig = new Dictionary<int, Shop>();
    Dictionary<int, ErrorMsg> errorMsgConfig = new Dictionary<int, ErrorMsg>();
    Dictionary<int, Blueprint> blueprintConfig = new Dictionary<int, Blueprint>();
    Dictionary<int, Entity> entityConfig = new Dictionary<int, Entity>();
    Dictionary<int, Guide> guideConfig = new Dictionary<int, Guide>();
    Dictionary<int, GuideStep> guideStepConfig = new Dictionary<int, GuideStep>();
    Dictionary<int, Gacha> gachaConfig = new Dictionary<int, Gacha>();
    Dictionary<int, Roulette> rouletteConfig = new Dictionary<int, Roulette>();
    Dictionary<int, RouletteCell> rouletteCellConfig = new Dictionary<int, RouletteCell>();
    Dictionary<int, RandomBonusPond> randomBonusPondConfig = new Dictionary<int, RandomBonusPond>();
    Dictionary<int, Mission> MissionConfig = new Dictionary<int, Mission>();
    Dictionary<int, MText> MTextConfig = new Dictionary<int, MText>();

    public Dictionary<int, MText> MText { get => MTextConfig; }
    public Dictionary<int, Mission> Mission { get => MissionConfig; }
    public Dictionary<int, RandomBonusPond> RandomBonusPond { get => randomBonusPondConfig; }
    public Dictionary<int, RouletteCell> RouletteCell { get => rouletteCellConfig; }
    public Dictionary<int, Roulette> Roulette { get => rouletteConfig; }
    public Dictionary<int, Gacha> Gacha { get => gachaConfig; }
    public Dictionary<int, Bonus> Bonus { get => bonusConfig; }
    public Dictionary<int, Map> Map{ get => mapConfig; }
    public Dictionary<int, Mountain> Mountain { get => mountainConfig; }
    public Dictionary<int, Resource> Resource { get => resourceConfig; }
    public Dictionary<int, TransferGate> TransferGate { get => transferGateConfig; }
    public Dictionary<int, Item> Item { get => itemConfig; }
    public Dictionary<int, Craft> Craft { get => craftConfig; }
    public Dictionary<int, Building> Building { get => buildingConfig; }
    public Dictionary<int, Shop> Shop { get => shopConfig; }
    public Dictionary<int, ErrorMsg> ErrorMsg { get => errorMsgConfig; }
    public Dictionary<int, Blueprint> Blueprint { get => blueprintConfig; }
    public Dictionary<int, Entity> Entity { get => entityConfig; }
    public Dictionary<int, Guide> Guide { get => guideConfig; }
    public Dictionary<int, GuideStep> GuideStep { get => guideStepConfig; }

    public IEnumerator InitInitCoroutine()
    {
        ReadConfig("Config/MText", MTextConfig);
        ReadConfig("Config/Mission", MissionConfig);
        ReadConfig("Config/RandomBonusPond", randomBonusPondConfig);
        ReadConfig("Config/RouletteCell", rouletteCellConfig);
        ReadConfig("Config/Roulette", rouletteConfig);
        ReadConfig("Config/Gacha", gachaConfig);
        ReadConfig("Config/Bonus", bonusConfig);
        ReadConfig("Config/Map", mapConfig);
        ReadConfig("Config/MapMountain", mountainConfig);
        ReadConfig("Config/Resource", resourceConfig);
        ReadConfig("Config/TransferGate", transferGateConfig);
        ReadConfig("Config/Item", itemConfig);
        ReadConfig("Config/Craft", craftConfig);
        ReadConfig("Config/Building", buildingConfig);
        ReadConfig("Config/Shop", shopConfig);
        ReadConfig("Config/ErrorMsg", errorMsgConfig);
        ReadConfig("Config/Blueprint", blueprintConfig);
        ReadConfig("Config/Entity", entityConfig);
        ReadConfig("Config/Guide", guideConfig);
        ReadConfig("Config/GuideStep", guideStepConfig);

        yield return null;
    }

    private void ReadConfig<T>(string path, Dictionary<int, T> dic) where T : ConfigBase
    {
        try
        {
            var config = Resources.Load<TextAsset>(path);
            if (config == null)
            {
                Logger.Error("load Resources fail." + path);
                return;
            }

            var list = JsonMapper.ToObject<List<T>>(config.text);
            if (list == null)
            {
                Logger.Error("DeserializeObject file fail." + path);
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                dic[list[i].ID] = list[i];
            }
        }
        catch (System.Exception ex)
        {
            Logger.Error(ex);
        }
    }

    public Shop GetShopByName(string name)
    {
        foreach (var key in Shop.Keys)
        {
            if (Shop[key].Name == name)
            {
                return Shop[key];
            }
        }
        return null;
    }
}