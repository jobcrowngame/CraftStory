using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class IconItemCell : UIBase
{
    Text Name;
    Text Count;
    Image Icon;

    private float count;
    private int upCount = 30;

    private void InitUI()
    {
        Name = FindChiled<Text>("Name");
        Count = FindChiled<Text>("Count");
        Icon = FindChiled<Image>("Icon");
    }

    public void Add(Item item, int count)
    {
        InitUI();

        Name.text = item.Name;
        Count.text = "x" + count;
        Icon.sprite = ReadResources<Sprite>(item.IconResourcesPath);

        this.count = count;
    }

    public void StartMultiAnim(int multiple)
    {
        StartCoroutine(MultiAnim(multiple));
    }
    private IEnumerator MultiAnim(int multiple)
    {
        float offset = (count / upCount) * (multiple - 1);
        float newCount = count;

        for (int i = 0; i < upCount; i++)
        {
            yield return new WaitForSeconds(0.03f);
            newCount += offset;
            Count.text = "x" + (int)newCount;
        }
        Count.text = "x" + count * multiple;
    }
}
