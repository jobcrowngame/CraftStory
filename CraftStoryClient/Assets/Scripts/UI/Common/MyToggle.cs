using System.Collections;
using UnityEngine.UI;

public class MyToggle : Toggle
{
    public int Index { get; set; }
    public ToggleBtns toggleBtns { get; set; }

    protected override void Awake()
    {
        if (group != null)
        {
            toggleBtns = group.GetComponent<ToggleBtns>();

            onValueChanged.AddListener((b) => 
            {
                if (b)
                    toggleBtns.OnValueChange(Index);
            });
        }
    }
}
