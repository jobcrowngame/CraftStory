using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleBtns : UIBase
{
    ToggleGroup group;
    MyToggle[] cells;

    UnityAction<int> callback;

    public override void Init()
    {
        base.Init();

        group = transform.GetComponent<ToggleGroup>();
        cells = new MyToggle[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            cells[i] = transform.GetChild(i).GetComponent<MyToggle>();
            cells[i].Index = i;
        }
        cells[0].isOn = true;
    }

    public void OnValueChange(int index)
    {
        if (callback != null)
        {
            callback(index);
        }
    }

    public void OnValueChangeAddListener(UnityAction<int> call)
    {
        callback = call;
    }

    public void SetBtnText(int index, string msg)
    {
        if (cells != null && cells.Length > index)
        {
            CommonFunction.FindChiledByName<Text>(cells[index].transform, "Text").text = msg;
        }
    }
    public void SetValue(int index)
    {
        if (cells.Length > index)
        {
            cells[index].isOn = true;
        }
    }
}
