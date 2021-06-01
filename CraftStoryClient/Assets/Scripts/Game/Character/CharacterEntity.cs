using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    private Transform model;
    public Transform Model { get => model; }


    public virtual void Init()
    {
        model = CommonFunction.FindChiledByName(transform, "Model").transform;
    }

    public void ChangePosition(Vector3 pos)
    {
        transform.position = pos;
        Debug.Log("Change position to " + transform.position);
    }

    public void ModelActive(bool b)
    {
        model.gameObject.SetActive(b);
    }
}