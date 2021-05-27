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

    public Dictionary<int, Block> Block { get => blockConfig; }
    public Dictionary<int, Map> Map{ get => mapConfig; }
    public Dictionary<int, Mountain> Mountain { get => mountainConfig; }

    public IEnumerator InitInitCoroutine()
    {
        blockConfig = new Dictionary<int, Block>();
        mapConfig = new Dictionary<int, Map>();
        mountainConfig = new Dictionary<int, Mountain>();

        ReadConfig("Config/Block", blockConfig);
        ReadConfig("Config/Map", mapConfig);
        ReadConfig("Config/MapMountain", mountainConfig);

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

    enum ConfigType
    {
        Block,
        Map,
        Mountain,
    }
}