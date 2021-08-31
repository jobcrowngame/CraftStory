using UnityEngine;
using UnityEngine.UI;

public class RouletteUI : UIBase
{
    Image TitleIcon { get => FindChiled<Image>("TitleIcon"); }
    Image RouletteBG { get => FindChiled<Image>("RouletteBG"); }
    Button StartBtn { get => FindChiled<Button>("StartBtn"); }
    Transform Parent { get => FindChiled("CellParent"); }
    Button OverMask { get => FindChiled<Button>("OverMask"); }
    Button AgainMask { get => FindChiled<Button>("AgainMask"); }

    int index;
    float speed;
    bool again;
    bool start;
    bool end = false;
    float maxSpeed = 1000;
    float rotateSpeed = 250;

    const float stopSpeed = 422;

    public override void Init(object gachaId)
    {
        base.Init(gachaId);
        RouletteLG.E.Init(this);

        StartBtn.onClick.AddListener(StartRoulette);
        OverMask.onClick.AddListener(Close);
        AgainMask.onClick.AddListener(()=> 
        {
            NWMng.E.GachaAddBonusAgain((rp) => 
            {
                int index = (int)rp["index"];
                var ui = UICtl.E.OpenUI<GachaAddBonusUI>(UIType.GachaAddBonus);
                ui.Set(index, 1);
            }, 1);
            Close();
        });

        AddCells();
    }

    public override void Open(object gachaId)
    {
        base.Open(gachaId);
        StartBtn.enabled = true;
        start = false;
        end = false;
        speed = 0;
        RouletteBG.transform.eulerAngles = Vector3.zero;

        OverMask.gameObject.SetActive(false);
        AgainMask.gameObject.SetActive(false);

        var config = ConfigMng.E.Gacha[(int)gachaId];
        again = Random.Range(0, 1000) > config.AddBonusPercent ? false : true;
        index = Random.Range(1, 6) * 2;

        if (again)
        {
            index--;
        }

        SetTitleIcon();
    }

    private void Update()
    {
        if (start)
        {
            RouletteBG.transform.Rotate(Vector3.back, speed * Time.deltaTime);

            if (speed > 0)
            {
                speed -= Time.deltaTime * rotateSpeed;
            }
            else
            {
                speed = 0;
                start = false;

                OverMask.gameObject.SetActive(!again);
                AgainMask.gameObject.SetActive(again);
            }

            if (speed < stopSpeed && !end)
            {
                speed = stopSpeed;
                end = CheckIndex();
            }
        }
    }

    private void StartRoulette()
    {
        StartBtn.enabled = false;

        speed = maxSpeed;
        start = true;
        end = false;
    }

    private bool CheckIndex()
    {
        var angles = RouletteBG.transform.eulerAngles.z;
        var min = 30 * (index - 1);
        var max = 30 * index;
        return angles >= min && angles < max;
    }

    private void AddCells()
    {
        ClearCell(Parent);

        for (int i = 0; i < 12; i++)
        {
            var cell = AddCell<RouletteUICell>("Prefabs/UI/RouletteCell", Parent);
            cell.Set(i, i % 2 == 0 ? 0 : 1, 30);
        }
    }
    private void SetTitleIcon()
    {
        if (again)
        {
            int rang = Random.Range(0, 100);
            string resources;
            if (rang < 5)
            {
                resources = "Textures/shop_2d_005";
            }else if (rang < 50)
            {
                resources = "Textures/shop_2d_006";
            }
            else
            {
                resources = "Textures/shop_2d_007";
            }
            TitleIcon.sprite = ReadResources<Sprite>(resources);
        }
        else
        {
            int rang = Random.Range(0, 100);
            string resources = rang < 95 ? "Textures/shop_2d_005" : "Textures/shop_2d_006";
            TitleIcon.sprite = ReadResources<Sprite>(resources);
        }
    }
}
