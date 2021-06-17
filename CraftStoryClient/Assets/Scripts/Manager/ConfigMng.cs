using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonConfigData;

class ConfigMng : Single<ConfigMng>
{
    Dictionary<int, Block> blockConfig;
    Dictionary<int, Map> mapConfig;
    Dictionary<int, Mountain> mountainConfig;
    Dictionary<int, JsonConfigData.Tree> treeConfig;
    Dictionary<int, Rock> rockConfig;
    Dictionary<int, TransferGate> transferGateConfig;
    Dictionary<int, Item> itemConfig;
    Dictionary<int, Craft> craftConfig;
    Dictionary<int, Building> buildingConfig;

    public Dictionary<int, Block> Block { get => blockConfig; }
    public Dictionary<int, Map> Map{ get => mapConfig; }
    public Dictionary<int, Mountain> Mountain { get => mountainConfig; }
    public Dictionary<int, JsonConfigData.Tree> Tree { get => treeConfig; }
    public Dictionary<int, Rock> Rock { get => rockConfig; }
    public Dictionary<int, TransferGate> TransferGate { get => transferGateConfig; }
    public Dictionary<int, Item> Item { get => itemConfig; }
    public Dictionary<int, Craft> Craft { get => craftConfig; }
    public Dictionary<int, Building> Building { get => buildingConfig; }

    public IEnumerator InitInitCoroutine()
    {
        blockConfig = new Dictionary<int, Block>();
        mapConfig = new Dictionary<int, Map>();
        mountainConfig = new Dictionary<int, Mountain>();
        treeConfig = new Dictionary<int, JsonConfigData.Tree>();
        rockConfig = new Dictionary<int, Rock>();
        transferGateConfig = new Dictionary<int, TransferGate>();
        itemConfig = new Dictionary<int, Item>();
        craftConfig = new Dictionary<int, Craft>();
        buildingConfig = new Dictionary<int, Building>();

        ReadConfig("Config/Block", blockConfig);
        ReadConfig("Config/Map", mapConfig);
        ReadConfig("Config/MapMountain", mountainConfig);
        ReadConfig("Config/Tree", treeConfig);
        ReadConfig("Config/Rock", rockConfig);
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