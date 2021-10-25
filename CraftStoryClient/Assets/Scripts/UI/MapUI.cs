using UnityEngine;
using UnityEngine.UI;

public class MapUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button MarketBtn { get => FindChiled<Button>("MarketBtn"); }
    Button BraveBtn { get => FindChiled<Button>("BraveBtn"); }
    Transform SpriteAnim { get => FindChiled("SpriteAnim"); }

    public override void Init()
    {
        base.Init();

        CloseBtn.onClick.AddListener(Close);

        HomeBtn.onClick.AddListener(() =>
        {
            GuideLG.E.Next();

            // �z�[���ɂ���z�[���J�ڂł��܂���
            if (DataMng.E.RuntimeData.MapType == MapType.Home)
                return;

            // �`���r���Ńz�[���߂�ꍇ�A�{�[�i�X�v�Z���܂�
            if (DataMng.E.RuntimeData.MapType == MapType.Brave)
            {
                // ��ɓ��ꂽ�A�C�e�����Ȃ��ꍇ�A�ʐM���Ȃ�
                if (AdventureCtl.E.BonusList.Count <= 0)
                {
                    CommonFunction.GoToNextScene(100);
                    return;
                }

                var ui = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox);
                ui.AddBonus(AdventureCtl.E.BonusList);
                ui.SetCallBack(() =>
                {
                    NWMng.E.GetItems(() =>
                    {
                        CommonFunction.GoToNextScene(100);
                    });
                });
            }
            else
            {
                CommonFunction.GoToNextScene(100);
            }
        });

        MarketBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.RuntimeData.GuideEnd2 == 0)
            {
                DataMng.E.RuntimeData.GuideId = 2;
                CommonFunction.GoToNextScene(104);
            }
            else
            {
                if (DataMng.E.RuntimeData.MapType != MapType.Market)
                    CommonFunction.GoToNextScene(103);
            }
        });

        BraveBtn.onClick.AddListener(() =>
        {
            if (!PlayerCtl.E.Character.IsEquipedEquipment())
            {
                CommonFunction.ShowHintBar(33);
                return;
            }

            // �`������̃~�b�V����
            NWMng.E.ClearMission(2, 1);

            if (DataMng.E.RuntimeData.MapType != MapType.Brave)
                CommonFunction.GoToNextScene(1000);
        });
    }

    public override void Open()
    {
        base.Open();

        EnActiveBtn(HomeBtn, DataMng.E.RuntimeData.MapType == MapType.Home);
        EnActiveBtn(MarketBtn, DataMng.E.RuntimeData.MapType == MapType.Market);
        EnActiveBtn(BraveBtn, DataMng.E.RuntimeData.MapType == MapType.Brave);

        SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide && DataMng.E.RuntimeData.GuideEnd2 == 0);
    }

    private void EnActiveBtn(Button btn, bool b = true)
    {
        btn.GetComponent<Image>().color = b ?
            Color.gray :
            Color.white;
        btn.enabled = !b;
    }
}
