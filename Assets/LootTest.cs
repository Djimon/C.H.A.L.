using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LootTest : MonoBehaviour
{
    public LootSystem lootsystem;

    public LootTable lootTable;

    [SerializeField]
    public List<RefinedLootTable> AddNewlootTableList;

    private Collider col;
    private Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        col = this.GetComponentInChildren<Collider>();
        rig = this.GetComponentInChildren<Rigidbody>();
        DebugManager.Log($"Collider: {col.gameObject.name} detected");
        DebugManager.Log($"Rigidbody: {rig.gameObject.name} detected");

        lootsystem = FindObjectOfType<LootSystem>();

        List<LootTable> ltl = lootsystem.GetLootTablesByType(ELootTableType.Chest);
        DebugManager.Log(ltl.Count.ToString());
        if(ltl.Count > 0 )
        {
            lootTable = ltl[0];
        }

        DebugManager.Log("Test1", 3, "Test");
        DebugManager.Log("Test2", 3, "Loot");
        DebugManager.Log("Test3", 2, "Achievement");

    }

    // Update is called once per frame
    void Update()
    { 

    }

    private void OnTriggerEnter(Collider other)
    {
        lootsystem.RollLootTable(lootTable);
        Destroy(this.gameObject);
    }

    public void SaveLootTablesToJson()
    {
        LootSerializer serializer = new LootSerializer();
        
        for (int i = 0; i < AddNewlootTableList.Count; i++)
        {
            RefinedLootTable currentLootTable = AddNewlootTableList[i];
            var lootTablename = currentLootTable.Name;
            string filePath = Path.Combine(Application.dataPath, $"Resources/lootTables/{lootTablename}.json");

            serializer.SaveLootTable(currentLootTable, filePath);
        }
        

    }
}
