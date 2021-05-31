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
            Debug.Log("go to map " + DataMng.E.NextSceneID);
            CommonFunction.GoToNextScene(DataMng.E.NextSceneID, DataMng.E.NextSceneName);
        }
    }
}