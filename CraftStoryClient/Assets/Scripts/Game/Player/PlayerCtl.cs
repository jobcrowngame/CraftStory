using System;
using System.Collections;
using System.Collections.Generic;
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
                Character.StopMove();
                Character.Behavior = BehaviorType.Waiting;
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


    public HPUICtl HpBar;

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
            DataMng.E.RuntimeData.GuideId = 1;
            GuideLG.E.ReStart();
            CommonFunction.GoToNextScene(101);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GuideLG.E.GoTo(32);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            var area = new MapAreaData(4);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            UserSkill(new SkillData(1));
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            UserSkill(new SkillData(2));
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            UserSkill(new SkillData(3));
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
                }

                return;
            }
            else
            {
                switch ((ItemType)selectItem.Config().Type)
                {
                    case ItemType.Block:
                    case ItemType.Grass:
                    case ItemType.Lanthanum:
                    case ItemType.NomoObject:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config().ReferenceID, Vector3Int.CeilToInt(pos));
                        Character.Behavior = BehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Flower:
                    case ItemType.BigFlower:
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

                    case ItemType.BuilderPencil:
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
        NWMng.E.ClearMission(4, 1);
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
        yield return new WaitForSeconds(0.1f);
        Lock = false;
    }

    /// <summary>
    /// クリックしたObjectを向かう
    /// </summary>
    /// <param name="targetPos"></param>
    private void ChangePlayerDirection(Transform target, Action callback)
    {
        var dir = Character.GetTargetDircetion(target);

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
        if (Character.ShareCDIsCooling || skill.IsCooling)
            return;

        // 単体攻撃の場合、目標がないと先ず目標を探す
        if ((SkillData.SkillType)skill.Config.Type == SkillData.SkillType.SingleAttack)
        {
            Character.Target = CharacterCtl.E.FindTargetInSecurityRange(CharacterBase.CharacterCamp.Monster,
                Character.transform.position, skill.distance);

            // 探しても目標がない場合、スキップ
            if (Character.Target == null)
                return;
        }

        // 移動停止
        Character.StopMove();
        Character.StartUseSkill(skill, Character.Target);
    }

    #endregion
    #region 装備

    /// <summary>
    /// 装備してるアイテム
    /// </summary>
    Dictionary<int, ItemData> equipedItems = new Dictionary<int, ItemData>();

    /// <summary>
    /// 装備する
    /// </summary>
    /// <param name="itemId"></param>
    public void EquipEquipment(ItemData itemData)
    {
        // 装備してるとスキップ
        if (itemData.equipSite > 0)
            return;

        // 装備じゃない場合、スキップ
        if (!CommonFunction.IsEquipment(itemData.itemId))
            return;

        var itemConfig = ConfigMng.E.Item[itemData.itemId];
        var equipmentConfig = ConfigMng.E.Equipment[itemConfig.ReferenceID];

        // Equipmentが装備してる場合、先ず装備してるEquipmentを消す
        if (equipedItems.ContainsKey(itemConfig.Type))
        {
            // Equipmentを消す
            NWMng.E.EquitItem(null, equipedItems[itemConfig.Type].id, 0);
            Character.RemoveEquipment(equipmentConfig.ID);
            equipedItems[itemConfig.Type].equipSite = 0;
        }

        Character.EquipEquipment(equipmentConfig.ID);
        equipedItems[itemConfig.Type] = itemData;

        // 装備する
        int equipSite = (int)CommonFunction.GetEquipSite((ItemType)itemData.Config().Type);
        itemData.equipSite = equipSite;
        NWMng.E.EquitItem((rp) => { BagLG.E.UI.RefreshItems(); }, equipedItems[itemConfig.Type].id, equipSite);

        //CommonFunction.ShowHintBar(31);

        BagLG.E.UI.RefreshItems();
    }

    #endregion
}