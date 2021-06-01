using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class ItemBtn : UIBase
{
    private static ItemBtn curSelectBtn;
    Image Icon;
    Transform Selection;

    Button btn;

    private int itemID;
    Item config { get => ConfigMng.E.Item[itemID]; }

    private void Awake()
    {
        Icon = FindChiled<Image>("Icon");
        Selection = FindChiled("Selection");

        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        OnSelected(false);
    }

    public void Init(int itemID)
    {
        this.itemID = itemID;

        Icon.sprite = ReadResources<Sprite>(config.IconResourcesPath);
    }

    private void OnClick()
    {
        Debug.LogFormat("Select item {0}. Type {1}", config.Name, (ItemType)config.Type);

        UICtl.E.GetUI<HomeUI>(UIType.Home).ShowBuilderPencilBtn(false);

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
        PlayerCtl.E.ChangeSelectItem(b ? itemID : 0);
        Selection.gameObject.SetActive(b);
    }

    public static void Select(ItemBtn itemBtn)
    {
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
}
