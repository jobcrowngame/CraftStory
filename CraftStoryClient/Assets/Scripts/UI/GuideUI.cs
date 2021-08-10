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
    CanvasScaler canvas { get => transform.parent.GetComponent<CanvasScaler>(); }

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
        ShowMask();

        var selectRect = selectedObj.GetComponent<RectTransform>();
        float offset = selectRect.lossyScale.x;
        float width = selectRect.rect.size.x;
        float height = selectRect.rect.size.y;
        var canvasX = canvas.referenceResolution;

        mask1.offsetMin = new Vector2(selectedObj.transform.position.x / offset + width / 2, 0);
        mask1.offsetMax = new Vector2(0, 0);

        mask2.offsetMin = new Vector2(0, 0);
        mask2.offsetMax = new Vector2(-(canvasX.x - selectedObj.transform.position.x / offset + width / 2), 0);

        mask3.offsetMin = new Vector2(selectedObj.transform.position.x / offset - width / 2,
            selectedObj.transform.position.y / offset + height / 2);
        mask3.offsetMax = new Vector2(-(canvasX.x - selectedObj.transform.position.x / offset - width / 2), 0);

        mask4.offsetMin = new Vector2(selectedObj.transform.position.x / offset - width / 2, 0);
        mask4.offsetMax = new Vector2(-(canvasX.x - selectedObj.transform.position.x / offset - width / 2), 
            -(canvasX.y - selectedObj.transform.position.y / offset + height / 2));
    }
    private void SetMessage(Vector2 pos, Vector2 size, string msg)
    {
        Msg.transform.localPosition = pos;
        Msg.sizeDelta = size;
        FindChiled<Text>("Text", Msg.transform).text = msg;

        Msg.gameObject.SetActive(!string.IsNullOrEmpty(msg));
    }
    private void ShowMask(bool b = true)
    {
        mask1.gameObject.SetActive(b);
        mask2.gameObject.SetActive(b);
        mask3.gameObject.SetActive(b);
        mask4.gameObject.SetActive(b);
    }
    private void End()
    {
        ShowMask(false);
        Msg.gameObject.SetActive(false);
        GuideLG.E.end = true;
    }

    public void NextStep(int stepId)
    {
        if (stepId == 0)
        {
            End();
            return;
        }

        StartCoroutine(StartNextStep(stepId));
    }

    IEnumerator StartNextStep(int stepId)
    {
        yield return new WaitForSeconds(0.1f);

        var config = ConfigMng.E.GuideStep[stepId];
        Select(CommonFunction.FindChiledByName(UICtl.E.Root, config.CellName));

        Vector2 pos = new Vector2(config.MsgPosX, config.MsgPosY);
        Vector2 size = new Vector2(config.MsgSizeX, config.MsgSizeY);
        SetMessage(pos, size, config.Message);
    }
}
