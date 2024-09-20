using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    protected override void Awake()
    {
        base.Awake();
        Createunit(MaxHealth, CurrentHealth, AggroRadius, UnitSize, updateInterval);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
