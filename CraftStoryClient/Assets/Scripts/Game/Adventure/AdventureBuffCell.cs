
using JsonConfigData;
using UnityEngine;

public class AdventureBuffCell : CharacterBase
{
    AdventureBuff config;

    private void OnTriggerEnter(Collider other)
    {
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

    public void Set(AdventureBuff config)
    {
        this.config = config;
    }

    private void UserSkill()
    {
        var skill = new SkillData(config.Skill);
        StartUseSkill((CharacterGroup)config.TargetGroup, skill);
    }
}