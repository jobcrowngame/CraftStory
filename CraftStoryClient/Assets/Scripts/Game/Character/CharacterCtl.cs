using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクタコンソール
/// </summary>
public class CharacterCtl
{
    private CharacterPlayer player;
    private List<CharacterBase> characterList = new List<CharacterBase>();

    // 敵の数
    private int mRemainingNumber = 0;
    public int RemainingNumber { get => mRemainingNumber; }

    // エリアマップの敵が生成される時間
    int createAreaMapMonsterTimer = 0;

    List<int> mAreaMapMonsterList = new List<int>();

    public void CreateCharacter()
    {
        AddPlayer();

        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            mAreaMapMonsterList = GetAreaMapCreateMonsterEntityId();
            mRemainingNumber = 0;
            createAreaMapMonsterTimer = SettingMng.AreaMapCreateMonsterInterval;
            TimeZoneMng.E.AddTimerEvent03(AddAreaMapMonster);
        }
        else
        {
            AddMonsters();
        }

        AddFollowCharacter();
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i] == null || characterList[i].IsDied)
                continue;

            characterList[i].OnUpdate();
        }
    }

    /// <summary>
    /// モンスターを倒す場合
    /// </summary>
    public void MonsterDied()
    {
        mRemainingNumber--;
        CheckMapClear();
    }

    /// <summary>
    /// 冒険マップがクリアしたかの判定
    /// </summary>
    private void CheckMapClear()
    {
        // エリアマップの場合、スキップ
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
            return;

        if (HomeLG.E.UI != null)
            HomeLG.E.UI.UpdateNumberLeftMonster(mRemainingNumber);

        if (mRemainingNumber == 0)
        {
            DataMng.E.MapData.OnClear();
        }
    }

    #region キャラクタ生成

    /// <summary>
    /// プレイヤーエンティティをインスタンス
    /// </summary>
    /// <returns></returns>
    public void AddPlayer()
    {
        var resource = Resources.Load("Prefabs/Game/Character/Player") as GameObject;
        if (resource == null)
        {
            Logger.Error("not find Player Prefabs");
            return;
        }

        // 生成する座標ゲット
        Vector3 pos = Vector3.zero;
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            pos = WorldMng.E.MapMng.GetPlayerGroundPos();
        }
        else
        {
            pos = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 1, DataMng.E.MapData.Config.CreatePosOffset);
        }

        var obj = GameObject.Instantiate(resource, pos, Quaternion.identity);
        if (obj == null)
        {
            Logger.Error("Player Instantiate fail");
            return;
        }

        player = obj.GetComponent<CharacterPlayer>();
        player.Init(LocalDataMng.E.Data.UserDataT.lv, CharacterBase.CharacterGroup.Player);
        var hpbar = CommonFunction.FindChiledByName<HpUIBase>(player.transform, "WorldUI");
        player.SetHpBar(hpbar);

        characterList.Add(player);
        PlayerCtl.E.SetCharacter(player);

        if (PlayerCtl.E.Character == null)
        {
            Logger.Error("not find CharacterEntity component");
            return;
        }
    }

    /// <summary>
    /// モンスターを生成
    /// </summary>
    private void AddMonsters()
    {
        var characterGeneratedStr = DataMng.E.MapData.Config.CharacterGenerated;
        string[] characterGenerated = characterGeneratedStr.Split(',');

        mRemainingNumber = (characterGeneratedStr != "N")
            ? characterGenerated.Length
            : 0;

        foreach (var item in characterGenerated)
        {
            if (item == "N")
                continue;

            AddMonster(int.Parse(item));
        }

        CheckMapClear();

        if (HomeLG.E.UI != null)
            HomeLG.E.UI.ShowMonsterNumberLeft();
    }

    /// <summary>
    /// モンスターを生成
    /// </summary>
    public void AddMonster(int characterGeneratedID, int posX = -1, int posZ = -1)
    {
        // モンスターをインスタンス
        var characterGeneratedCfg = ConfigMng.E.CharacterGenerated[characterGeneratedID];
        var characterCfg = ConfigMng.E.Character[characterGeneratedCfg.CharacterId];

        posX = posX > 0 ? posX : characterGeneratedCfg.PosX;
        posZ = posZ > 0 ? posZ : characterGeneratedCfg.PosZ;

        int CreatePosOffset = DataMng.E.MapData != null ? DataMng.E.MapData.Config.CreatePosOffset : 3;

        // 生成する座標ゲット
        Vector3Int pos = MapMng.GetGroundPos(posX, posZ, 1, CreatePosOffset);

        var resource = Resources.Load(characterCfg.Prefab) as GameObject;
        if (resource == null)
        {
            Logger.Error("not find Monster Prefabs： {0}", characterCfg.Prefab);
            return;
        }

        var obj = GameObject.Instantiate(resource, pos, Quaternion.identity);
        if (obj == null)
        {
            Logger.Error("Monster Instantiate fail ID: {0}", characterCfg.ID);
            return;
        }

        var monster = obj.GetComponent<CharacterMonster>();
        if (monster == null)
        {
            Logger.Error("not find CharacterMonster component");
            return;
        }

        monster.Init(characterCfg.ID, CharacterBase.CharacterGroup.Monster);
        monster.SetHpBar(CommonFunction.FindChiledByName<HpUIBase>(monster.transform, "WorldUI"));
        characterList.Add(monster);
    }

    /// <summary>
    /// フォローしてる妖精を生成
    /// </summary>
    private void AddFollowCharacter()
    {
        // チュートリアルの場合、スキップ
        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            return;

        int id = 10001;
        var config = ConfigMng.E.Character[id];

        // 生成する座標ゲット
        var followCharacter = CommonFunction.Instantiate<CharacterFollow>(config.Prefab, null, player.FollowPoint.position);
        if (followCharacter == null)
        {
            Logger.Error("EntityFollowCharacter Instantiate fail");
            return;
        }

        followCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0);
        followCharacter.Init(id, CharacterBase.CharacterGroup.Fairy);
        followCharacter.SetTarget(player.FollowPoint);

        PlayerCtl.E.Fairy = followCharacter;
    }

    /// <summary>
    /// エリアマップのモンスター生成
    /// </summary>
    public void AddAreaMapMonster()
    {
        // 主人公がない場合、スキップ
        if (player == null)
            return;

        // 夜になった場合、敵スポーン時間チェックして更新
        if (WorldMng.E.GameTimeCtl.IsNight && createAreaMapMonsterTimer > SettingMng.AreaMapCreateMonsterNightInterval)
            createAreaMapMonsterTimer = SettingMng.AreaMapCreateMonsterNightInterval;

        createAreaMapMonsterTimer--;
        if (createAreaMapMonsterTimer > 0)
            return;

        createAreaMapMonsterTimer = WorldMng.E.GameTimeCtl.IsNight ? SettingMng.AreaMapCreateMonsterNightInterval : SettingMng.AreaMapCreateMonsterInterval;
        int defaltMaxCount = WorldMng.E.GameTimeCtl.IsNight ? SettingMng.AreaMapCreateMonsterNightMaxCount : SettingMng.AreaMapCreateMonsterMaxCount;

        int range = Random.Range(0, defaltMaxCount - RemainingNumber + 1);
        var createPosList = GetAreaMapCreateMonsterPosList();

        // 生成できる座標がないとスキップ
        if (createPosList.Count <= 0)
            return;

        for (int i = 0; i < range; i++)
        {
            int rangePos = Random.Range(0, createPosList.Count);
            int rangeEntityIndex = Random.Range(0, mAreaMapMonsterList.Count);

            AddMonster(mAreaMapMonsterList[rangeEntityIndex], createPosList[rangePos].x, createPosList[rangePos].z);
            mRemainingNumber++;

            Logger.Warning("敵が生成 {0}", mAreaMapMonsterList[rangeEntityIndex]);
        }

    }

    public void RemoveMonster(CharacterBase character)
    {
        characterList.Remove(character);
    }

    /// <summary>
    /// エリアマップで敵が生成する座標
    /// </summary>
    private List<Vector3Int> GetAreaMapCreateMonsterPosList()
    {
        int y = (int)player.transform.position.y;
        int minX = (int)(player.transform.position.x - SettingMng.AreaMapCreateMonsterRange);
        int minZ = (int)(player.transform.position.z - SettingMng.AreaMapCreateMonsterRange);
        int maxX = (int)(player.transform.position.x + SettingMng.AreaMapCreateMonsterRange);
        int maxZ = (int)(player.transform.position.z + SettingMng.AreaMapCreateMonsterRange);

        var torchArea = GetTorchAreaPosList();
        List<Vector3Int> list = new List<Vector3Int>();
        for (int z = minZ; z < maxZ; z++)
        {
            for (int x = minX; x < maxX; x++)
            {
                var newPos = new Vector3Int(x, y, z);

                // マップ範囲以外ならスキップ
                if (MapMng.IsOutRange(newPos))
                    continue;

                // 松明範囲内の場合、スキップ
                if (torchArea.Contains(newPos))
                    continue;

                list.Add(newPos);
            }
        }
        return list;
    }

    /// <summary>
    /// 松明エリアをゲット
    /// </summary>
    /// <returns></returns>
    private List<Vector3Int> GetTorchAreaPosList()
    {
        List<Vector3Int> list = new List<Vector3Int>();
        var torchList = WorldMng.E.MapMng.GetTorchs();
        foreach (var item in torchList)
        {
            int y = item.WorldPosition.y;
            int minX = item.WorldPosition.x - SettingMng.TorchRange;
            int minZ = item.WorldPosition.z - SettingMng.TorchRange;
            int maxX = item.WorldPosition.x + SettingMng.TorchRange;
            int maxZ = item.WorldPosition.z + SettingMng.TorchRange;

            for (int z = minZ; z < maxZ; z++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    var newPos = new Vector3Int(x, y, z);

                    // マップ範囲以外ならスキップ
                    if (MapMng.IsOutRange(newPos))
                        continue;

                    if (!list.Contains(newPos))
                        list.Add(newPos);
                }
            }

        }

        return list;
    }

    /// <summary>
    /// エリアマップで生成できるモンスターリスト
    /// </summary>
    /// <returns></returns>
    private List<int> GetAreaMapCreateMonsterEntityId()
    {
        List<int> list = new List<int>();
        foreach (var item in ConfigMng.E.CharacterGenerated.Values)
        {
            if (item.AreaMapActivation == 0)
                continue;

            // 主人公よりレベルが高いのは生成しない
            if (ConfigMng.E.Character[item.CharacterId].Level > LocalDataMng.E.Data.UserDataT.lv)
                continue;

            list.Add(item.ID);
        }
        return list;
    }

    #endregion

    #region 戦闘

    /// <summary>
    /// プレイヤーをゲット
    /// </summary>
    /// <returns></returns>
    public CharacterPlayer getPlayer()
    {
        return player;
    }

    /// <summary>
    /// 攻撃範囲内のキャラをゲット
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public List<CharacterBase> FindCharacterInRange(Transform startPoint, float distance, CharacterBase.CharacterGroup camp)
    {
        return FindCharacterInRange(startPoint.position, distance, camp);
    }
    public List<CharacterBase> FindCharacterInRange(Vector3 startPoint, float distance, CharacterBase.CharacterGroup camp)
    {
        List<CharacterBase> targets = new List<CharacterBase>();
        foreach (var item in characterList)
        {
            if (item == null)
                continue;

            if (!item.IsDied && InDistance(distance, startPoint, item.transform.position) && item.Group == camp)
            {
                targets.Add(item);
            }
        }

        return targets;
    }
    public List<CharacterBase> FindCharacterInRect(CharacterBase attacker, float maxDistance, float radius, CharacterBase.CharacterGroup camp)
    {
        List<CharacterBase> targets = new List<CharacterBase>();

        foreach (var item in characterList)
        {
            if (item == null || item.Group != camp || item.IsDied)
                continue;

            var result = CommonFunction.TargetPosInRect(attacker.FowardObj.transform.position, attacker.transform.position, item.transform.position, maxDistance, radius);
            if (result)
            {
                targets.Add(item);
            }
        }

        return targets;
    }
    public List<CharacterBase> FindCharacterAll(CharacterBase.CharacterGroup camp)
    {
        List<CharacterBase> targets = new List<CharacterBase>();

        foreach (var item in characterList)
        {
            if (item == null || item.Group != camp || item.IsDied)
                continue;

            targets.Add(item);
        }

        return targets;
    }

    /// <summary>
    /// 指定距離ないかの判定
    /// </summary>
    /// <param name="distanse">指定距離</param>
    /// <param name="target">目標</param>
    /// <returns></returns>
    public bool InDistance(float distanse, Vector3 start, Vector3 end)
    {
        return Vector3.Distance(start, end) <= distanse;
    }

    /// <summary>
    /// 2ポイント間の向き計算
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public Vector2 CalculationDir(Vector3 start, Vector3 end)
    {
        return new Vector2(start.x - end.x, start.z - end.z);
    }

    /// <summary>
    /// 範囲内にいる一番近い目標を探す
    /// </summary>
    /// <param name="camp">キャンプ</param>
    /// <param name="startPos">始点</param>
    /// <param name="distance">距離</param>
    public CharacterBase FindTargetInSecurityRange(CharacterBase.CharacterGroup camp, Vector3 startPos, float distance)
    {
        CharacterBase selected = null;
        float minDis = 0;
        foreach (var character in characterList)
        {
            if (character != null && !character.IsDied && character.Group == camp)
            {
                var dis = Mathf.Abs(Vector3.Distance(character.transform.position, startPos));

                // 範囲外ならスキップ
                if (dis > distance)
                    continue;

                // 始めの場合
                if (selected == null)
                {
                    selected = character;
                    minDis = dis;
                }

                // もっと近い目標がある場合
                if (dis < minDis)
                {
                    selected = character;
                    minDis = dis;
                }
            }
        }

        return selected;
    }

    /// <summary>
    /// 範囲内にいる全目標を探す
    /// </summary>
    /// <param name="camp">キャンプ</param>
    /// <param name="startPos">始点</param>
    /// <param name="distance">距離</param>
    /// <returns></returns>
    public List<CharacterBase> FindTargetListInSecurityRange(CharacterBase.CharacterGroup camp, Vector3 startPos, int distance)
    {
        List<CharacterBase> list = new List<CharacterBase>();
        foreach (var item in characterList)
        {
            if (item.IsDied)
                continue;

            if (Mathf.Abs(Vector3.Distance(item.transform.position, startPos)) <= distance
                && item.Group == camp)
            {
                list.Add(item);
            }
        }
        return list;
    }

    /// <summary>
    /// キャラクタクリア
    /// </summary>
    public void ClearCharacter()
    {
        characterList.Clear();
        TimeZoneMng.E.RemoveTimerEvent03(AddAreaMapMonster);
    }

    #endregion 
}