using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    private Transform model;
    public Transform Model { get => model; }
    private Animator animator;

    public PlayerBehavior Behavior
    {
        get
        {
            if (playerBehavior == null)
                playerBehavior = new PlayerBehavior();

            return playerBehavior;
        }
    }
    private PlayerBehavior playerBehavior;

    public virtual void Init()
    {
        model = CommonFunction.FindChiledByName(transform, "Model").transform;
        animator = model.GetComponent<Animator>();
        Behavior.Type = PlayerBehaviorType.Waiting;
    }

    public void ModelActive(bool b)
    {
        model.gameObject.SetActive(b);

        if (b) Behavior.Type = PlayerBehaviorType.Waiting;
    }

    public void EntityBehaviorChange(int stage)
    {
        if (animator == null)
        {
            Debug.LogError("not find animator");
            return;
        }

        animator.SetInteger("State", stage);
    }
}