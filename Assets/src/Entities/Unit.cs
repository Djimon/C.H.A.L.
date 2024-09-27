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

public enum EMonsterType
{
    undefined = -1,
    Player,
    Chitinoid,
    Reptiloid,
    Beastoid
}

public struct Enemy
{
    public Transform Transform;
    public float Distance;
    public float currentHealth;
    public float currentArmor;
    public bool isBoss;
}

public struct Ally
{
    public Transform Transform;
    public float Distance;
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
    public EMonsterType MonsterType = EMonsterType.undefined;

    [SerializeField]
    public int TeamNumber;

    private IAttackSet MeleeAttack;
    private IAttackSet RangeAttack;
    private IAttackSet SpecialAttack;


    protected bool canWalk = true ;
    protected bool canAttack = true;
   
    private List<Enemy> EnemiesInAggroDistance;
    private List<Enemy> EnemiesInAttackRange;
    [SerializeField]
    private List<Enemy> EnemiesKnown;
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

    protected void Createunit(float maxHealth, float currentHealth, float armor, float aggroRadius, EUnitSize size, EMonsterType monsterType, float interval, bool canMelee = true, bool canRange = false)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Armor = armor;
        AggroRadius = aggroRadius;
        UnitSize = size;
        updateInterval = interval;
        MonsterType = monsterType;

        MeleeAttack = canMelee ? new MeleeAttackStrategy(): null;
        RangeAttack = canRange ? new RangedAttackStrategy(): null;

        //SpecialAttack will allways be activated later in the game
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
        int knownEnemies = 0;
        if (EnemiesKnown != null)
        {
            EnemiesKnown = UpdateKnownEnemies();
            knownEnemies = EnemiesKnown.Count;
        }        
        
        newtarget = knownEnemies switch
        {
            0 => newtarget = mainTarget,
            1 => newtarget = EnemiesKnown[0].Transform,
            _ => newtarget = GetTargetByGoal() //BUG TODO: Seems to not be called everytime?
        };

        if (newtarget != currentTarget)
        {
            currentTarget = newtarget;
            Debug.Log($"{this.name} changed target to {newtarget.name} ({CombatFocus})");
        }
        

        MoveTo(currentTarget);

        if (currentTarget!=null && CanAttack(currentTarget))
        {
            Attack();
        }

        if (knownEnemies > 0)
        {
            Communicate();
        }

    }

    private List<Enemy> UpdateKnownEnemies()
    {
        List<Enemy> enemyList = new List<Enemy>(EnemiesKnown);
        for (int i = 0; i < enemyList.Count; i++) 
        { 
            Enemy enemy = EnemiesKnown[i];
            if (enemy.Transform == null)
            {
                enemyList.Remove(enemy);
                continue;
            }
            enemy.Distance = Vector3.Distance(transform.position, enemy.Transform.position);
            enemy.currentHealth = enemy.Transform.GetComponent<Unit>().CurrentHealth;
            enemy.currentArmor = enemy.Transform.GetComponent<Unit>().Armor;
            Debug.Log($"{name} knows: {enemy.Transform.gameObject.name} with distance {enemy.Distance} at {enemy.currentHealth} HP");
        }
        return new List<Enemy>(enemyList);
    }

    private void Communicate()
    {
        foreach(Ally ally in AlliesInRange)
        {
            if (ally.Transform != null)
            { 
                Unit unit = ally.Transform.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.ShareSeenEnemies(EnemiesInAggroDistance);
                }
            }    
        }
        //TODO: NeedHelp if HP is low? only if a healing feature is added
    }

    //TODO
    //BUG: zwischen verschiedenen Playern wird dieselbe Referenz der EnemiesKnown geshared, ejde rspieler sollte aber siene individuelle Lsite mit den individuellen distances haben
    public void ShareSeenEnemies(List<Enemy> enemiesInAggroDistance)
    {
        //create new List to not share the same reference?
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < enemiesInAggroDistance.Count; i++)
        {
            if (enemiesInAggroDistance[i].Transform == null)
            {
                enemies.Remove(enemiesInAggroDistance[i]);
                continue;
            }

            float dist = Vector3.Distance(transform.position, enemiesInAggroDistance[i].Transform.position);
            //make new Enemy to prevent overriting the distance of allies Enemies-List.
            Enemy newEnemy = new Enemy();
            newEnemy.Transform = enemiesInAggroDistance[i].Transform;
            newEnemy.Distance = dist;
            newEnemy.currentHealth = enemiesInAggroDistance[i].Transform.GetComponent<Unit>().CurrentHealth;
            newEnemy.currentArmor = enemiesInAggroDistance[i].Transform.GetComponent<Unit>().Armor;
            newEnemy.isBoss = enemiesInAggroDistance[i].isBoss;


            enemies.Add(newEnemy);
            //Debug.Log($"{name} knows: {newEnemy.Transform.gameObject.name} with distance {newEnemy.Distance} at {newEnemy.currentHealth} HP");
        }

        MergeEnemiesKnown(enemies);      
    }

    public void MergeEnemiesKnown(List<Enemy> enemiesToMerge)
    {
        // If targetList is null, initialize an empty list
        List<Enemy> result = EnemiesKnown != null ? EnemiesKnown : new List<Enemy>();

        // Create a HashSet for quick lookup of known transforms (empty if targetList is null)
        HashSet<Transform> knownTransforms = EnemiesKnown != null ? new HashSet<Transform>(EnemiesKnown.Select(e => e.Transform)) : new HashSet<Transform>();

        // Loop through the enemies to merge and add those that aren't already in the HashSet
        for (int i = 0; i < enemiesToMerge.Count; i++)
        {
            Enemy enemy = enemiesToMerge[i]; // Greife auf den aktuellen Enemy zu
            if (knownTransforms.Add(enemy.Transform)) // Add returns false if it's already present
            {
                result.Add(enemy);
            }
        }

        EnemiesKnown = new List<Enemy>(result);
    }

    public void DealDamage(float dmg, Unit source, EDamageType type=EDamageType.normal, float piercing =0f)
    {
        float damageAfterArmor = CalculateArmorAbsorbtion(dmg, type, ref piercing);
        Debug.Log($"{gameObject.name} lost {damageAfterArmor} healtpoints.");

        SubstractHealth(damageAfterArmor, source);
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

    private void SubstractHealth(float damageAfterArmor, Unit source)
    {
        CurrentHealth -= damageAfterArmor;
        if(CurrentHealth <= 0f) 
        {
            CurrentHealth = 0f;
            source.GetKill(this);
            Die();
        }
    }

    public void GetKill(Unit unit)
    {
        String victimName  = unit.name;
        EUnitSize victimSize = unit.UnitSize;
        
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
        currentTarget.GetComponent<Unit>()?.DealDamage(dmg, this);
        canAttack = false;

        //TODO: Refactor to decide if use PerformMeleeAttack oder PerformRangeAttack oder PerformSpecialAttack

    }

    protected void PerformMeleeAttack()
    {
        MeleeAttack.Attack();
    }

    protected void PerformRangeAttack()
    {
        RangeAttack.Attack();
    }

    protected void PerformSpecialAttack()
    {
        SpecialAttack.Attack();
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


    private Transform GetTargetByGoal()
    {

        return CombatFocus switch
        {
            ECombatFocus.unkown => GetNearestTarget(EnemiesKnown),
            ECombatFocus.nearest => GetNearestTarget(EnemiesKnown),
            ECombatFocus.highestLife => GetTargetBasedOnLife(EnemiesKnown, "high"),
            ECombatFocus.lowestLife => GetTargetBasedOnLife(EnemiesKnown, "low"),
            ECombatFocus.Armored => GetTargetBasedOnArmor(EnemiesKnown),
            ECombatFocus.Boss => GetBoss(EnemiesKnown),
            _ => null
        };
    }

    private Transform GetBoss(List<Enemy> enemies)
    {
        Transform target = enemies.FirstOrDefault(e => e.isBoss).Transform;
        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetTargetBasedOnArmor(List<Enemy> enemies)
    {
        Transform target = enemies.OrderByDescending(enemy => enemy.currentArmor).FirstOrDefault().Transform;
        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetTargetBasedOnLife(List<Enemy> enemies, string v)
    {
        Transform target = v switch
        {
            "high" => enemies.OrderByDescending(enemy => enemy.currentHealth).FirstOrDefault().Transform,
            "low" => enemies.OrderBy(enemy => enemy.currentHealth).FirstOrDefault().Transform,
            _ => null
        };

        return target ?? GetNearestTarget(enemies);
    }

    private Transform GetNearestTarget(List<Enemy> enemies)
    {
        List<Enemy> allenemies = enemies.OrderBy(enemy => enemy.Distance).ToList<Enemy>();
        
        for (int i = 0; i < allenemies.Count; i++)
        {
            var enemy = allenemies[i];
            Debug.Log($"{name} targetOption: {enemy.Transform.name}, Distance: {enemy.Distance}");
        }

        Transform target = allenemies.FirstOrDefault().Transform;

        if (target != null)
        {
            Debug.Log($"Selected Target: {target.name}");
        }

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
                Transform = collider.transform,
                Distance = Vector3.Distance(transform.position, collider.transform.position),
                currentHealth = collider.GetComponent<Unit>()?.MaxHealth ?? 0,
                currentArmor = collider.GetComponent<Unit>()?.Armor ?? 0,
                isBoss = collider.GetComponent<Unit>()?.UnitSize == EUnitSize.huge // Example condition for boss
            }).ToList<Enemy>();

        var allies = hitColliders
            .Where(collider => collider.CompareTag("Entity") && collider.transform != transform && TeamNumber == collider.GetComponent<Unit>()?.TeamNumber)
            .Select(collider => new Ally
            {
                Transform = collider.transform,
                Distance = Vector3.Distance(transform.position, collider.transform.position),
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


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

}
