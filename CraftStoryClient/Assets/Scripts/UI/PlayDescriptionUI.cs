using UnityEngine;
using UnityEngine.UI;

public class PlayDescriptionUI : UIBase
{
    Image img { get => FindChiled<Image>("Image"); }
    Button RightBtn { get => FindChiled<Button>("RightBtn"); }
    Button LeftBtn { get => FindChiled<Button>("LeftBtn"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }

    private int Index
    {
        get => index;
        set
        {
            if (value < 1 || value > 3)
                return;

            index = value;

            OkBtn.gameObject.SetActive(index == 3);
            LeftBtn.gameObject.SetActive(index > 1);
            RightBtn.gameObject.SetActive(index < 3);

            switch (index)
            {
                case 1: img.sprite = ReadResources<Sprite>("Textures/grass"); break;
                case 2: img.sprite = ReadResources<Sprite>("Textures/blueprint"); break;
                case 3: img.sprite = ReadResources<Sprite>("Textures/sand"); break;
            }
        }
    }
    private int index;

    public override void Init()
    {
        base.Init();

        PlayDescriptionLG.E.Init(this);

        RightBtn.onClick.AddListener(() => { Index++; });
        LeftBtn.onClick.AddListener(() => { Index--; });
        OkBtn.onClick.AddListener(() => { Close(); });

        Index = 1;
    }
}