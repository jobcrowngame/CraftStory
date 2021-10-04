﻿using UnityEngine;

/// <summary>
/// 機能Object
/// </summary>
public class EntityFunctionalObject : EntityBase
{
    // ドアの状態（閉じっている）
    private bool DoorIsClosed
    {
        get => doorIsClosed;
        set
        {
            doorIsClosed = value;

            var collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.isTrigger = !value;
            }

            var openObj = CommonFunction.FindChiledByName(transform, "Open");
            if (openObj != null)
            {
                openObj.SetActive(!value);
            }

            var closeObj = CommonFunction.FindChiledByName(transform, "Close");
            if (closeObj != null)
            {
                closeObj.SetActive(value);
                collider.isTrigger = !value;
            }
        }
    }
    private bool doorIsClosed = true;

    /// <summary>
    /// 長い時間クリック終了場合のロジック
    /// </summary>
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        OnDestroyEntity();
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();

        

        switch (Type)
        {
            case EntityType.Door:
                DoorIsClosed = !DoorIsClosed;
                break;

            case EntityType.Workbench:
            case EntityType.Kamado:
                //var ui = UICtl.E.OpenUI<CraftUI>(UIType.Craft);
                //ui.SetType(Type);

                PlayerCtl.E.TalkToNPC(transform);
                break;

            case EntityType.Mission:
                UICtl.E.OpenUI<MissionUI>(UIType.Mission);
                break;

            case EntityType.ChargeShop:
                // 距離判定
                if (!CheckDistance())
                {
                    CommonFunction.ShowHintBar(30);
                    break;
                }

                var chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, 1);
                chatUi.AddListenerOnClose(() =>
                {
                    UICtl.E.OpenUI<ShopChargeUI>(UIType.ShopCharge);
                });
                break;

            case EntityType.GachaShop:
                // 距離判定
                if (!CheckDistance())
                {
                    CommonFunction.ShowHintBar(30);
                    break;
                }

                chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, 2);
                chatUi.AddListenerOnClose(() =>
                {
                    UICtl.E.OpenUI<ShopGachaUI>(UIType.ShopGacha);
                });
                break;

            case EntityType.ResourceShop:
                // 距離判定
                if (!CheckDistance())
                {
                    CommonFunction.ShowHintBar(30);
                    break;
                }

                chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, 3);
                chatUi.AddListenerOnClose(() =>
                {
                    UICtl.E.OpenUI<ShopResourceUI>(UIType.ShopResource);
                });
                break;

            case EntityType.BlueprintShop:
                // 距離判定
                if (!CheckDistance())
                {
                    CommonFunction.ShowHintBar(30);
                    break;
                }

                chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, 4);
                chatUi.AddListenerOnClose(() =>
                {
                    UICtl.E.OpenUI<ShopBlueprintUI>(UIType.ShopBlueprint);
                });
                break;

            case EntityType.GiftShop:
                // 距離判定
                if (!CheckDistance())
                {
                    CommonFunction.ShowHintBar(30);
                    break;
                }

                if (DataMng.E.RuntimeData.Coin3 < 1000)
                {
                    CommonFunction.ShowHintBar(18);
                    break;
                }

                chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, 5);
                chatUi.AddListenerOnClose(() =>
                {
                    UICtl.E.OpenUI<ExchangePointUI>(UIType.ExchangePoint);
                });
                break;

            default: Logger.Error("not find entityType " + Type); break;
        }
    }

    private bool CheckDistance()
    {
        return CommonFunction.GetDistance(transform.position, PlayerCtl.E.PlayerEntity.transform.position) < 3;
    }
}