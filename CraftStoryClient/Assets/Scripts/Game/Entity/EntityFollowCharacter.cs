using UnityEngine;

public class EntityFollowCharacter : EntityBase
{
    public Transform target;
    float smoothing = 1;

    bool following = false;

    public override void OnClick()
    {
        base.OnClick();

        Logger.Warning("Clicked " + gameObject.name);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target != null)
        {
            following = Mathf.Abs(Vector3.Distance(target.position, transform.position)) > 0.1f;

            if (following)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothing);

                var dir = target.position - transform.position;
                var angle = CommonFunction.Vector2ToAngle(new Vector2(dir.x, dir.z));
                transform.rotation = Quaternion.Euler(new Vector3(0, -angle + 90, 0));
            }
        }
    }
}