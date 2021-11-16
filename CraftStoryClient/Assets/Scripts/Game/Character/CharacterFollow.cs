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


    public override void Init(int characterId, CharacterCamp camp)
    {
        base.Init(characterId, camp);


        ShowChatFlg(false);

        Model.gameObject.SetActive(false);

        StartCoroutine(AppearancePerformanceIE());
    }

    public override void OnClick()
    {
        base.OnClick();

        if (!TaskMng.E.IsEnd)
        {
            TaskMng.E.IsReaded = true;
            ShowChatFlg(false);

            if (TaskMng.E.IsClear)
            {
                NWMng.E.MainTaskEnd((rp) =>
                {
                    UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, TaskMng.E.MainTaskConfig.EndChat);
                    TaskMng.E.Next();

                    NWMng.E.RefreshCoins(() =>
                    {
                        if (HomeLG.E.UI != null) 
                            HomeLG.E.UI.RefreshCoins();
                    });
                }, TaskMng.E.MainTaskId);
            }
            else
            {
                UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, TaskMng.E.MainTaskConfig.StartChat);
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
        // 2S 後出る
        yield return new WaitForSeconds(2);

        // add effect
        EffectMng.E.AddEffect<EffectBase>(transform.position + new Vector3(0,1,0), EffectType.FairyCreate);

        Model.gameObject.SetActive(true);
        ShowChatFlg(false);

        // アニメション完了待つ
        yield return new WaitForSeconds(1.8f);

        if (DataMng.E.UserData.FirstShowFairy)
        {
            // 挨拶チャット
            UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, 99);
            DataMng.E.UserData.FirstShowFairy = false;
        }

        Behavior = BehaviorType.Run;

        // チャットフラグ出す
        if (!TaskMng.E.IsEnd)
        {
            ShowChatFlg(!TaskMng.E.IsReaded);
        }

        // タスクがクリア状態なら吹き出しを出す
        if (TaskMng.E.IsClear)
        {
            ShowChatFlg();
        }
    }
}