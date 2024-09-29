using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reptiloid : Unit
{
    private float RangeCooldown = 3.5f;

    protected override void Awake()
    {
        base.Awake();
        Createunit(MaxHealth, CurrentHealth, Armor, AggroRadius, UnitSize, EMonsterType.Reptiloid, updateInterval, false, true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateUnit(); //the TroopManager does this
    }
    protected override void Attack()
    {
        //Can't wlak during shooting
        canWalk = false;
        Invoke(nameof(ResetWalkingSpeed), 1.5f); //TODO: ersetzen mit Dauer der Animation

        base.Attack();
        Invoke(nameof(ResetAttack), RangeCooldown); //3 Sekunden Cooldown
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void ResetWalkingSpeed()
    {
        agent.speed = WalkingSpeed;
    }

}
