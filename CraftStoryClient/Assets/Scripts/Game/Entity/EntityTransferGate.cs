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
        var player = other.GetComponent<CharacterPlayer>();
        if (player != null)
        {
            AdventureCtl.E.GetBonus(() =>
            {
                GoToNext();
            });
        }
    }

    // 次のダンジョンに行く
    private void GoToNext()
    {
        PlayerCtl.E.Lock = false;
        int nextTransferGateID = 0;

        if (DataMng.E.MapData.Config.TransferGateID == 9999)
        {
            nextTransferGateID = NowLoadingLG.E.BeforTransferGateID;
        }
        else
        {
            nextTransferGateID = DataMng.E.MapData.Config.TransferGateID;
        }

        CommonFunction.GoToNextScene(nextTransferGateID);
        AdventureCtl.E.Clear();
    }
}