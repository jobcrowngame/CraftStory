using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public virtual void Init()
    {
    }

    public void ChangePosition(Vector3 pos)
    {
        transform.position = pos;
        Debug.Log("Change position to " + transform.position);
    }
}