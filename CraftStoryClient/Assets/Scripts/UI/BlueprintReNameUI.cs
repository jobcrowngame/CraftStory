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
            NWMng.E.RemoveItemByGuid(null, PlayerCtl.E.SelectItem.id, 1);
            PlayerCtl.E.SelectItem = null;
            Close();
        });
    }
}
