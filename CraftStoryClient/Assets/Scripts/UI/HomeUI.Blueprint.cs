using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class HomeUI
{
    #region 変数

    /// <summary>
    /// ビルダーペンセル
    /// </summary>
    Transform BuilderPencil { get => FindChiled("BuilderPencil"); }
    /// <summary>
    /// ビルドボタン
    /// </summary>
    Button BuilderBtn { get => FindChiled<Button>("BuilderBtn", BuilderPencil); }
    /// <summary>
    /// キャンセルビルダーボタン
    /// </summary>
    Button BuilderPencilCancelBtn { get => FindChiled<Button>("BuilderPencilCancelBtn", BuilderPencil); }
    /// <summary>
    /// 設計図を使用する場合、コンソールWindow
    /// </summary>
    Transform Blueprint { get => FindChiled("Blueprint"); }
    /// <summary>
    /// 
    /// </summary>
    Transform BlueprintScrollView { get => FindChiled("Scroll View", Blueprint.gameObject); }
    /// <summary>
    /// ロック設計図消耗説明
    /// </summary>
    Transform BlueprintDes { get => FindChiled("BlueprintDes", Blueprint.gameObject); }
    /// <summary>
    /// 消耗するアイテムリストのサブ親
    /// </summary>
    Transform BlueprintCellGrid { get => FindChiled("Content", Blueprint.gameObject); }
    /// <summary>
    /// 回転ボタン
    /// </summary>
    Button SpinBtn { get => FindChiled<Button>("SpinBtn", Blueprint); }
    /// <summary>
    /// ビルダーキャンセルボタン
    /// </summary>
    Button BlueprintCancelBtn { get => FindChiled<Button>("BlueprintCancelBtn", Blueprint); }
    /// <summary>
    /// ビルダーボタン
    /// </summary>
    Button BuildBtn { get => FindChiled<Button>("BuildBtn", Blueprint); }

    #endregion

    public void InitBlueprint()
    {
        BuilderBtn.onClick.AddListener(CreateBlueprint);
        BuilderPencilCancelBtn.onClick.AddListener(CancelBuilderPencilCancelBtn);
        SpinBtn.onClick.AddListener(SpinBlueprint);
        BlueprintCancelBtn.onClick.AddListener(CancelUserBlueprint);
        BuildBtn.onClick.AddListener(BuildBlueprint);
    }

    private void CreateBlueprint()
    {
        Logger.Log("BuilderBtn");

        PlayerCtl.E.BuilderPencil.CreateBlueprint();
    }
    private void CancelBuilderPencilCancelBtn()
    {
        Logger.Log("CancelBtn");

        PlayerCtl.E.BuilderPencil.CancelCreateBlueprint();
    }
    private void SpinBlueprint()
    {
        PlayerCtl.E.BuilderPencil.SpinBlueprint();
    }
    private void CancelUserBlueprint()
    {
        PlayerCtl.E.BuilderPencil.CancelUserBlueprint();
    }
    private void BuildBlueprint()
    {
        PlayerCtl.E.BuilderPencil.BuildBlueprint();
    }

    /// <summary>
    /// ビルダーペンセルコンソールを表し
    /// </summary>
    /// <param name="b"></param>
    public void ShowBuilderPencilBtn(bool b = true)
    {
        if (BuilderPencil != null)
            BuilderPencil.gameObject.SetActive(b);
    }

    /// <summary>
    /// 設計図使用場合のコンソールWindowを表し
    /// </summary>
    /// <param name="b"></param>
    public void ShowBlueprintBtn(bool b = true)
    {
        if (Blueprint != null)
            Blueprint.gameObject.SetActive(b);
    }

    /// <summary>
    /// 設計図を使うに必要なブロックリスト
    /// </summary>
    /// <param name="blueprint">設計図</param>
    public void AddBlueprintCostItems(BlueprintData blueprint)
    {
        BlueprintDes.gameObject.SetActive(false);
        BlueprintScrollView.gameObject.SetActive(true);
        ClearCell(BlueprintCellGrid);

        Dictionary<int, int> costs = new Dictionary<int, int>();
        foreach (var entity in blueprint.blocks)
        {
            // Obstacleは無視
            if ((EntityType)ConfigMng.E.Entity[entity.id].Type == EntityType.Obstacle)
                continue;

            if (costs.ContainsKey(entity.id))
            {
                costs[entity.id]++;
            }
            else
            {
                costs[entity.id] = 1;
            }
        }

        foreach (var key in costs.Keys)
        {
            var cell = AddCell<BlueprintCell>("Prefabs/UI/BlueprintCell", BlueprintCellGrid);
            if (cell == null)
                return;

            cell.Init(ConfigMng.E.Entity[key].ItemID, costs[key]);
        }
    }

    /// <summary>
    /// ロックされた設計図を使う場合の説明を出す
    /// </summary>
    public void ShowLockBlueprintDes()
    {
        BlueprintDes.gameObject.SetActive(true);
        BlueprintScrollView.gameObject.SetActive(false);
    }
}
