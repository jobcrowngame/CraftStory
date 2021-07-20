using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonConfigData;
using LitJson;

class ConfigMng : Single<ConfigMng>
{
    Dictionary<int, Bonus> bonusConfig;
    Dictionary<int, Map> mapConfig;
    Dictionary<int, Mountain> mountainConfig;
    Dictionary<int, Resource> resourceConfig;
    Dictionary<int, TransferGate> transferGateConfig;
    Dictionary<int, Item> itemConfig;
    Dictionary<int, Craft> craftConfig;
    Dictionary<int, Building> buildingConfig;
    Dictionary<int, Shop> shopConfig;
    Dictionary<int, ErrorMsg> errorMsgConfig;
    Dictionary<int, Blueprint> blueprintConfig;
    Dictionary<int, Entity> entityConfig;

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

    public IEnumerator InitInitCoroutine()
    {
        bonusConfig = new Dictionary<int, Bonus>();
        mapConfig = new Dictionary<int, Map>();
        mountainConfig = new Dictionary<int, Mountain>();
        resourceConfig = new Dictionary<int, Resource>();
        transferGateConfig = new Dictionary<int, TransferGate>();
        itemConfig = new Dictionary<int, Item>();
        craftConfig = new Dictionary<int, Craft>();
        buildingConfig = new Dictionary<int, Building>();
        shopConfig = new Dictionary<int, Shop>();
        errorMsgConfig = new Dictionary<int, ErrorMsg>();
        blueprintConfig = new Dictionary<int, Blueprint>();
        entityConfig = new Dictionary<int, Entity>();

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
}