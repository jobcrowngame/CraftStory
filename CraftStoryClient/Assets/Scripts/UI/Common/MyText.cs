using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyText : MonoBehaviour
{
    Text mText { get => CommonFunction.FindChiledByName<Text>(transform, "Text"); }

    public string text { set => mText.text = value; }
}
