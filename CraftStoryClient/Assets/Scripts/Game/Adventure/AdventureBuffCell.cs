
using JsonConfigData;
using UnityEngine;

public class AdventureBuffCell : CharacterBase
{
    AdventureBuff config;
    Vector3 targetPos;
    bool isActived;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActived)
            return;

        if ((CharacterGroup)config.TargetGroup == CharacterGroup.Player)
        {
            var character = other.GetComponent<CharacterBase>();
            if (character != null && character.Group == CharacterGroup.Player)
            {
                Model.gameObject.SetActive(false);
                gameObject.GetComponent<Collider>().enabled = false;
                UserSkill();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isActived)
        {
            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position += Vector3.down * 0.03f;
            }
            else
            {
                isActived = true;
            }
        }
    }

    public void Set(AdventureBuff config)
    {
        this.config = config;

        targetPos = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);

        isActived = false;
    }

    private void UserSkill()
    {
        var skill = new SkillData(config.Skill);
        StartUseSkill((CharacterGroup)config.TargetGroup, skill);
    }
}