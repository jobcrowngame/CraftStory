using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    private Transform model;
    public Transform Model { get => model; }
    private Animator animator;

    private Transform deleteEffect;

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

        deleteEffect = CommonFunction.FindChiledByName(transform, "DeleteEffect").transform;
    }

    public virtual void ModelActive(bool b)
    {
        model.gameObject.SetActive(b);
    }

    public void EntityBehaviorChange(int stage)
    {
        if (animator == null)
        {
            Logger.Error("not find animator");
            return;
        }

        animator.SetInteger("State", stage);
    }

    public void ShowDestroyEffect(bool b = true)
    {
        if (deleteEffect != null)
        {
            deleteEffect.gameObject.SetActive(b);
        }
    }
}