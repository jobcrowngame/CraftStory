using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;

/// <summary>
/// チュートリアル
/// </summary>
public class GuideUI : UIBase
{
    RectTransform mask1 { get => FindChiled<RectTransform>("Mask (1)"); }
    RectTransform mask2 { get => FindChiled<RectTransform>("Mask (2)"); }
    RectTransform mask3 { get => FindChiled<RectTransform>("Mask (3)"); }
    RectTransform mask4 { get => FindChiled<RectTransform>("Mask (4)"); }
    RectTransform Chat { get => FindChiled<RectTransform>("Chat"); }
    Transform Hand { get => FindChiled<Transform>("Hand"); }
    RectTransform canvas { get => transform.parent.GetComponent<RectTransform>(); }
    Transform FullMask { get => FindChiled<Transform>("FullMask"); }
    Button NextMask { get => FindChiled<Button>("NextMask"); }

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        GuideLG.E.Init(this);
        GuideLG.E.Clear();

        NextMask.onClick.AddListener(() =>
        {
            GuideLG.E.Next();
        });

        ShowMask(false);
        GuideLG.E.Next();
    }

    /// <summary>
    /// オブジェクト選択
    /// </summary>
    /// <param name="selectedObj"></param>
    private void Select(GameObject selectedObj)
    {
        if (selectedObj == null)
        {
            ShowMask(false);
            return;
        }

        ShowMask();

        var selectRect = selectedObj.GetComponent<RectTransform>();
        float offset = selectRect.lossyScale.x;
        float width = selectRect.rect.size.x;
        float height = selectRect.rect.size.y;
        var canvasSize = canvas.sizeDelta;

        mask1.offsetMin = new Vector2(selectedObj.transform.position.x / offset + width / 2, 0);
        mask1.offsetMax = new Vector2(0, 0);

        mask2.offsetMin = new Vector2(0, 0);
        mask2.offsetMax = new Vector2(-(canvasSize.x - selectedObj.transform.position.x / offset + width / 2), 0);

        mask3.offsetMin = new Vector2(selectedObj.transform.position.x / offset - width / 2,
            selectedObj.transform.position.y / offset + height * selectRect.pivot.y);
            //selectedObj.transform.position.y / offset + height / 2);
        mask3.offsetMax = new Vector2(-(canvasSize.x - selectedObj.transform.position.x / offset - width / 2), 0);

        mask4.offsetMin = new Vector2(selectedObj.transform.position.x / offset - width / 2, 0);
        mask4.offsetMax = new Vector2(-(canvasSize.x - selectedObj.transform.position.x / offset - width / 2), 
            -(canvasSize.y - selectedObj.transform.position.y / offset + height * selectRect.pivot.y));
            //-(canvasSize.y - selectedObj.transform.position.y / offset + height / 2));

        SetHand(new Vector2(selectedObj.transform.position.x, selectedObj.transform.position.y));
    }
    private void SetMessage(Vector2 pos, Vector2 size, string msg)
    {
        Chat.gameObject.SetActive(true);

        Chat.transform.localPosition = pos;
        Chat.sizeDelta = size;
        FindChiled<Text>("Text", Chat.transform).text = msg;

        Chat.gameObject.SetActive(!string.IsNullOrEmpty(msg));
    }
    private void SetHand(Vector2 pos)
    {
        Hand.transform.position = pos;
    }
    public void ShowHandOnObj(GameObject obj)
    {
        Hand.gameObject.SetActive(true);
        Hand.transform.position = obj.transform.position;
    }
    private void ShowMask(bool b = true)
    {
        mask1.gameObject.SetActive(b);
        mask2.gameObject.SetActive(b);
        mask3.gameObject.SetActive(b);
        mask4.gameObject.SetActive(b);
    }
    public void ShowFullMask(bool b)
    {
        FullMask.gameObject.SetActive(b);
    }
    private void End()
    {
        ShowMask(false);
        Chat.gameObject.SetActive(false);
        Hand.gameObject.SetActive(false);
        GuideLG.E.end = true;

        NWMng.E.GuideEnd((rp)=> 
        {
            if (DataMng.E.RuntimeData.GuideId == 1)
            {
                DataMng.E.RuntimeData.GuideEnd = 1;
                DataMng.E.RuntimeData.NewEmailCount++;
                // 設計図タスク完了
                TaskMng.E.AddMainTaskCount(5);
            }
            // ショップのチュートリアル完了
            else if (DataMng.E.RuntimeData.GuideId == 2)
            {
                DataMng.E.RuntimeData.GuideEnd2 = 1;
                // ショップタスク完了
                TaskMng.E.AddMainTaskCount(1);
            }
            // 最初のクラフトチュートリアル完了
            else if (DataMng.E.RuntimeData.GuideId == 3)
            {
                DataMng.E.RuntimeData.GuideEnd3 = 1;
                DataMng.E.RuntimeData.NewEmailCount++;
            }
            // 武器チュートリアル完了
            else if (DataMng.E.RuntimeData.GuideId == 4)
            {
                DataMng.E.RuntimeData.GuideEnd4 = 1;
                PlayerCtl.E.GetEquipedItems().Clear();
                TaskMng.E.AddMainTaskCount(2);

                // チュートリアルが完了後、デフォルトで武器を鑑定して装備
                var item = DataMng.E.GetItemByItemId(10001);
                NWMng.E.AppraisalEquipment((rp) =>
                {
                    NWMng.E.EquitItem((rp)=> 
                    {
                        PlayerCtl.E.EquipEquipment(new ItemEquipmentData(item));
                    }, item.id, 101);
                }, item.id, 1);
            }
            else if (DataMng.E.RuntimeData.GuideId == 6)
            {
                DataMng.E.RuntimeData.GuideEnd5 = 1;
                DataMng.E.RuntimeData.Lv = GuideLG.E.HomeRuntimeData.Lv;
                DataMng.E.RuntimeData.Exp = GuideLG.E.HomeRuntimeData.Exp;
                TaskMng.E.AddMainTaskCount(11);
            }
        }, DataMng.E.RuntimeData.GuideId);
    }

    /// <summary>
    /// 喋るWindowを閉じる
    /// </summary>
    public void CloseChatWindow()
    {
        Chat.gameObject.SetActive(false);
    }

    public void NextStep(int stepId)
    {
        if (stepId == 0)
        {
            End();
            return;
        }

        SetMessage(Vector3.zero, Vector3.one, "");
        StartCoroutine(StartNextStep(stepId));
    }

    IEnumerator StartNextStep(int stepId)
    {
        yield return new WaitForSeconds(0.1f);

        var config = ConfigMng.E.GuideStep[stepId];
        if (config.CellName == "N")
        {
            Select(null);
        }
        else
        {
            // 自動移動がある場合
            if (config.AutoMove == 1)
            {
                var targetObj = GameObject.Find(config.CellName);
                if (targetObj != null)
                {
                    var entity = targetObj.GetComponent<EntityFunctionalObject>();
                    if (entity != null)
                    {
                        entity.OnClick();
                    }
                }
                else
                {
                    Logger.Error("targetObj が見つけません。{0}", config.CellName);
                }
            }
            // WindowのObjectを選択場合
            else
            {
                Select(CommonFunction.FindChiledByName(UICtl.E.Root, config.CellName));
            }
        }

        // メッセージを設定
        Vector2 pos = new Vector2(config.MsgPosX, config.MsgPosY);
        Vector2 size = new Vector2(config.MsgSizeX, config.MsgSizeY);
        SetMessage(pos, size, config.Message);

        // 表示オブジェクト
        if (config.DisplayObject != "N")
        {
            var targetObj = GetGameObject(config.DisplayObject);
            if (targetObj != null)
            {
                targetObj.gameObject.SetActive(true);
            }
            else
            {
                Logger.Error("DisplayObject が見つけません。{0}", config.DisplayObject);
            }
        }

        // 非表示オブジェクト
        if (config.HideObject != "N")
        {
            var targetObj = GetGameObject(config.HideObject);
            if (targetObj != null)
            {
                targetObj.gameObject.SetActive(false);
            }
            else
            {
                Logger.Error("HideObject が見つけません。{0}", config.HideObject);
            }
        }

        // 手オブジェクト表し
        Hand.gameObject.SetActive(config.HideHand != 1);

        // メソッド実行
        if (config.GuideLGMethodList != "N")
        {
            foreach(string method in config.GuideLGMethodList.Split(','))
            {
                MethodInfo mi = GuideLG.E.GetType().GetMethod(method);
                mi.Invoke(GuideLG.E, null);
            }
        }
 
        // 次会話に遷移
        NextMask.gameObject.SetActive(config.NextMask == 1);

        // 自動移動する場合はずっとロックされています。
        if (config.AutoMove < 1)
        {
            // 画面ロック解除
            GuideLG.E.UnLock();
        }
    }

    public GameObject GetGameObject(string path)
    {
        string target = path;
        Transform parent = UICtl.E.Root;
        // '/'始まりの場合ルートからのパスとする、それ以外の場合Canvasからのパスとする
        Regex re = new Regex(@"^\/(.+?)\/(.+)", RegexOptions.Compiled);
        Match match = re.Match(path);
        if (match.Success)
        {
            string rootPath = match.Groups[1].Value;
            parent = GameObject.Find(rootPath).transform;
            target = match.Groups[2].Value;
        }
        return CommonFunction.FindChiledByName(parent, target);
    }
}
