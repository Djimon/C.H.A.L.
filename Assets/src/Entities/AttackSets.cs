using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackSet 
{
    void Attack();

}

public class MeleeAttackStrategy : IAttackSet
{
    public void Attack()
    {
        Debug.Log("Nahkampfangriff.");
    }
}

public class RangedAttackStrategy : IAttackSet
{
    public void Attack()
    {
        Debug.Log($"Fernkampfangriff mit.");
    }
}
