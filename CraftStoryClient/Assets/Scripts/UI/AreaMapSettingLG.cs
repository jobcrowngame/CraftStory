

public class AreaMapSettingLG : UILogicBase<AreaMapSettingLG, AreaMapSettingUI>
{
    private string AreaMapDeteriorationMsg = @"サバイバルエリアに設置したオブジェクトが
すべてなくなり、初期化されます。

※ホームエリアは初期化されません

本当に初期化しますか？
";

    public void AreaMapDeterioration(bool b)
    {

        CommonFunction.ShowHintBoxInTitle("サバイバルエリア設定", AreaMapDeteriorationMsg, () =>
         {
             DataMng.E.UserData.IsDeterioration = b;

             FileIO.E.DeleteAreaMap(PublicPar.SaveRootPath);

            UI.Close();
         }, () => { }, "button_2d_064", "button_2d_065");
    }
}