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

    private ItemData itemData;

    private void Awake()
    {
        Icon = FindChiled<Image>("Icon");
        Count = FindChiled<Text>("Count");
        Selection = FindChiled("Selection");

        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        OnSelected(false);
    }

    public void Init(ItemData itemData)
    {
        this.itemData = itemData;

        if (itemData == null)
        {
            Icon.sprite = null;
            Count.text = "";
            OnSelected(false);
        }
        else
        {
            Icon.sprite = ReadResources<Sprite>(itemData.Config().IconResourcesPath);
            Count.text = itemData.Config().MaxCount == 1
                ? ""
                : "x" + itemData.count;
        }
    }

    private void OnClick()
    {
        if (itemData == null)
            return;

        Debug.LogFormat("Select item {0}. Type {1}", itemData.Config().Name, (ItemType)itemData.Config().Type);
        if(itemData.Data != null) Debug.LogFormat(itemData.Data.ToString());

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
        PlayerCtl.E.ChangeSelectItem(b ? itemData : null);
        Selection.gameObject.SetActive(b);
    }

    public void Clear()
    {
        itemData = null;
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
