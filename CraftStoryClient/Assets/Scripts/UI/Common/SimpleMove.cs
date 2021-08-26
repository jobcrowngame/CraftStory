using UnityEngine;

/// <summary>
/// クラフト場合、簡単なUI移動アニメション
/// </summary>
public class SimpleMove : MonoBehaviour
{
    Vector2 endPos;
    public void Set(Vector2 targetPos)
    {
        endPos = targetPos;
    }

    private void Update()
    {
        if (endPos != Vector2.zero)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, endPos, 20f);
            if (Vector2.Distance(transform.position, endPos) < 1)
            {
                Destroy(this.gameObject);
            }
        }
    }
}