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

public enum EDamageType
{
    normal = 1,
    fire,
    cold,
    lightning
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
    public float updateInterval { get; set; } = 2f; // Time in seconds between updates
    private float lastUpdateTime = 0f; // Time of last update

    [SerializeField]
    public float MaxHealth = 0;

    [SerializeField]
    public float CurrentHealth = 0;

    [SerializeField] 
    public float AttackPower = 0;

    [SerializeField] 
    public float Armor = 0;

    [SerializeField]
    public float FireResistance = 0;

    [SerializeField]
    public float ColdResistance = 0;

    [SerializeField]
    public float LightningResistance = 0;

    [SerializeField]
    public float AggroRadius = 1;

    [SerializeField]
    public float WalkingSpeed = 1f;

    [SerializeField]
    public EUnitSize UnitSize = EUnitSize.unkown;

    [SerializeField]
    public ECombatFocus CombatFocus = ECombatFocus.unkown;

    [SerializeField]
    public int TeamNumber = 0;


    private bool canWalk = true ;
    private float cooldown = 0f;

    private float MeleeCooldown = 0f;
    private float RangeCooldown = 0f;
    
    private Enemy[] EnemiesInAggroDistance;
    private Enemy[] EnemiesInAttackRange;
    private Enemy[] EnemiesKnownFromTeam;


    private NavMeshAgent agent;
    public Transform mainTarget; // Assign this in the Inspector
    private Transform currentTarget;
    private Transform newtarget;

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

    protected void Createunit(float maxHealth, float currentHealth, float armor, float aggroRadius, EUnitSize size, float interval)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Armor = armor;
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
        Debug.Log($"found {foundEnemies} enemies.");

        newtarget = foundEnemies switch
        {
            0 => null,
            1 => newtarget = EnemiesInAggroDistance[0].transform,
            _ => newtarget = GetTargetByGoal(EnemiesInAggroDistance)
        };

        if (newtarget != null && newtarget != currentTarget)
        {
            currentTarget = newtarget;
            MoveTo(currentTarget);
        }

        if (currentTarget!=null && CanAttack(currentTarget))
        {
            Attack(currentTarget);
        }

        if (foundEnemies != 0)
        {
            Communicate();
        }

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

    public void DealDamage(float dmg, EDamageType type=EDamageType.normal, float piercing =0f)
    {
        float damageAfterArmor = CalculateArmorAbsorbtion(dmg, type, ref piercing);

        SubstractHealth(damageAfterArmor);
    }

    private float CalculateArmorAbsorbtion(float dmg, EDamageType type, ref float piercing)
    {
        piercing = Mathf.Clamp(piercing, 0f, 1f);
        float effectiveArmor = Armor;
        // Je 10 Puntke Armor, wird 1 Schaden absorbiert
        float damageAfterArmor = dmg;

        float resistance = type switch
        {
            EDamageType.normal => Armor,
            EDamageType.fire => FireResistance,
            EDamageType.cold => ColdResistance,
            EDamageType.lightning => LightningResistance,
            _ => Armor
        };

        //Rüstungsdruchdringung abziehen
        effectiveArmor = resistance * (1 - piercing);
        // Je 10 Puntke Armor, wird 1 Schaden absorbiert
        damageAfterArmor = Mathf.Max(0f, dmg - (effectiveArmor / 10));
        return damageAfterArmor;
    }

    private void SubstractHealth(float damageAfterArmor)
    {
        CurrentHealth -= damageAfterArmor;
        if(CurrentHealth < 0f) 
        {
            CurrentHealth = 0f;
            Die();
        }
    }

    private void AddHealth(float health)
    {
        CurrentHealth += health;
    }

    private void Die()
    {
        Debug.Log($"{this.name} died!");
        //TODO: Refactor UnitPools
        Destroy(gameObject);
    }

    private void Attack(Transform target)
    {
        // Placeholder: Implement attack logic here
        Debug.Log($"{gameObject.name} is attacking {target.name}");
        // Reduce health, trigger animations, etc.on();
        float dmg = CalculateDamageDealt();
        //TODO: What dmage-type do we have?
        // Do we have piercing bonus?
        target.GetComponent<Unit>()?.DealDamage(dmg);

        //TODO get cooldown intellgient
        cooldown = 3f;

    }

    private float CalculateDamageDealt()
    {
        float bonusDamage = 0; //active Runes
        return AttackPower += bonusDamage;
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
        //Debug.Log($"Hit: {hitColliders[0].name} at {hitColliders[0].transform.position} ");
        return hitColliders
            .Where(collider => collider.CompareTag("Entity") && collider.transform != transform && TeamNumber != collider.GetComponent<Unit>()?.TeamNumber)
            .Select(collider => new Enemy
            {
                transform = collider.transform,
                distance = Vector3.Distance(transform.position, collider.transform.position),
                currentHealth = collider.GetComponent<Unit>()?.MaxHealth ?? 0,
                currentArmor = collider.GetComponent<Unit>()?.Armor ?? 0,
                isBoss = collider.GetComponent<Unit>()?.UnitSize == EUnitSize.huge // Example condition for boss
            }).ToArray(); ;
    }

    private void MoveTo(Transform target)
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            Debug.Log($"Set new target: {target.name} at {target.position}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AggroRadius);
    }

}
