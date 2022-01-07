using JsonConfigData;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using static MapData;

class GuideLG : UILogicBase<GuideLG, GuideUI>
{
    private Guide config { get => ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId]; }
    private string[] guideSteps;

    /// <summary>
    /// チュートリアルが完了してるかのフラグ
    /// </summary>
    public bool end { get; set; }

    public Vector3Int TransferGateSite { get; set; }
    public RuntimeData HomeRuntimeData { get; set; }

    private int itemGuid = 1;
    private int createBlockCount = 0;
    private bool Lock 
    {
        get => mLock; 
        set
        {
            if (DataMng.E.RuntimeData.MapType != MapType.Guide)
                return;

            mLock = value;
            UI.ShowFullMask(mLock);
        }
    }
    private bool mLock;

    private string CurStep
    {
        set
        {
            UI.NextStep(int.Parse(value));
        }
    }
    private string mCurStep;

    public int Step { get => stepIndex; }

    private int stepIndex = 0;

    public override void Init(GuideUI ui)
    {
        base.Init(ui);

        end = false;
        guideSteps = config.StepList.Split(',');
    }
    public void ReStart()
    {
        end = false;
        stepIndex = 1;
        itemGuid = 1;
        createBlockCount = 0;
    }
    public void UnLock()
    {
        Lock = false;
    }

    /// <summary>
    // ガイドでリソースがクリックされた場合呼び出される
    /// </summary>
    public void OnClickEntityResource(EntityResources entity)
    {
        // ClickObject設定時、クリックしたパスとあっていない場合はクリック無効とする
        string objPath = ConfigMng.E.GuideStep[int.Parse(guideSteps[stepIndex - 1])].ClickObject;
        if (objPath != null && objPath != "N")
        {
            GameObject obj = UI.GetGameObject(objPath);
            if(obj.transform != entity.transform)
            {
                return;
            }
        }

        var effect = EffectMng.E.AddEffect<EffectBase>(entity.transform.position, EffectType.ResourcesDestroy);
        AdventureCtl.E.AddBonus(entity.EConfig.BonusID);
        effect.Init();
        WorldMng.E.MapCtl.DeleteEntity(entity);

        Next();
    }

    /// <summary>
    // 通常のNext
    /// </summary>
    public void Next()
    {
        if (!CanNext())
            return;

        // CreateBlockCount設定時は通常のNextをはじく
        if (stepIndex != 0)
        {
            string createBlockCountCfg = ConfigMng.E.GuideStep[int.Parse(guideSteps[stepIndex - 1])].CreateBlockCount;
            if (createBlockCountCfg != null && createBlockCountCfg != "N")
                return;
        }

        DoNext();
    }

    /// <summary>
    // エンティティクリックでのNext
    /// </summary>
    public void NextOnClickEntity(EntityType type)
    {
        if (!CanNext())
            return;

        string clickType = ConfigMng.E.GuideStep[int.Parse(guideSteps[stepIndex - 1])].ClickType;
        if (clickType == null || clickType == "N")
            return;

        if ((EntityType)int.Parse(clickType) != type)
            return;

        DoNext();
    }

    /// <summary>
    /// ブロック作成時のNext
    /// チュートリアル中作成したブロック数を記録
    /// </summary>
    public void NextOnCreateBlock()
    {
        if (!CanNext())
            return;

        string createBlockCountCfg = ConfigMng.E.GuideStep[int.Parse(guideSteps[stepIndex - 1])].CreateBlockCount;
        if (createBlockCountCfg == null || createBlockCountCfg == "N")
            return;

        createBlockCount++;
        if (createBlockCount == int.Parse(createBlockCountCfg))
        {
            createBlockCount = 0;
            DoNext();
        }
    }

    /// <summary>
    /// スキル発動時のNext
    /// 指定したスキルでない場合Nextにしない
    /// </summary>
    public void NextWithSkill(SkillData skill)
    {
        if (!CanNext())
            return;

        string skillCfg = ConfigMng.E.GuideStep[int.Parse(guideSteps[stepIndex - 1])].Skill;
        if (skillCfg == "N")
            return;

        int skillCfgInt = int.Parse(skillCfg);
        if (skillCfgInt != skill.Config.ID)
            return;

        DoNext();
    }

    /// <summary>
    /// エラー発生などで指定のステップに戻す際に使用する
    /// ※GuideにFailCheckIndexListとRollbackIndexListを設定する必要あり、未設定時は只のNextになる

    /// </summary>
    public void Rollback()
    {
        if (!CanNext())
            return;

        int failCheckIndex = Array.IndexOf(config.FailCheckIndexList.Split(','), stepIndex.ToString());
        if (failCheckIndex != -1)
        {
            int rollbackIndex = int.Parse(config.RollbackIndexList.Split(',')[failCheckIndex]);
            stepIndex = rollbackIndex - 1;
        }

        DoNext();
    }

    /// <summary>
    /// クリアする
    /// </summary>
    public void Clear()
    {
        end = false;
        Lock = false;
        stepIndex = 0;
        createBlockCount = 0;
    }

    public void SetGuideItems()
    {
        var items = ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId].ItemList.Split(',');
        var counts = ConfigMng.E.Guide[DataMng.E.RuntimeData.GuideId].ItemCount.Split(',');

        DataMng.E.GuideItems.Clear();
        for (int i = 0; i < items.Length; i++)
        {
            if (int.Parse(items[i]) == 3002)
            {
                AddBlueprint();
            }
            else
            {
                AddGuideItem(int.Parse(items[i]), int.Parse(counts[i]));
            }
        }
    }
    public void AddGuideItem(int itemId, int count)
    {
        AddGuideItem(new ItemData(itemId, count));
    }
    public void AddGuideItem(ItemData itemData)
    {
        itemData.id = itemGuid++;
        DataMng.E.GuideItems.Add(itemData);
    }
    private void AddBlueprint()
    {
        var config = ConfigMng.E.Blueprint[1];
        AddGuideItem(new ItemData()
        {
            itemId = 3002,
            count = 1,
            newName = "ハウス",
            relationData = config.Data,
            equipSite = 1,
        });
    }

    private bool CanNext()
    {
        if (end || Lock)
            return false;

        if (DataMng.E.RuntimeData.MapType != MapType.Guide)
            return false;

        return true;
    }

    private void DoNext()
    {
        CurStep = guideSteps[stepIndex];
        stepIndex++;
        Lock = true;
    }

    /*
    ここからはGuideLGMethodListの設定で呼び出すメソッド     
    */

    public void PreserveRuntimeData()
    {
        HomeRuntimeData = new RuntimeData();
        HomeRuntimeData.Lv = DataMng.E.RuntimeData.Lv;
        HomeRuntimeData.Exp = DataMng.E.RuntimeData.Exp;
        HomeRuntimeData.Coin1 = DataMng.E.RuntimeData.Coin1;
        HomeRuntimeData.Coin2 = DataMng.E.RuntimeData.Coin2;
        HomeRuntimeData.Coin3 = DataMng.E.RuntimeData.Coin3;
    }

    public void InitGuideEquip()
    {
        var data = DataMng.E.GetItemByItemId(10004);
        var equip = new ItemEquipmentData(data);
        data.equipSite = 101;
        equip.SetAttachSkills("100,107");
        PlayerCtl.E.EquipGuideEquipment(equip);
    }

    public void AddGuideMonster1()
    {
        WorldMng.E.CharacterCtl.AddMonster(1);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (1)").SetActive(false);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (2)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (3)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (4)").SetActive(true);
        UI.ShowHandOnObj(UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (1)"));
    }

    public void AddGuideMonster2()
    {
        WorldMng.E.CharacterCtl.AddMonster(1);
        WorldMng.E.CharacterCtl.AddMonster(1);
        WorldMng.E.CharacterCtl.AddMonster(1);
        WorldMng.E.CharacterCtl.AddMonster(1);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (1)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (2)").SetActive(false);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (3)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (4)").SetActive(true);
        UI.ShowHandOnObj(UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (2)"));
    }

    public void SetSkill2Enabled()
    {
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (1)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (2)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (3)").SetActive(false);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (4)").SetActive(true);
        UI.ShowHandOnObj(UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (3)"));
    }

    public void DisableAllSkillUI()
    {
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (1)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (2)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (3)").SetActive(true);
        UI.GetGameObject("Home(Clone)/Battle/SkillMask/SkillMask (4)").SetActive(true);
    }

    public void GenerateGate()
    {
        DataMng.E.MapData.Add(DataMng.E.MapData.Map[TransferGateSite.x, TransferGateSite.y, TransferGateSite.z], TransferGateSite);
        DataMng.E.MapData.OnClear();
    }

    public void MaskAdButton()
    {
        UI.GetGameObject("GiftBox(Clone)/Image/GameObject/AdvertisingBtn").SetActive(false);
        UI.GetGameObject("GiftBox(Clone)/Image/GameObject/AdvertisingLabel").SetActive(true);
    }

    public void EndBraveGuide1()
    {
        DataMng.E.RuntimeData.GuideId = 6;
    }

    public void DisableCraftScroll()
    {
        UI.GetGameObject("Craft(Clone)/BG/Scroll View").GetComponent<ScrollRect>().enabled = false;
    }
}