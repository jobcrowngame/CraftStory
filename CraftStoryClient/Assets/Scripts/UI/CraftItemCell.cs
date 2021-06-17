using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class CraftItemCell : UIBase
{
    public static CraftItemCell SelectedCraftItem
    {
        get => selectedCraftItem;
        set
        {
            if(selectedCraftItem != null) selectedCraftItem.IsSelected(false);

            selectedCraftItem = value;
            if (selectedCraftItem != null) selectedCraftItem.IsSelected(true);
        }
    }
    private static CraftItemCell selectedCraftItem;

    Text itemName;
    Text itemCount;
    Image Icon;
    Button clickBtn;
    Transform selected;

    private void Awake()
    {
        itemName = FindChiled<Text>("Name");
        itemCount = FindChiled<Text>("Count");
        Icon = FindChiled<Image>("Icon");
        selected = FindChiled("Select");

        clickBtn = transform.GetComponent<Button>();
    }

    public void Init(Craft config)
    {
        var itemConfig = ConfigMng.E.Item[config.ItemID];
        itemName.text = itemConfig.Name;
        itemCount.text = "x1";
        Icon.sprite = ReadResources<Sprite>(itemConfig.IconResourcesPath);
        clickBtn.onClick.AddListener(() => 
        {
            SelectedCraftItem = this;
            CraftLG.E.UI.RefreshCost(config); 
        });
    }

    private void IsSelected(bool b)
    {
        selected.gameObject.SetActive(b);
    }
}
