using System;
using System.Collections;
using System.Collections.Generic;
using JsonConfigData;
using LitJson;
using SimpleInputNamespace;
using UnityEngine;

/// <summary>
/// プレイヤーのコンソール
/// </summary>
public class PlayerCtl : MonoBehaviour
{
    public static PlayerCtl E
    {
        get
        {
            if (entity == null)
            {
                entity = CommonFunction.CreateGlobalObject<PlayerCtl>();
                entity.Init();
            }

            return entity;
        }
    }
    private static PlayerCtl entity;

    public bool Lock { get; set; } // ロック

    public CharacterPlayer Character { get; private set; }
    public CharacterFollow Fairy { get; set; }

    public Joystick Joystick
    {
        get => joystick;
        set
        {
            joystick = value;

            // Joystickをスクロールイベント
            Joystick.AddListionOnDrag(() =>
            {
                if (Lock || Character == null)
                    return;

                Character.Move(joystick.xAxis.value, joystick.yAxis.value);
            });

            // Joystickスクロールが停止場合のイベント
            Joystick.AddListionOnPointerUp(() =>
            {
                if (Character != null && !Character.IsDied)
                {
                    if (!Character.CanNotChangeBehavior())
                    {
                        Character.StopMove();
                        Character.Behavior = BehaviorType.Waiting;
                    }
                }
            });
        }
    }
    private Joystick joystick; // ジョイスティック

    public ScreenDraggingCtl ScreenDraggingCtl { get => screenDraggingCtl; set => screenDraggingCtl = value; }
    private ScreenDraggingCtl screenDraggingCtl; // 画面操作コンソール

    public BuilderPencil BuilderPencil { get => builderPencil; }
    private BuilderPencil builderPencil; // ビルダーペンセルコンソール

    public ItemData SelectItem 
    {
        get => selectItem; 
        set => selectItem = value;
    }
    private ItemData selectItem; // 選択したアイテム
    private EntityBase clickingEntity; // 長い時間クリックしてるエンティティ

    public CameraCtl CameraCtl
    {
        get => cameraCtl;
        set
        {
            cameraCtl = value;
            cameraCtl.Init();
        }
    }
    private CameraCtl cameraCtl; // カメラコンソール

    public BlueprintPreviewCtl BlueprintPreviewCtl
    {
        get => blueprintPreviewCtl;
        set
        {
            blueprintPreviewCtl = value;
        }
    }
    private BlueprintPreviewCtl blueprintPreviewCtl; // 設計図プレイビューコンソール

    public void Init()
    {
        builderPencil = new BuilderPencil();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Character.Jump();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            CommonFunction.GoToNextScene(0);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            CommonFunction.GotoAreaMap();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (Character.Target != null)
            {
                Character.AddFireBool(Character.Target);
            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            HomeLG.E.UI.ClickSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            HomeLG.E.UI.ClickSkill(1);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            HomeLG.E.UI.ClickSkill(2);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            HomeLG.E.UI.ClickSkill(3);
        }
    }

    public void SetCharacter(CharacterPlayer character)
    {
        Character = character;
    }

    public void Jump()
    {
        Character.Jump();
    }

    /// <summary>
    /// 選択したアイテムを変換
    /// </summary>
    /// <param name="item"></param>
    public void ChangeSelectItem(ItemData item)
    {
        builderPencil.CancelCreateBlueprint();
        builderPencil.CancelUserBlueprint();

        SelectItem = item;
    }

    /// <summary>
    /// クリックした場合のロジック
    /// </summary>
    /// <param name="collider">クリックしたGameObject</param>
    /// <param name="pos">クリックした座標</param>
    /// <param name="dType">クリックした向き</param>
    public void OnClick(GameObject collider, Vector3 pos, Direction dType)
    {
        // プレビューの場合、クリックを無視
        if (DataMng.E.RuntimeData.IsPreviev)
            return;

        // ホーム、チュートリアルの場合
        if (DataMng.E.RuntimeData.MapType == MapType.Home
            || DataMng.E.RuntimeData.MapType == MapType.AreaMap
            || DataMng.E.RuntimeData.MapType == MapType.Guide
            || DataMng.E.RuntimeData.MapType == MapType.Market)
        {
            if (selectItem == null)
            {
                if (collider != null)
                {
                    var entity = collider.GetComponent<EntityBase>();
                    if(entity != null) entity.OnClick();

                    // キャラをタップ
                    var character = collider.GetComponent<CharacterBase>();
                    if (character != null) character.OnClick();
                }

                return;
            }
            else
            {
                switch ((ItemType)selectItem.Config.Type)
                {
                    case ItemType.Block:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config.ReferenceID, Vector3Int.CeilToInt(pos));
                        Character.Behavior = BehaviorType.Create;

                        WorldMng.E.MapCtl.CombineMesh();

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Block2:
                        Lock = true;
                        var direction = CommonFunction.GetCreateEntityDirection(pos);
                        CreateEntity(collider, selectItem.Config.ReferenceID, Vector3Int.CeilToInt(pos), direction);
                        Character.Behavior = BehaviorType.Create;

                        WorldMng.E.MapCtl.CombineMesh();

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Crops:
                        Lock = true;
                        Character.Behavior = BehaviorType.Create;
                        direction = CommonFunction.GetCreateEntityDirection(pos);
                        CreateEntity(collider, ConfigMng.E.Crops[selectItem.Config.ReferenceID].EntityID, Vector3Int.CeilToInt(pos), direction);

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Lanthanum:
                    case ItemType.NomoObject:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config.ReferenceID, Vector3Int.CeilToInt(pos));
                        Character.Behavior = BehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Blast:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config.ReferenceID, Vector3Int.CeilToInt(pos));
                        Character.Behavior = BehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Workbench:
                    case ItemType.Kamado:
                    case ItemType.EquipmentWorkbench:
                    case ItemType.CookingTable:
                    case ItemType.Door:
                    case ItemType.Mission:
                    case ItemType.HaveDirectionNomoObject:
                    case ItemType.Bed:
                        Lock = true;
                        direction = CommonFunction.GetCreateEntityDirection(pos);
                        CreateEntity(collider, selectItem.Config.ReferenceID, Vector3Int.CeilToInt(pos), direction);
                        Character.Behavior = BehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.NullBlueprint:
                        var newPos = collider.transform.position;
                        newPos.y += 1;

                        if (builderPencil.IsStart)
                            builderPencil.Start(newPos);
                        else
                            builderPencil.End(newPos);
                        break;

                    case ItemType.Blueprint:
                        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
                        {
                            BuilderPencil.UseBlueprint(Vector3Int.CeilToInt(pos), selectItem.relationData);
                        }
                        else
                        {
                            NWMng.E.GetItemRelationData(selectItem.id, selectItem, () =>
                            {
                                BuilderPencil.UseBlueprint(Vector3Int.CeilToInt(pos), selectItem.relationData, selectItem.IsLocked);
                            });
                        }
                        break;

                    case ItemType.Torch:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config.ReferenceID, Vector3Int.CeilToInt(pos), dType);
                        Character.Behavior = BehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Hoe:
                        if (collider != null)
                        {
                            var entity = collider.GetComponent<EntityBase>();
                            if (entity != null) entity.OnClick();
                        }
                        break;

                    default: Logger.Error("Not find ItemType " + (ItemType)selectItem.Config.Type); break;
                }
            }
        }
        // 冒険の場合
        else
        {
            if (collider != null)
            {
                // 資源をタップ
                var entity = collider.GetComponent<EntityBase>();
                if (entity != null) entity.OnClick();

                // キャラをタップ
                var character = collider.GetComponent<CharacterBase>();
                if (character != null) character.OnClick();
            }
        }
    }

    /// <summary>
    /// 長い時間クリックロジック
    /// </summary>
    /// <param name="time"></param>
    /// <param name="collider"></param>
    public void OnClicking(float time, GameObject collider)
    {
        if (collider == null)
            return;

        if (DataMng.E.RuntimeData.MapType == MapType.Home 
            || DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            var baseCell = collider.GetComponent<EntityBase>();
            if (baseCell != null)
            {
                if (baseCell.EConfig.CanDestroy == 0)
                    return;

                if (clickingEntity == null || clickingEntity != baseCell)
                {
                    clickingEntity = baseCell;
                    clickingEntity.CancelClicking();
                }
                else
                {
                    Character.Behavior = BehaviorType.Breack;
                    baseCell.OnClicking(time);
                }
            }
        }
    }

    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="collider">クリックしたGameObject</param>
    /// <param name="entityId">インスタンスするエンティティID</param>
    /// <param name="pos">インスタンス座標</param>
    /// <param name="dType">向き</param>
    private EntityBase CreateEntity(GameObject collider, int entityId, Vector3Int pos, Direction dType = Direction.up)
    {
        var cell = collider.GetComponent<EntityBase>();
        if (cell == null)
            return null;

        // ターゲットEntityに置くできるかのチェック
        if (cell.EConfig.CanPut == 0)
        {
            CommonFunction.ShowHintBar(19);
            return null;
        }

        EntityBase entity;
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            entity = WorldMng.E.MapMng.CreateEntity(entityId, pos, dType);
        }
        else
        {
            entity = WorldMng.E.MapCtl.CreateEntity(entityId, pos, dType);
        }

        // ブロックやオブジェクトを置くミッション
        if (entity != null && DataMng.E.RuntimeData.MapType != MapType.Guide)
        {
            NWMng.E.ClearMission(4, 1);

            DataMng.E.RuntimeData.TotalSetBlockCount++;
            if (DataMng.E.RuntimeData.TotalSetBlockCount >= 100 && DataMng.E.RuntimeData.GuideEnd == 0)
            {
                // 設計図チュートリアルへ招待
                var chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.OnCloseDestroyObj, 7);
                chatUi.AddListenerOnClose(() =>
                {
                    DataMng.E.RuntimeData.GuideId = 1;
                    CommonFunction.GoToNextScene(105);
                });
            }
        }

        return entity;
    }

    /// <summary>
    /// アイテムを使用
    /// </summary>
    /// <param name="count"></param>
    public void UseSelectItem(int count = 1)
    {
        DataMng.E.ConsumableItemByGUID(selectItem.id, count);
    }

    /// <summary>
    /// アイテムを消耗
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public bool ConsumableItems(Dictionary<int, int> items)
    {
        bool ret = false;

        foreach (var item in items)
        {
            ret = DataMng.E.CheckConsumableItemByItemId(item.Key, item.Value);

            if (ret == false)
                break;
        }

        foreach (var item in items)
        {
            ret = DataMng.E.ConsumableItemByItemId(item.Key, item.Value);

            if (ret == false)
                break;
        }

        return ret;
    }

    /// <summary>
    /// NPC と話す
    /// </summary>
    public void TalkToNPC(Transform target, Action callback)
    {
        if (NPCTTalkDistanceChect(target))
        {
            ChangePlayerDirection(target, callback);
        }
        else
        {
            Character.PlayerMoveTo(target.position, SettingMng.NPCTTalkDistance, () =>
            {
                ChangePlayerDirection(target, callback);
            });
        }
    }

    /// <summary>
    /// ロックを解除
    /// </summary>
    /// <returns></returns>
    private IEnumerator UnLock()
    {
        yield return new WaitForSeconds(0.2f);
        Lock = false;
        Character.Behavior = BehaviorType.Waiting;
    }

    /// <summary>
    /// クリックしたObjectを向かう
    /// </summary>
    /// <param name="targetPos"></param>
    private void ChangePlayerDirection(Transform target, Action callback)
    {
        var dir = Character.GetDircetion(target);

        Character.Rotation(dir);
        cameraCtl.CameraslowlyRotateToTarget(dir, callback);
    }

    /// <summary>
    /// NPC話す距離をチェック
    /// </summary>
    /// <returns></returns>
    public bool NPCTTalkDistanceChect(Transform target)
    {
        var distance = CommonFunction.GetDistance(target.position, Character.transform.position);
        return distance < SettingMng.NPCTTalkDistance;
    }

    #region 普通

    public void EatFood(ItemData item)
    {
        if (HomeLG.E.UI != null)
        {
            Lock = true;

            // 回復量計算
            Food food = ConfigMng.E.Food[item.itemId];
            int v = food.Amount + (int)(SettingMng.MaxHunger * food.Percent * 0.01f);

            // 空腹度を回復
            HomeLG.E.UI.RecoveryHunger(v);

            // アイテムを消耗
            DataMng.E.ConsumableItemByGUID(item.id, 1);

            StartCoroutine(EatFoodIE());
        }
    }
    private IEnumerator EatFoodIE()
    {
        Character.Behavior = BehaviorType.EatFood;

        yield return new WaitForSeconds(2f);

        Character.Behavior = BehaviorType.Waiting;
        Lock = false;
    }

    #endregion

    #region 戦闘

    /// <summary>
    /// スキルを使う
    /// </summary>
    /// <param name="skill"></param>
    public void UserSkill(SkillData skill, Action<SkillData> successCallback)
    {
        if (Character.ShareCDIsCooling || skill.IsCooling || Character.IsDied || Character.SkillUsing)
            return;

        // 単体攻撃、遠距離範囲攻撃の場合、目標がないと先ず目標を探す
        if ((SkillData.SkillType)skill.Config.Type == SkillData.SkillType.SingleAttack 
            || (SkillData.SkillType)skill.Config.Type == SkillData.SkillType.RangedRangeAttack)
        {
            // 目標がない場合、探す
            if (Character.Target == null)
                Character.Target = WorldMng.E.CharacterCtl.FindTargetInSecurityRange(CharacterBase.CharacterGroup.Monster,
                    Character.transform.position, skill.distance);

            // 探しても目標がない場合、スキップ
            if (Character.Target == null)
            {
                CommonFunction.ShowHintBar(31);
                return;
            }
        }

        // 移動停止
        //Character.StopMove();
        Character.StartUseSkill(CharacterBase.CharacterGroup.Monster, skill, Character.Target);

        foreach (var item in Character.SkillList)
        {
            if (item.Config.ID == skill.Config.NextSkill && successCallback != null)
            {
                successCallback(item);
            }
        }

        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
        {
            GuideLG.E.NextWithSkill(skill);
        }
    }

    /// <summary>
    /// Exp 追加
    /// </summary>
    public void AddExp(int exp)
    {
        Character.AddExp(exp);
    }

    /// <summary>
    /// 一時停止
    /// </summary>
    /// <param name="pause"></param>
    public void Pause(bool pause = true)
    {
        //Time.timeScale = pause ? 0 : 1;
    }

    #endregion
    #region 装備

    /// <summary>
    /// 装備してるアイテム（equipType, itemData）
    /// </summary>
    Dictionary<int, ItemEquipmentData> equipedItems = new Dictionary<int, ItemEquipmentData>();
    Dictionary<int, ItemEquipmentData> guideGquipedItems = new Dictionary<int, ItemEquipmentData>();
    Dictionary<int, ItemEquipmentData> EquipedItems 
    {
        get
        {
            return DataMng.E.RuntimeData.MapType == MapType.Guide ? guideGquipedItems : equipedItems;
        }
    }

    /// <summary>
    /// アイテムタイプによって装備してる装備をゲット
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public ItemEquipmentData GetEquipByItemType(ItemType itemType)
    {
        if (EquipedItems == null)
        {
            Logger.Error("equipedItems is null");
        }
        if (EquipedItems.ContainsKey((int)itemType))
        {
            return EquipedItems[(int)itemType];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 装備をEquip
    /// </summary>
    public void EquipEquipments()
    {
        NWMng.E.GetEquipmentInfoList((rp) =>
        {
            if (string.IsNullOrEmpty(rp.ToString()))
                return;
            else
            {
                var result = JsonMapper.ToObject<List<EquipListLG.EquipListRP>>(rp.ToJson());
                foreach (var r in result)
                {
                    var item = DataMng.E.GetItemByGuid(r.id);
                    if (item != null && item.equipSite > 0)
                    {
                        OnEquipment(new ItemEquipmentData(r), item.equipSite);
                    }
                }
            }

            Character.Parameter.Refresh();
            if(HomeLG.E.UI != null) HomeLG.E.UI.ShowSpriteAnimation();

            //// 武器を装備してる場合、タスクが完了とする
            //if (Character.IsEquipedEquipment())
            //    TaskMng.E.AddMainTaskCount(2);

            // チュートリアルマップ以外、クラフトチュートリアル完了、現在武器装備してない場合
            // デフォルトで10001を装備する
            if (DataMng.E.RuntimeData.MapType != MapType.Guide &&
                DataMng.E.RuntimeData.GuideEnd3 == 1 &&
                PlayerCtl.E.GetEquipByItemType(ItemType.Weapon) == null)
            {
                // チュートリアルが完了後、デフォルトで武器を鑑定して装備
                var item = DataMng.E.GetItemByItemId(10001);
                if (item != null)
                {
                    var equipment = new ItemEquipmentData(item);
                    NWMng.E.AppraisalEquipment((rp) =>
                    {
                        equipment.islocked = 1;
                        equipment.SetAttachSkills((string)rp);

                        NWMng.E.EquitItem((rp) =>
                        {
                            PlayerCtl.E.EquipEquipment(equipment);
                        }, item.id, 101);
                    }, item.id, 1);
                }
            }
        });
    }

    /// <summary>
    /// 装備する
    /// </summary>
    /// <param name="itemId"></param>
    public void EquipEquipment(ItemEquipmentData itemData)
    {
        // 装備してるとスキップ
        if (itemData.equipSite > 0)
        {
            CommonFunction.ShowHintBar(34);
            return;
        }

        // 装備じゃない場合、スキップ
        if (!CommonFunction.IsEquipment(itemData.itemId))
            return;

        // Equipmentが装備してる場合、先ず装備してるEquipmentを消す
        if (EquipedItems.ContainsKey(itemData.Config.Type))
        {
            Unequipment(() =>
            {
                Equipment(itemData);
            }, itemData.Config.Type);
        }
        else
        {
            Equipment(itemData);
        }
    }

    /// <summary>
    /// 装備解除
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="equipType"></param>
    private void Unequipment(Action callback, int equipType)
    {
        if (EquipedItems.ContainsKey(equipType))
        {
            // Equipmentを消す
            NWMng.E.EquitItem((rp) =>
            {

                // 装備解除
                DataMng.E.GetItemByGuid(EquipedItems[equipType].id).equipSite = 0;
                Character.RemoveEquipment(EquipedItems[equipType]);

                EquipedItems.Remove(equipType);

                if (callback != null)
                    callback();
            }, EquipedItems[equipType].id, 0);
        }
    }

    /// <summary>
    /// 装備
    /// </summary>
    /// <param name="itemData"></param>
    private void Equipment(ItemEquipmentData itemData)
    {
        // 装備する
        int equipSite = (int)CommonFunction.GetEquipSite((ItemType)itemData.Config.Type);
        NWMng.E.EquitItem((rp) =>
        {
            itemData.equipSite = equipSite;

            OnEquipment(itemData, equipSite);
        }, itemData.id, equipSite);
    }
    private void OnEquipment(ItemEquipmentData itemData, int equipSite)
    {
        Character.EquipEquipment(itemData);
        EquipedItems[itemData.Config.Type] = itemData;
        DataMng.E.GetItemByGuid(itemData.id).equipSite = equipSite;

        // 持ち物アイテム更新
        if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();

        // ホームのスキル更新
        if (HomeLG.E.UI != null) HomeLG.E.UI.SetSkills();

        // 装備Window更新
        if (EquipLG.E.UI != null)
        {
            EquipLG.E.UI.RefreshEquip();
            EquipLG.E.UI.RefreshParameter();
        }
    }

    public void EquipGuideEquipment(ItemEquipmentData itemData)
    {
        Character.EquipEquipment(itemData);
        EquipedItems[itemData.Config.Type] = itemData;

        // ホームのスキル更新
        if (HomeLG.E.UI != null) HomeLG.E.UI.SetSkills();
    }

    /// <summary>
    /// 装備してるアイテムゲット
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, ItemEquipmentData> GetEquipedItems()
    {
        return EquipedItems;
    }

    #endregion
}