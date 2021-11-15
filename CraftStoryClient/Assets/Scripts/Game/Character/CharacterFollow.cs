using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterFollow : CharacterBase
{
    Image ChatFlg { get => CommonFunction.FindChiledByName<Image>(transform, "ChatFlg"); }

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
            ChangeChatFlgImg();

            if (TaskMng.E.IsClear)
            {
                NWMng.E.MainTaskEnd((rp) => 
                {
                    UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, TaskMng.E.MainTaskConfig.EndChat);
                    TaskMng.E.Next();
                }, TaskMng.E.MainTaskId);
            }
            else
            {
                UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.None, TaskMng.E.MainTaskConfig.StartChat);
            }
        }
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
    /// チャット画像
    /// </summary>
    /// <param name="isReaded">true = 既読　false = 未読</param>
    public void ChangeChatFlgImg(bool isReaded = true)
    {
        string iconPath = isReaded 
            ? "Textures/button_2D_006"
            : "Textures/button_2D_007";

        ChatFlg.sprite = ResourcesMng.E.ReadResources<Sprite>(iconPath);
    }

    /// <summary>
    /// 出現演出
    /// </summary>
    /// <returns></returns>
    IEnumerator AppearancePerformanceIE()
    {
        yield return new WaitForSeconds(2);

        // add effect

        yield return new WaitForSeconds(1);

        Model.gameObject.SetActive(true);

        // アニメション

        ChangeChatFlgImg(false);

        // アニメション完了待つ
        yield return new WaitForSeconds(1);

        // チャットフラグ出す
        if (!TaskMng.E.IsEnd)
        {
            ChangeChatFlgImg(false);
            ShowChatFlg(true);
        }
    }
}