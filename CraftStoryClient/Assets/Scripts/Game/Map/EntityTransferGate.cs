using JsonConfigData;
using UnityEngine;

/// <summary>
/// 転送門
/// </summary>
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
            GiftBoxUI ui = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox);
            ui.AddBonus(AdventureCtl.E.BonusList);
        }
    }
}