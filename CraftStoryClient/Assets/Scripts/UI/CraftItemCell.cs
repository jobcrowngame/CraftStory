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
    Image RecommendationIcon;
    Button clickBtn;
    Transform selected;

    private void Awake()
    {
        itemName = FindChiled<Text>("Name");
        itemCount = FindChiled<Text>("Count");
        Icon = FindChiled<Image>("Icon");
        RecommendationIcon = FindChiled<Image>("RecommendationIcon");
        selected = FindChiled("Select");

        clickBtn = transform.GetComponent<Button>();

        RecommendationIcon.gameObject.SetActive(false);
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
            CraftLG.E.SelectCraft = config;
            CraftLG.E.SelectCraftItemCell = this;
            CraftLG.E.UI.RefreshCost();
            GuideLG.E.Next();
        });

        if (config.Recommendation == 1)
        {
            RecommendationIcon.gameObject.SetActive(true);
        }
    }

    private void IsSelected(bool b)
    {
        selected.gameObject.SetActive(b);
    }

    public void CloneIconToBag()
    {
        var clone = Instantiate(Icon.gameObject);
        clone.transform.SetParent(UICtl.E.Root);
        clone.transform.position = transform.position;
        clone.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        var cell = clone.AddComponent<SimpleMove>();
        if(cell != null) cell.Set(HomeLG.E.UI.GetBagIconPos());
    }
}
