using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FireBool : MonoBehaviour
{
    CharacterBase attacker;
    CharacterBase target;
    float speed;

    public void Update()
    {
        if (target == null)
            return;
        
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
        transform.LookAt(target.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        var entity = other.GetComponent<CharacterBase>();
        if (entity == null)
        {
            entity.AddImpact(entity, attacker, 1);
        }

        EffectMng.E.AddBattleEffect("FireBollBlast01", 3, target.transform);

        Destroy(gameObject);
    }

    public void SetInfo(CharacterBase attacker, CharacterBase target, float speed)
    {
        this.attacker = attacker;
        this.target = target;
        this.speed = speed;
    }
}
