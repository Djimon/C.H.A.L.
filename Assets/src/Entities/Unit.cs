using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

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

public struct Ally
{
    public Transform transform;
    public float distance;
    public float currentHealth;
    public float currentArmor;
}

public class GroupedEntities
{
    public List<Enemy> Enemies { get; set;}
    public List<Ally> Allies { get; set; }
}

public abstract class Unit : MonoBehaviour

{
    private TroopManager troopManager;

    // New properties for layered update
    [SerializeField]
    public float updateInterval = 1f; // Time in seconds between updates. should not be greater than 1!!
    private float lastUpdateTime = 0f; // Time of last update

    [SerializeField]
    public float MaxHealth = 0;

    [SerializeField]
    public float CurrentHealth = 0;

    [SerializeField] 
    public float AttackPower = 0;

    [SerializeField]
    public float AttackRange = 1; //10-20 good base Measure for melee

    [SerializeField] 
    public float Armor = 0;

    [SerializeField]
    public float FireResistance = 0;

    [SerializeField]
    public float ColdResistance = 0;

    [SerializeField]
    public float LightningResistance = 0;

    [SerializeField]
    public float AggroRadius = 5f;

    [SerializeField]
    public float? CommunicationRadius;

    [SerializeField]
    public float WalkingSpeed = 1f;

    [SerializeField]
    public EUnitSize UnitSize = EUnitSize.unkown;

    [SerializeField]
    public ECombatFocus CombatFocus = ECombatFocus.unkown;

    [SerializeField]
    public int TeamNumber;


    protected bool canWalk = true ;
    protected bool canAttack = true;
   
    private List<Enemy> EnemiesInAggroDistance;
    private List<Enemy> EnemiesInAttackRange;
    [SerializeField]
    public List<Enemy> EnemiesKnown;
    private List<Ally> AlliesInRange;

    protected NavMeshAgent agent;
    public Transform mainTarget; // Assign this in the Inspector
    private Transform currentTarget;
    private Transform newtarget;

    protected virtual void Awake()
    {
        troopManager = FindObjectOfType<TroopManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = WalkingSpeed; 
    }

    protected virtual void OnEnable()
    {
        if(CommunicationRadius == null)
        {
            CommunicationRadius = AggroRadius;
        }
        
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
        var DetectedEntities = DetectEntities();
        EnemiesInAggroDistance = DetectedEntities.Enemies;
        AlliesInRange = DetectedEntities.Allies;

        int foundEnemies = EnemiesInAggroDistance.Count;
        Debug.Log($"found {foundEnemies} enemies.");

        if (foundEnemies > 0)
        {
            MergeEnemiesKnown(EnemiesInAggroDistance);
        }

        //Before Calculating the Target, update position and Helath
        if(EnemiesKnown != null)
        {
            UpdateKnownEnemies();
        }
        

        //BUG: If first target dies, changed to main target and never changed back, evne if Enemie stays Known
        newtarget = foundEnemies switch
        {
            0 => null,
            1 => newtarget = EnemiesKnown[0].transform,
            _ => newtarget = GetTargetByGoal(EnemiesKnown)
        };

        if (currentTarget == null)
        {
            currentTarget = mainTarget;
        }

        if (newtarget != null && newtarget != currentTarget)
        {
            currentTarget = newtarget;
            Debug.Log($"{this.name} changed target to {newtarget.name} ({CombatFocus})");
        }
        

        MoveTo(currentTarget);

        if (currentTarget!=null && CanAttack(currentTarget))
        {
            Attack();
        }

        if (foundEnemies != 0)
        {
            Communicate();
        }

    }

    private void UpdateKnownEnemies()
    {
        List<Enemy> enemyList = new List<Enemy>(EnemiesKnown);
        for (int i = 0; i < enemyList.Count; i++) 
        { 
            Enemy enemy = EnemiesKnown[i];
            if (enemy.transform == null)
            {
                enemyList.Remove(enemy);
                continue;
            }
            enemy.distance = Vector3.Distance(transform.position, enemy.transform.position);
            enemy.currentHealth = enemy.transform.GetComponent<Unit>().CurrentHealth;
            enemy.currentArmor = enemy.transform.GetComponent<Unit>().Armor;
        }
    }

    private void Communicate()
    {
        foreach(Ally ally in AlliesInRange)
        {
            if (ally.transform != null)
            { 
                Unit unit = ally.transform.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.ShareSeenEnemies(EnemiesInAggroDistance);
                }
            }    
        }
        //TODO: NeedHelp if HP is low? only if a healing feature is added
    }

    public void ShareSeenEnemies(List<Enemy> enemiesInAggroDistance)
    {
        //populate all enemies in AggroDistance to other TeamMembers in range
        //Debug.Log("Existance of Enemies has been shared");
        MergeEnemiesKnown(enemiesInAggroDistance);
    }


    public void MergeEnemiesKnown(List<Enemy> enemiesToMerge)
    {
        // If targetList is null, initialize an empty list
        List<Enemy> result = EnemiesKnown != null ? EnemiesKnown : new List<Enemy>();

        // Create a HashSet for quick lookup of known transforms (empty if targetList is null)
        HashSet<Transform> knownTransforms = EnemiesKnown != null ? new HashSet<Transform>(EnemiesKnown.Select(e => e.transform)) : new HashSet<Transform>();

        // Loop through the enemies to merge and add those that aren't already in the HashSet
        for (int i = 0; i < enemiesToMerge.Count; i++)
        {
            Enemy enemy = enemiesToMerge[i]; // Greife auf den aktuellen Enemy zu
            if (knownTransforms.Add(enemy.transform)) // Add returns false if it's already present
            {
                result.Add(enemy);
            }
        }

        // After merging, convert back to an array (if you need to reassign it)
        EnemiesKnown = result;

        // Füge nur die Feinde hinzu, die noch nicht in der Ziel-Liste sind

    }

    public void DealDamage(float dmg, EDamageType type=EDamageType.normal, float piercing =0f)
    {
        float damageAfterArmor = CalculateArmorAbsorbtion(dmg, type, ref piercing);
        Debug.Log($"{gameObject.name} lost {damageAfterArmor} healtpoints.");

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
        if(CurrentHealth <= 0f) 
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
        troopManager.UnregisterUnit(this);
        gameObject.SetActive(false);
        //TODO: Refactor with UnitPools
        Destroy(gameObject);
    }

    protected virtual void Attack()
    {
        // Placeholder: Implement attack logic here
        // Reduce health, trigger animations, etc.on();
        float dmg = CalculateDamageDealt();  
        Debug.Log($"{gameObject.name} attacked {currentTarget.name} with {dmg} damage");
        //TODO: What dmage-type do we have?
        // Do we have piercing bonus?
        // TODO: Chekcen ob meelee oder Range
        currentTarget.GetComponent<Unit>()?.DealDamage(dmg);
        canAttack = false;

    }

    private float CalculateDamageDealt()
    {
        float bonusDamage = 0; //active Runes
        return AttackPower/10 + bonusDamage;
    }

    protected bool CanAttack(Transform target)
    {
        bool result = false;
        //Already claculated once in EnemiesInAggroDistance
        float distance = Vector3.Distance(transform.position, target.position);
        result = distance < AttackRange;
        result &= canAttack;

        return result;
    }


    private Transform GetTargetByGoal(List<Enemy> enemies)
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

    private Transform GetBoss(List<Enemy> enemies)
    {
        Transform target = enemies.FirstOrDefault(e => e.isBoss).transform;
        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetTargetBasedOnArmor(List<Enemy> enemies)
    {
        Transform target = enemies.OrderByDescending(enemy => enemy.currentArmor).FirstOrDefault().transform;
        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetTargetBasedOnLife(List<Enemy> enemies, string v)
    {
        Transform target = v switch
        {
            "high" => enemies.OrderByDescending(enemy => enemy.currentHealth).FirstOrDefault().transform,
            "low" => enemies.OrderBy(enemy => enemy.currentHealth).FirstOrDefault().transform,
            _ => null
        };

        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetNearestTarget(List<Enemy> enemies)
    {
        Transform target = enemies.OrderBy(enemy => enemy.distance).FirstOrDefault().transform;
        return target;
    }

    protected GroupedEntities DetectEntities()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AggroRadius);
        //Debug.Log($"Hit: {hitColliders[0].name} at {hitColliders[0].transform.position} ");

        var enemies = hitColliders
            .Where(collider => collider.CompareTag("Entity") && collider.transform != transform && TeamNumber != collider.GetComponent<Unit>()?.TeamNumber)
            .Select(collider => new Enemy
            {
                transform = collider.transform,
                distance = Vector3.Distance(transform.position, collider.transform.position),
                currentHealth = collider.GetComponent<Unit>()?.MaxHealth ?? 0,
                currentArmor = collider.GetComponent<Unit>()?.Armor ?? 0,
                isBoss = collider.GetComponent<Unit>()?.UnitSize == EUnitSize.huge // Example condition for boss
            }).ToList<Enemy>();

        var allies = hitColliders
            .Where(collider => collider.CompareTag("Entity") && collider.transform != transform && TeamNumber == collider.GetComponent<Unit>()?.TeamNumber)
            .Select(collider => new Ally
            {
                transform = collider.transform,
                distance = Vector3.Distance(transform.position, collider.transform.position),
                currentHealth = collider.GetComponent<Unit>()?.MaxHealth ?? 0,
                currentArmor = collider.GetComponent<Unit>()?.Armor ?? 0
                // Weitere Attribute für Freunde können hier hinzugefügt werden
            }).ToList<Ally>();

        return new GroupedEntities
        {
            Enemies = enemies,
            Allies = allies
        };

    }

    private void MoveTo(Transform target)
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            //Debug.Log($"Set new target: {target.name} at {target.position}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AggroRadius);
    }

}
