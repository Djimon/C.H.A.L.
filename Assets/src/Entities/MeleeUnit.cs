using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{

    //TODO: Get rid of MAgic Cosntants
    public float MeleeCooldown = 2f;
    
    protected override void Awake()
    {
        base.Awake();
        Createunit(MaxHealth, CurrentHealth, Armor, AggroRadius, UnitSize, updateInterval);
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
        base.Attack();
        Invoke(nameof(ResetAttack), MeleeCooldown); //3 Sekunden Cooldown
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}
