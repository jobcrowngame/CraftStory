using JsonConfigData;
using UnityEngine;

public class EntityTransferGate : EntityBase
{
    public int TransferGateID { get => transferGateID; }
    private int transferGateID;

    public TransferGate Config { get => ConfigMng.E.TransferGate[transferGateID]; }

    private void OnTriggerEnter(Collider other)
    {
        var characterCtl = other.GetComponent<CharacterController>();
        if (characterCtl != null)
        {
            NWMng.E.ClearAdventure((rp)=> 
            {
                DataMng.GetItems(rp);
                GiftBoxUI ui = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox);
                ui.AddBonus(AdventureCtl.E.BonusList);
            },AdventureCtl.E.BonusList);
        }
    }
}