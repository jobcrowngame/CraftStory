using JsonConfigData;
using UnityEngine;

public class EntityTransferGate : EntityBase
{
    public TransferGate Config { get => ConfigMng.E.TransferGate[Data.ID]; }

    private void OnTriggerEnter(Collider other)
    {
        var characterCtl = other.GetComponent<CharacterController>();
        if (characterCtl != null)
        {
            NWMng.E.ClearAdventure((rp)=> 
            {
                DataMng.GetItems(rp[0]);
                GiftBoxUI ui = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox) as GiftBoxUI;
                ui.AddBonus(AdventureCtl.E.BonusList);
            },AdventureCtl.E.BonusList);
        }
    }
}