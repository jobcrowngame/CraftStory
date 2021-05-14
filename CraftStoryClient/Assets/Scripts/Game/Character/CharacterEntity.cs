using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public CharacterData CharacterData { get; private set; }

    public void Init(CharacterData characterInfo)
    {
        CharacterData = characterInfo;

        transform.position = CharacterData.Pos;
    }

    //public void Move(Vector3 offset)
    //{
    //    transform.position += offset * moveSpeed;
    //}
}