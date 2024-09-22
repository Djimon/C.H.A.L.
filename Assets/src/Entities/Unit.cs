using System;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;
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
    public float WalkingSpeed { get; private set; } = 1f;

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


    private NavMeshAgent agent;
    public Transform mainTarget; // Assign this in the Inspector
    private Transform currentTarget;

    protected virtual void Awake()
    {
        troopManager = FindObjectOfType<TroopManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void OnEnable()
    {
        if (troopManager != null)
        {
            troopManager.RegisterUnit(this);
        }

        currentTarget = mainTarget; // Start by moving towards the main target
        agent.SetDestination(currentTarget.position);
        Debug.Log($"Target locked: {currentTarget.position}");
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

    void Update()
    {
        // Call the UpdateUnit method for layered update functionality
        UpdateUnit();

        //EnemiesInAggroDistance = DetectEnemies();

        //// Wenn es Gegner gibt, wähle ein Ziel und bewege dich hin
        //if (EnemiesInAggroDistance.Length > 0)
        //{
        //    target = GetTargetByGoal(EnemiesInAggroDistance);
        //    MoveTo(target);
        //}

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

        if (target != null)
        {
            MoveTo(target);
        }

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
    }

    private void ShareSeenEnemies(Enemy[] enemiesInAggroDistance, int teamNumber)
    {
        //populate all enemies in AggroDistance to other TeamMembers
        throw new NotImplementedException();
    }

    private void Attack(Transform target)
    {
        // Placeholder: Implement attack logic here
        Debug.Log($"{gameObject.name} is attacking {target.name}");
        // Reduce health, trigger animations, etc.on();
    }

    private bool CanAttack(Transform target)
    {
        // Placeholder: Implement logic to determine if the unit can attack
        // For example, check cooldowns and ranges
        return (cooldown <= 0); // Simplified check, needs more logic based on type of unit (melee/ranged)
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
        Transform target = enemies.FirstOrDefault(e => e.isBoss).transform;
        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetTargetBasedOnArmor(Enemy[] enemies)
    {
        Transform target = enemies.OrderByDescending(enemy => enemy.currentArmor).FirstOrDefault().transform;
        return target ?? GetNearestTarget(enemies);
        return target;
    }

    private Transform GetTargetBasedOnLife(Enemy[] enemies, string v)
    {
        Transform target = v switch
        {
            "high" => enemies.OrderByDescending(enemy => enemy.currentHealth).FirstOrDefault().transform,
            "low" => enemies.OrderBy(enemy => enemy.currentHealth).FirstOrDefault().transform,
            _ => null
        };

        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetNearestTarget(Enemy[] enemies)
    {
        Transform target = enemies.OrderBy(enemy => enemy.distance).FirstOrDefault().transform;
        return target;
    }

    protected Enemy[] DetectEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AggroRadius);
        return hitColliders
            .Where(collider => collider.CompareTag("Entity") && collider.transform != this.transform)
            .Select(collider => new Enemy
            {
                transform = collider.transform,
                distance = Vector3.Distance(transform.position, collider.transform.position),
                currentHealth = collider.GetComponent<Unit>()?.MaxHealth ?? 0,
                isBoss = collider.GetComponent<Unit>()?.UnitSize == EUnitSize.huge // Example condition for boss
            }).ToArray(); ;
    }

    private void MoveTo(Transform target)
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AggroRadius);
    }

}
