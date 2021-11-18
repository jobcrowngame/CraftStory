using System;
using System.Collections;
using System.Collections.Generic;
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
                entity = UICtl.E.CreateGlobalObject<PlayerCtl>();
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
                    Character.StopMove();
                    Character.Behavior = BehaviorType.Waiting;
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

        selectItem = item;
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
                switch ((ItemType)selectItem.Config().Type)
                {
                    case ItemType.Block:
                    case ItemType.Lanthanum:
                    case ItemType.NomoObject:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config().ReferenceID, Vector3Int.CeilToInt(pos));
                        Character.Behavior = BehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Workbench:
                    case ItemType.Kamado:
                    case ItemType.Door:
                    case ItemType.Mission:
                    case ItemType.HaveDirectionNomoObject:
                        Lock = true;
                        var direction = CommonFunction.GetCreateEntityDirection(pos);
                        CreateEntity(collider, selectItem.Config().ReferenceID, Vector3Int.CeilToInt(pos), direction);
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
                                BuilderPencil.UseBlueprint(Vector3Int.CeilToInt(pos), selectItem.relationData);
                            });
                        }
                        break;

                    case ItemType.Torch:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config().ReferenceID, Vector3Int.CeilToInt(pos), dType);
                        Character.Behavior = BehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    default: Logger.Error("Not find ItemType " + (ItemType)selectItem.Config().Type); break;
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

        if (DataMng.E.RuntimeData.MapType == MapType.Home)
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
    private void CreateEntity(GameObject collider, int entityId, Vector3Int pos, Direction dType = Direction.up)
    {
        var cell = collider.GetComponent<EntityBase>();
        if (cell == null)
            return;

        // ターゲットEntityに置くできるかのチェック
        if (cell.EConfig.CanPut == 0)
        {
            CommonFunction.ShowHintBar(19);
            return;
        }

        WorldMng.E.MapCtl.CreateEntity(entityId, pos, dType);

        // ブロックやオブジェクトを置くミッション
        if (DataMng.E.RuntimeData.MapType != MapType.Guide)
        {
            NWMng.E.ClearMission(4, 1);

            DataMng.E.RuntimeData.TotalSetBlockCount++;
            if (DataMng.E.RuntimeData.TotalSetBlockCount >= 100 && DataMng.E.RuntimeData.GuideEnd == 0)
            {
                // 設計図チュートリアルへ招待
                var chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, 7);
                chatUi.AddListenerOnClose(() =>
                {
                    DataMng.E.RuntimeData.GuideId = 1;
                    CommonFunction.GoToNextScene(105);
                });
            }
        }
    }

    /// <summary>
    /// アイテムを使用
    /// </summary>
    /// <param name="count"></param>
    public void UseItem(int count = 1)
    {
        DataMng.E.UseItem(selectItem.id, count);
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
            ret = DataMng.E.CheckConsumableItem(item.Key, item.Value);

            if (ret == false)
                break;
        }

        foreach (var item in items)
        {
            ret = DataMng.E.ConsumableItem(item.Key, item.Value);

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


    #region 戦闘

    /// <summary>
    /// スキルを使う
    /// </summary>
    /// <param name="skill"></param>
    public void UserSkill(SkillData skill)
    {
        if (Character.ShareCDIsCooling || skill.IsCooling || Character.IsDied)
            return;

        // 単体攻撃、遠距離範囲攻撃の場合、目標がないと先ず目標を探す
        if ((SkillData.SkillType)skill.Config.Type == SkillData.SkillType.SingleAttack 
            || (SkillData.SkillType)skill.Config.Type == SkillData.SkillType.RangedRangeAttack)
        {
            // 目標がない場合、探す
            if (Character.Target == null)
                Character.Target = CharacterCtl.E.FindTargetInSecurityRange(CharacterBase.CharacterCamp.Monster,
                    Character.transform.position, skill.distance);

            // 探しても目標がない場合、スキップ
            if (Character.Target == null)
            {
                CommonFunction.ShowHintBar(31);
                return;
            }
        }

        // 移動停止
        Character.StopMove();
        Character.StartUseSkill(skill, Character.Target);


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

            // 武器を装備してる場合、タスクが完了とする
            if (Character.IsEquipedEquipment())
                TaskMng.E.AddMainTaskCount(2);
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
        if (EquipedItems.ContainsKey(itemData.Config().Type))
        {
            Unequipment(() =>
            {
                Equipment(itemData);
            }, itemData.Config().Type);
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
        int equipSite = (int)CommonFunction.GetEquipSite((ItemType)itemData.Config().Type);
        NWMng.E.EquitItem((rp) =>
        {
            itemData.equipSite = equipSite;

            OnEquipment(itemData, equipSite);
        }, itemData.id, equipSite);
    }
    private void OnEquipment(ItemEquipmentData itemData, int equipSite)
    {
        Character.EquipEquipment(itemData);
        EquipedItems[itemData.Config().Type] = itemData;
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
        EquipedItems[itemData.Config().Type] = itemData;

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