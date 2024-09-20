using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopManager : MonoBehaviour
{
    private List<Unit> units = new List<Unit>();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var unit in units)
        {
            unit.UpdateUnit();
        }
    }

    public void RegisterUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void UnregisterUnit(Unit unit)
    {
        units.Remove(unit);
    }

}
