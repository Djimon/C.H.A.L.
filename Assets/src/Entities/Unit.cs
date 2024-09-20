using System;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering;

public enum EUnitSize
{
    unkown =-1,
    small,
    medium,
    large,
    huge
}

public enum ECombatFocus
{
    unkown =-1,
    nearest,
    highestLife,
    lowestLife,
    Armored,
    Boss
}

public struct Enemy
{
    public Transform transform;
    public float distance;
    public float currentHealth;
    public float currentArmor;
    public bool isBoss;
}

public abstract class Unit : MonoBehaviour

{
    private TroopManager troopManager;

    // New properties for layered update
    [SerializeField]
    public float updateInterval { get; private set; } = 1f; // Time in seconds between updates
    private float lastUpdateTime = 0f; // Time of last update

    [SerializeField]
    public float MaxHealth { get; private set; } = 0;

    [SerializeField]
    public float CurrentHealth { get; private set; } = 0;

    [SerializeField]
    public float AggroRadius { get; private set; } = 1;

    [SerializeField]
    public EUnitSize UnitSize { get; private set; }

    [SerializeField]
    public ECombatFocus CombatFocus { get; set; }

    [SerializeField]
    public int TeamNumber = 0;


    private bool canWalk = true ;
    private float cooldown = 0f;

    private float MeleeCooldown = 0f;
    private float RangeCooldown = 0f;
    private Transform target;
    private Enemy[] EnemiesInAggroDistance;
    private Enemy[] EnemiesInAttackRange;
    private Enemy[] EnemiesKnownFromTeam;

    protected virtual void Awake()
    {
        troopManager = FindObjectOfType<TroopManager>();
    }

    protected virtual void OnEnable()
    {
        if (troopManager != null)
        {
            troopManager.RegisterUnit(this);
        }
    }

    protected virtual void OnDisable()
    {
        if (troopManager != null)
        {
            troopManager.UnregisterUnit(this);
        }
    }

    protected void Createunit(float maxHealth, float currentHealth, float aggroRadius, EUnitSize size, float interval)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        AggroRadius = aggroRadius;
        UnitSize = size;
        updateInterval = interval;
    }

    public void UpdateUnit()
    {
        if (CanThink())
        {
            Think();
            lastUpdateTime = Time.time;
        }
    }

    private bool CanThink()
    {
        return Time.time >= lastUpdateTime + updateInterval;
    }

    private void Think()
    {
        //1. Search Enemy
        EnemiesInAggroDistance = DetectEnemies();

        int foundEnemies = EnemiesInAggroDistance.Length;

        target = foundEnemies switch
        {
            0 => null,
            1 => target = EnemiesInAggroDistance[0].transform,
            _ => target = GetTargetByGoal(EnemiesInAggroDistance)
        };

        if (target!=null && CanAttack(target))
        {
            Attack(target);
        }

        Communicate();       

    }

    private void Communicate()
    {
        //ToDo placeholder for other things a troop can share
        ShareSeenEnemies(EnemiesInAggroDistance,TeamNumber);
        throw new NotImplementedException();
    }

    private void ShareSeenEnemies(Enemy[] enemiesInAggroDistance, int teamNumber)
    {
        //populate all enemies in AggroDistance to other TeamMembers
        throw new NotImplementedException();
    }

    private void Attack(Transform target)
    {
        //  -> can walk?
        //  -> attack duration
        //  -> 
        throw new NotImplementedException();
    }

    private bool CanAttack(Transform target)
    {
        //  -> If Cooldown -> false
        //  -> If (ranged && target < range && target > minRange) true
        //  -> ff (melee && target < meleerange) true
        //  -> else false
        throw new NotImplementedException();
    }

    private Transform GetTargetByGoal(Enemy[] enemies)
    {
        return CombatFocus switch
        {
            ECombatFocus.unkown => GetNearestTarget(enemies),
            ECombatFocus.nearest => GetNearestTarget(enemies),
            ECombatFocus.highestLife => GetTargetBasedOnLife(enemies, "high"),
            ECombatFocus.lowestLife => GetTargetBasedOnLife(enemies, "low"),
            ECombatFocus.Armored => GetTargetBasedOnArmor(enemies),
            ECombatFocus.Boss => GetBoss(enemies),
            _ => null
        };
        
        
        throw new NotImplementedException();
    }

    private Transform GetBoss(Enemy[] enemies)
    {
        Transform target = null;

        //TODO: implement

        if (target == null)
        {
            target = GetNearestTarget(enemies);
        }

        return target;
    }

    private Transform GetTargetBasedOnArmor(Enemy[] enemies)
    {
        Transform target = null;

        //TODO: implement

        if (target == null)
        {
            target = GetNearestTarget(enemies);
        }

        return target;
    }

    private Transform GetTargetBasedOnLife(Enemy[] enemies, string v)
    {
        Transform target = null;

        target = v switch
        {
            "high" => enemies.OrderByDescending(enemy => enemy.currentHealth).ToArray()[0].transform,
            "low" => enemies.OrderBy(enemy => enemy.currentHealth).ToArray()[0].transform,
            _ => target
        };


        if (target == null)
        {
            target = GetNearestTarget(enemies);
        }

        return target;
    }

    private Transform GetNearestTarget(Enemy[] enemies)
    {
        throw new NotImplementedException();
    }

    protected Enemy[] DetectEnemies()
    {
        //TODO implement
        //physics.OverlapSphere

        // Check if found Entities is are different Team
        
        
        throw new NotImplementedException();
    }

    private void MoveTo(Transform target)
    {
       
    }

}
