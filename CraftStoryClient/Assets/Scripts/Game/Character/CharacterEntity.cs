using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public CharacterData CharacterData { get; private set; }

    public void Init(CharacterData characterInfo)
    {
        CharacterData = characterInfo;

        ChangePosition(CharacterData.Pos);
    }

    private void ChangePosition(Vector3 pos)
    {
        Debug.Log("Change position to " + pos);
        transform.position = CharacterData.Pos;
    }

    //public void Move(Vector3 offset)
    //{
    //    transform.position += offset * moveSpeed;
    //}
}