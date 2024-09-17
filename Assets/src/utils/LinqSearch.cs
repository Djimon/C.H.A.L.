using System.Collections.Generic;
using System.Linq;

static public class LinqSearch
{
    
    static public List<LootTable> GetLootTablesByType(ELootTableType lootType, List<LootTable> lootTables)
    {
        return lootTables.Where(lt => lt.lootType == lootType).ToList();
    }

}
   