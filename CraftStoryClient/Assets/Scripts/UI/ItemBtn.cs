using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class ItemBtn : UIBase
{
    private static ItemBtn curSelectBtn;

    Image Icon;
    Text Count;
    Transform Selection;

    Button btn;

    private ItemData item;
    Item config { get => ConfigMng.E.Item[item.ItemID]; }

    private void Awake()
    {
        Icon = FindChiled<Image>("Icon");
        Count = FindChiled<Text>("Count");
        Selection = FindChiled("Selection");

        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        OnSelected(false);
    }

    public void Init(ItemData item)
    {
        this.item = item;

        Icon.sprite = ReadResources<Sprite>(config.IconResourcesPath);
        Count.text = item.Config.MaxCount == 1 
            ? ""
            : "x" + item.Count;
    }

    private void OnClick()
    {
        if (item == null)
            return;

        Debug.LogFormat("Select item {0}. Type {1}", config.Name, (ItemType)config.Type);
        if(item.Data != null) Debug.LogFormat(item.Data.ToString());

        if (curSelectBtn == this)
        {
            OnSelected(false);

            curSelectBtn = null;
            return;
        }

        Select(this);
    }
    
    public void OnSelected(bool b = true)
    {
        PlayerCtl.E.ChangeSelectItem(b ? item : null);
        Selection.gameObject.SetActive(b);
    }

    public void Clear()
    {
        item = null;
        Icon.sprite = null;
        Count.text = "";
        Selection.gameObject.SetActive(false);
    }

    public static void Select(ItemBtn itemBtn)
    {
        if (itemBtn == null)
        {
            curSelectBtn.OnSelected(false);
            curSelectBtn = null;
            return;
        }

        if (curSelectBtn != null)
            curSelectBtn.OnSelected(false);

        curSelectBtn = itemBtn;
        curSelectBtn.OnSelected();
    }
}

public enum ItemType
{
    Block = 1,
    BuilderPencil = 50,
    Blueprint = 51,
}
