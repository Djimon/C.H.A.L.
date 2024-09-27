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
        // Verwende eine temporäre Liste, um Modifikationen während der Iteration zu vermeiden
        List<Unit> unitsToUpdate = new List<Unit>(units);

        foreach (var unit in unitsToUpdate)
        {
            if (unit != null)
            {
                unit.UpdateUnit();
            }
        }

        // Optional: Bereinige 'null'-Einträge (falls Einheiten unerwartet null werden)
        units.RemoveAll(unit => unit == null);
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit != null && !units.Contains(unit))
        {
            units.Add(unit);
        }
    }

    public void UnregisterUnit(Unit unit)
    {
        if (unit != null && units.Contains(unit))
        {
            units.Remove(unit);
        }
    }

}
