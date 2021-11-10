using UnityEngine;
using UnityEngine.UI;

public class ExpAdd : UIBase
{
    Text Exp { get => GetComponent<Text>(); }

    float timer = 3;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void Set(int exp)
    {
        Exp.text = "Exp " + exp;
    }
}
