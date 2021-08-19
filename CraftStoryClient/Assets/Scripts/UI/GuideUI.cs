using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class GuideUI : UIBase
{
    RectTransform mask1 { get => FindChiled<RectTransform>("Mask (1)"); }
    RectTransform mask2 { get => FindChiled<RectTransform>("Mask (2)"); }
    RectTransform mask3 { get => FindChiled<RectTransform>("Mask (3)"); }
    RectTransform mask4 { get => FindChiled<RectTransform>("Mask (4)"); }
    RectTransform Msg { get => FindChiled<RectTransform>("Image"); }
    Transform Hand { get => FindChiled<Transform>("Hand"); }
    RectTransform canvas { get => transform.parent.GetComponent<RectTransform>(); }
    Transform FullMask { get => FindChiled<Transform>("FullMask"); }

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        GuideLG.E.Init(this);

        ShowMask(false);
        GuideLG.E.Next();
    }

    private void Select(GameObject selectedObj)
    {
        if (selectedObj == null)
        {
            ShowMask(false);
            Logger.Error("Not find selectedObj");
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
            selectedObj.transform.position.y / offset + height / 2);
        mask3.offsetMax = new Vector2(-(canvasSize.x - selectedObj.transform.position.x / offset - width / 2), 0);

        mask4.offsetMin = new Vector2(selectedObj.transform.position.x / offset - width / 2, 0);
        mask4.offsetMax = new Vector2(-(canvasSize.x - selectedObj.transform.position.x / offset - width / 2), 
            -(canvasSize.y - selectedObj.transform.position.y / offset + height / 2));

        SetHand(new Vector2(selectedObj.transform.position.x + 60, selectedObj.transform.position.y + 20));
    }
    private void SetMessage(Vector2 pos, Vector2 size, string msg)
    {
        Msg.transform.localPosition = pos;
        Msg.sizeDelta = size;
        FindChiled<Text>("Text", Msg.transform).text = msg;

        Msg.gameObject.SetActive(!string.IsNullOrEmpty(msg));
    }
    private void SetHand(Vector2 pos)
    {
        Hand.transform.position = pos;
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
        Msg.gameObject.SetActive(false);
        Hand.gameObject.SetActive(false);
        GuideLG.E.end = true;

        NWMng.E.GuideEnd((rp)=> { DataMng.E.RuntimeData.GuideEnd = 1; });
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
        yield return new WaitForSeconds(0.08f);

        var config = ConfigMng.E.GuideStep[stepId];
        if (config.CellName == "N")
        {
            Select(null);
        }
        else
        {
            Select(CommonFunction.FindChiledByName(UICtl.E.Root, config.CellName));
        }

        Vector2 pos = new Vector2(config.MsgPosX, config.MsgPosY);
        Vector2 size = new Vector2(config.MsgSizeX, config.MsgSizeY);
        SetMessage(pos, size, config.Message);

        Hand.gameObject.SetActive(config.HideHand != 1);

        GuideLG.E.UnLock();
    }
}
