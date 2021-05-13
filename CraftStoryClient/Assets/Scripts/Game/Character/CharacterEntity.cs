using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    private CharacterInfo cInfo;

    public void Init(CharacterInfo characterInfo)
    {
        cInfo = characterInfo;

        transform.position = cInfo.Pos;
    }

    //public void Move(Vector3 offset)
    //{
    //    transform.position += offset * moveSpeed;
    //}
}