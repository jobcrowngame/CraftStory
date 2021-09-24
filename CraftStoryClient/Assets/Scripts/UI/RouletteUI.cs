using UnityEngine;
using UnityEngine.UI;

public class RouletteUI : UIBase
{
    Image TitleIcon { get => FindChiled<Image>("TitleIcon"); }
    Image RouletteBG { get => FindChiled<Image>("RouletteBG"); }
    Button StartBtn { get => GetComponent<Button>(); }
    Button OverMask { get => FindChiled<Button>("OverMask"); }
    Button AgainMask { get => FindChiled<Button>("AgainMask"); }

    bool again;
    bool start;
    int index;
    float speed;
    float stopAngle;
    const float minSpeed = 0.3f;
    public float curAngle;
    float targetAngle;

    public override void Init(object gachaId)
    {
        base.Init(gachaId);
        RouletteLG.E.Init(this);

        StartBtn.onClick.AddListener(StartRoulette);
        OverMask.onClick.AddListener(()=> 
        {
            AudioMng.E.ShowBGM("bgm_01");

            Close();
        });
        AgainMask.onClick.AddListener(()=> 
        {
            NWMng.E.GachaAddBonusAgain((rp) => 
            {
                int index = (int)rp["index"];
                var ui = UICtl.E.OpenUI<GachaAddBonusUI>(UIType.GachaAddBonus);
                ui.Set(index, (int)gachaId);
            }, (int)gachaId);
            Close();
        });
    }

    public override void Open(object gachaId)
    {
        base.Open(gachaId);
        StartBtn.enabled = true;
        start = false;
        RouletteBG.transform.eulerAngles = Vector3.zero;

        OverMask.gameObject.SetActive(false);
        AgainMask.gameObject.SetActive(false);

        var config = ConfigMng.E.Gacha[(int)gachaId];
        again = Random.Range(0, 1000) > config.AddBonusPercent ? false : true;
        index = Random.Range(1, 6) * 2;

        if (!again)
        {
            index--;
        }

        SetTitleIcon();
    }

    public override void Close()
    {
        base.Close();
        NWMng.E.GetItems();
    }

    private void Update()
    {
        if (start)
        {
            curAngle -= speed;
            targetAngle += speed;
            RouletteBG.transform.eulerAngles = new Vector3(0, 0, -targetAngle);

            if (curAngle > 0)
            {
                speed = GetSpeed();
            }
            else
            {
                speed = 0;
                start = false;

                OverMask.gameObject.SetActive(!again);
                AgainMask.gameObject.SetActive(again);

                string seName = again ? "HitSE" : "OffSE";
                AudioMng.E.ShowSE(seName);
            }
        }
    }

    private void StartRoulette()
    {
        StartBtn.enabled = false;
        curAngle = 360 * 3 - index * 30;
        targetAngle = 0;
        Logger.Log("Index:{0}, StopAngle:{1}", index, curAngle);
        start = true;
        RouletteBG.transform.eulerAngles = Vector3.zero;

        speed = GetSpeed();

        AudioMng.E.ShowSE("rouletteSE");
    }
    private float GetSpeed()
    {
        float newSpeed = curAngle / 50;
        if (newSpeed < minSpeed)
        {
            newSpeed = minSpeed;
        }
        return newSpeed;
    }
    private void SetTitleIcon()
    {
        if (again)
        {
            int rang = Random.Range(0, 100);
            string resources;
            TitleIcon.transform.GetChild(0).gameObject.SetActive(false);

            if (rang < 5)
            {
                resources = "Textures/shop_2d_20";
            }else if (rang < 50)
            {
                resources = "Textures/shop_2d_21";
            }
            else
            {
                resources = "Textures/shop_2d_22";
                TitleIcon.transform.GetChild(0).gameObject.SetActive(true);
            }
            TitleIcon.sprite = ReadResources<Sprite>(resources);
        }
        else
        {
            int rang = Random.Range(0, 100);
            string resources = rang < 95 ? "Textures/shop_2d_20" : "Textures/shop_2d_21";
            TitleIcon.sprite = ReadResources<Sprite>(resources);
        }
    }
}
