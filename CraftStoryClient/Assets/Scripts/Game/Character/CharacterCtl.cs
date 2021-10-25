using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクタコンソール
/// </summary>
public class CharacterCtl : Single<CharacterCtl>
{
    private CharacterPlayer player;
    private List<CharacterBase> characterList = new List<CharacterBase>();

    public void CreateCharacter()
    {
        AddPlayer();
        AddMonsters();
    }

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
        Vector3 pos = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 0);
        pos = MapCtl.FixEntityPos(DataMng.E.MapData, pos, DataMng.E.MapData.Config.CreatePosOffset);
        pos.y += 3;
        var obj = GameObject.Instantiate(resource, pos, Quaternion.identity);
        if (obj == null)
        {
            Logger.Error("Player Instantiate fail");
            return;
        }

        player = obj.GetComponent<CharacterPlayer>();
        player.Init(1, CharacterBase.CharacterCamp.Player);
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
        foreach (var item in characterGenerated)
        {
            if (item == "N")
                continue;

            // モンスターをインスタンス
            var characterGeneratedCfg = ConfigMng.E.CharacterGenerated[int.Parse(item)];
            var characterCfg = ConfigMng.E.Character[characterGeneratedCfg.CharacterId];

            var resource = Resources.Load(characterCfg.Prefab) as GameObject;
            if (resource == null)
            {
                Logger.Error("not find Monster Prefabs： {0}", characterCfg.Prefab);
                return;
            }

            // 生成する座標ゲット
            Vector3 pos = MapCtl.GetGroundPos(DataMng.E.MapData, characterGeneratedCfg.PosX, characterGeneratedCfg.PosZ, 0);
            pos = MapCtl.FixEntityPos(DataMng.E.MapData, pos, DataMng.E.MapData.Config.CreatePosOffset);
            pos.y += 3;

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

            monster.Init(characterCfg.ID, CharacterBase.CharacterCamp.Monster);
            monster.SetHpBar(CommonFunction.FindChiledByName<HpUIBase>(monster.transform, "WorldUI"));
            characterList.Add(monster);
        }
    }

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
    public List<CharacterBase> FindCharacterInRange(Transform startPoint, float distance, CharacterBase.CharacterCamp camp)
    {
        return FindCharacterInRange(startPoint.position, distance, camp);
    }
    public List<CharacterBase> FindCharacterInRange(Vector3 startPoint, float distance, CharacterBase.CharacterCamp camp)
    {
        List<CharacterBase> targets = new List<CharacterBase>();
        foreach (var item in characterList)
        {
            if (!item.IsDied && InDistance(distance, startPoint, item.transform.position) && item.Camp == camp)
            {
                targets.Add(item);
            }
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
    public CharacterBase FindTargetInSecurityRange(CharacterBase.CharacterCamp camp, Vector3 startPos, float distance)
    {
        CharacterBase selected = null;
        float minDis = 0;
        foreach (var character in characterList)
        {
            if (!character.IsDied && character.Camp == camp)
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
    public List<CharacterBase> FindTargetListInSecurityRange(CharacterBase.CharacterCamp camp, Vector3 startPos, int distance)
    {
        List<CharacterBase> list = new List<CharacterBase>();
        foreach (var item in characterList)
        {
            if (item.IsDied)
                continue;

            if (Mathf.Abs(Vector3.Distance(item.transform.position, startPos)) <= distance
                && item.Camp == camp)
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
    }

    #endregion 
}