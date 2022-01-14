using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterFollow : CharacterBase
{
    Transform ChatFlg { get => CommonFunction.FindChiledByName(transform, "effect_2d_010").transform; }
    Animator animator { get => CommonFunction.FindChiledByName<Animator>(transform, "Model"); }

    public Transform target;
    float smoothing = 1;

    bool following = false;


    public override void Init(int characterId, CharacterGroup camp)
    {
        base.Init(characterId, camp);


        ShowChatFlg(false);

        Model.gameObject.SetActive(false);
        ShowFairy();
    }

    public override void OnClick()
    {
        base.OnClick();

        if (DataMng.E.RuntimeData.MapType == MapType.Brave ||
            DataMng.E.RuntimeData.MapType == MapType.Event)
            return;

        if (!TaskMng.E.IsEnd)
        {
            TaskMng.E.IsReaded = true;
            ShowChatFlg(false);

            if (TaskMng.E.IsClear)
            {
                var chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.OnCloseDestroyObj, TaskMng.E.MainTaskConfig.EndChat);
                chatUi.AddListenerOnClose(() =>
                {
                    HomeLG.E.UI.RefreshTaskOverview();
                });

                DataMng.E.AddBonus(TaskMng.E.MainTaskConfig.Bonus);
                
                TaskMng.E.Next();

                LocalDataMng.E.Data.UserDataT.main_task = TaskMng.E.MainTaskConfig.Next;
                LocalDataMng.E.Data.UserDataT.main_task_count = 0;

                if (HomeLG.E.UI != null)
                    HomeLG.E.UI.RefreshCoins();

                NWMng.E.GetItems();
            }
            else
            {
                var chatUi = UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.OnCloseDestroyObj, TaskMng.E.MainTaskConfig.StartChat);
                chatUi.AddListenerOnClose(() =>
                {
                    HomeLG.E.UI.RefreshTaskOverview();
                });
            }
        }
    }

    public override void OnBehaviorChange(BehaviorType behavior)
    {
        base.OnBehaviorChange(behavior);

        animator.SetInteger("State", (int)behavior);
    }

    void Update()
    {
        if (target != null)
        {
            following = Mathf.Abs(Vector3.Distance(target.position, transform.position)) > 0.1f;

            if (following)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothing);

                var dir = target.position - transform.position;
                var angle = CommonFunction.Vector2ToAngle(new Vector2(dir.x, dir.z));
                transform.rotation = Quaternion.Euler(new Vector3(0, -angle + 90, 0));
            }
        }
    }

    public void ShowFairy()
    {
        StartCoroutine(AppearancePerformanceIE());
    }

    /// <summary>
    /// 目標の設定
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <summary>
    /// チャットフラグ
    /// </summary>
    /// <param name="b"></param>
    public void ShowChatFlg(bool b = true)
    {
        ChatFlg.gameObject.SetActive(b);
    }

    /// <summary>
    /// 出現演出
    /// </summary>
    /// <returns></returns>
    IEnumerator AppearancePerformanceIE()
    {
        yield return new WaitForSeconds(1f);

        // add effect
        EffectMng.E.AddEffect<EffectBase>(transform.position + new Vector3(0,1,0), EffectType.FairyCreate);

        Model.gameObject.SetActive(true);
        ShowChatFlg(false);

        // アニメション完了待つ
        yield return new WaitForSeconds(1.8f);

        if (DataMng.E.UserData.FirstShowFairy)
        {
            // 挨拶チャット
            UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.OnCloseDestroyObj, 99);
            DataMng.E.UserData.FirstShowFairy = false;
        }

        Behavior = BehaviorType.Run;

        if (DataMng.E.RuntimeData.MapType != MapType.Brave &&
            DataMng.E.RuntimeData.MapType != MapType.Event)
        {
            // タスク全部Endしたら吹き出しを出さない
            if (TaskMng.E.IsEnd)
            {
                ShowChatFlg(false);
            }
            else
            {
                if (TaskMng.E.IsClear)
                {
                    // タスクがクリア状態なら吹き出しを出す
                    ShowChatFlg(true);
                }
                else
                {
                    // 今のタスクを既読した場合、吹き出しを出さない
                    ShowChatFlg(!TaskMng.E.IsReaded);
                }
            }

            HomeLG.E.UI.ActivateTaskOverview();
            HomeLG.E.UI.RefreshTaskOverview();
        }
    }
}