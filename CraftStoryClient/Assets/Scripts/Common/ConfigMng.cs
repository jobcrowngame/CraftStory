using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonConfigData;

class ConfigMng : Single<ConfigMng>
{
    Dictionary<int, Block> blockConfig;
    public Dictionary<int, Block> BlockConfig { get => blockConfig; }

    public IEnumerator InitInitCoroutine()
    {
        blockConfig = new Dictionary<int, Block>();

        ReadAllConfig();

        yield return null;
    }

    private void ReadAllConfig()
    {
        ReadBlock();
    }
    private void ReadBlock()
    {
        ConfigType cType = ConfigType.Block;
        string path = GetConfigFilePath(cType);

        var config = Resources.Load<TextAsset>(path);
        if (config == null)
        {
            Debug.LogError("load Resources fail." + path);
            return;
        }

        var obj = JsonConvert.DeserializeObject<List<Block>>(config.text);
        if (obj == null)
        {
            Debug.LogError("DeserializeObject file fail." + path);
            return;
        }

        for (int i = 0; i < obj.Count; i++)
        {
            blockConfig[obj[i].ID] = obj[i];
        }
    }

    private string GetConfigFilePath(ConfigType cType)
    {
        switch (cType)
        {
            case ConfigType.Block: return "Config/Block";
            default: Debug.LogError("not find config type " + cType); return "";
        }
    }
}

enum ConfigType
{
    Block,
}