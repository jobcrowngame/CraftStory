using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonConfigData;

class ConfigMng : Single<ConfigMng>
{
    Dictionary<int, Block> blockConfig;
    Dictionary<int, Bonus> bonusConfig;
    Dictionary<int, Map> mapConfig;
    Dictionary<int, Mountain> mountainConfig;
    Dictionary<int, Resource> resourceConfig;
    Dictionary<int, TransferGate> transferGateConfig;
    Dictionary<int, Item> itemConfig;
    Dictionary<int, Craft> craftConfig;
    Dictionary<int, Building> buildingConfig;

    public Dictionary<int, Block> Block { get => blockConfig; }
    public Dictionary<int, Bonus> Bonus { get => bonusConfig; }
    public Dictionary<int, Map> Map{ get => mapConfig; }
    public Dictionary<int, Mountain> Mountain { get => mountainConfig; }
    public Dictionary<int, Resource> Resource { get => resourceConfig; }
    public Dictionary<int, TransferGate> TransferGate { get => transferGateConfig; }
    public Dictionary<int, Item> Item { get => itemConfig; }
    public Dictionary<int, Craft> Craft { get => craftConfig; }
    public Dictionary<int, Building> Building { get => buildingConfig; }

    public IEnumerator InitInitCoroutine()
    {
        blockConfig = new Dictionary<int, Block>();
        bonusConfig = new Dictionary<int, Bonus>();
        mapConfig = new Dictionary<int, Map>();
        mountainConfig = new Dictionary<int, Mountain>();
        resourceConfig = new Dictionary<int, Resource>();
        transferGateConfig = new Dictionary<int, TransferGate>();
        itemConfig = new Dictionary<int, Item>();
        craftConfig = new Dictionary<int, Craft>();
        buildingConfig = new Dictionary<int, Building>();

        ReadConfig("Config/Block", blockConfig);
        ReadConfig("Config/Bonus", bonusConfig);
        ReadConfig("Config/Map", mapConfig);
        ReadConfig("Config/MapMountain", mountainConfig);
        ReadConfig("Config/Resource", resourceConfig);
        ReadConfig("Config/TransferGate", transferGateConfig);
        ReadConfig("Config/Item", itemConfig);
        ReadConfig("Config/Craft", craftConfig);
        ReadConfig("Config/Building", buildingConfig);

        yield return null;
    }

    private void ReadConfig<T>(string path, Dictionary<int, T> dic) where T : Base
    {
        var config = Resources.Load<TextAsset>(path);
        if (config == null)
        {
            Debug.LogError("load Resources fail." + path);
            return;
        }

        var list = JsonConvert.DeserializeObject<List<T>>(config.text);
        if (list == null)
        {
            Debug.LogError("DeserializeObject file fail." + path);
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            dic[list[i].ID] = list[i];
        }
    }
}