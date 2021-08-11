using System.Collections;
using UnityEngine.UI;

public class BlueprintReNameUI : UIBase
{
    Button OkBtn;
    Button CancelBtn;
    InputField input;

    public string mapData { get; set; }

    private void Awake()
    {
        OkBtn = FindChiled<Button>("OkBtn");
        OkBtn.onClick.AddListener(OnClickOK);

        CancelBtn = FindChiled<Button>("CancelBtn");
        CancelBtn.onClick.AddListener(()=> { Close(); });

        input = FindChiled<InputField>("InputField");
        input.onEndEdit.AddListener((r) =>
        {
            if (string.IsNullOrEmpty(input.text))
            {
                CommonFunction.ShowHintBar(3);
                return;
            }

            GuideLG.E.Next();
        });
    }

    public void SetMapData(string msg)
    {
        mapData = msg;
    }

    public void OnClickOK()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            CommonFunction.ShowHintBar(3);
            return;
        }

        DataMng.E.AddItemInData(3002, 1, input.text, mapData, ()=> 
        {
            if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            {
                DataMng.E.RemoveItemByGuid(PlayerCtl.E.SelectItem.id, 1);
                PlayerCtl.E.SelectItem = null;
                Close();

                GuideLG.E.Next();
            }
            else
            {
                NWMng.E.RemoveItem((rp) =>
                {
                    NWMng.E.GetItems(() =>
                    {
                        PlayerCtl.E.SelectItem = null;
                        Close();
                    });
                }, PlayerCtl.E.SelectItem.itemId, 1);
            }
        });
    }
}
