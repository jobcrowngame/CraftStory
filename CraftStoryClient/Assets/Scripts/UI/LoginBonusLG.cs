using System;
using System.Collections.Generic;
using LitJson;

public class LoginBonusLG : UILogicBase<LoginBonusLG, LoginBonusUI>
{
    LoginBonusInfoRP result;
    int index = 0;

    public void GetInfo()
    {
        NWMng.E.GetLoginBonusInfo((rp) =>
        {
            result = JsonMapper.ToObject<LoginBonusInfoRP>(rp.ToJson());

            OpenNextUI();
        });
    }

    public void OpenNextUI()
    {
        if (result.arr.Count ==0 || index == result.arr.Count)
        {
            // ログインボーナス完了後妖精を出す。
            PlayerCtl.E.Fairy.ShowFairy();
            return;
        }

        var Type = result.arr[index].type;
        LoginBonusInfoCellRP info = result.arr[index];

        int step = 0;
        if (!string.IsNullOrEmpty(result.loginBonus))
        {
            string[] bonus = result.loginBonus.Split(',');
            string[] bonusStep = result.loginBonusStep.Split(',');

            for (int i = 0; i < bonus.Length; i++)
            {
                if (int.Parse(bonus[i]) == info.id)
                {
                    step = int.Parse(bonusStep[i]);
                    break;
                }
            }
        }

        index++;

        var ui = UICtl.E.OpenUI<LoginBonusUI>(UIType.LoginBonus);
        ui.Set(info, step);
    }

    public struct LoginBonusInfoRP
    {
        public string loginBonus { get; set; }
        public string loginBonusStep { get; set; }
        public List<LoginBonusInfoCellRP> arr { get; set; }
    }

    public struct LoginBonusInfoCellRP
    {
        public int id { get; set; }
        public int type { get; set; }
        public string themeTexture { get; set; }
        public string items { get; set; }
        public string itemCounts { get; set; }
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
    }
}