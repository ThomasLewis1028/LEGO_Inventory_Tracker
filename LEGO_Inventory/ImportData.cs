using System.Text.Json.Nodes;
using LEGO_Inventory.Database;

namespace LEGO_Inventory;

public class ImportData
{
    public bool ImportSetInfo(string setId)
    {
        RebrickableApi api = new RebrickableApi();
        
        JsonObject? setInfo = api.GetSetInfo(setId).Result;

        using (var context = new InventoryContext())
        {
            var setContext = context.Set<Set>();

            if (setContext.Any(s => s.SetId == setId))
            {
                var set = setContext.First(s => s.SetId == setId);
                
                if(set.DateModified >= DateTime.Parse(setInfo!["last_modified_date"].ToString()))
                {
                    set.Name = setInfo!["name"].ToString();
                    set.SetImg = setInfo!["set_img_url"].ToString();
                    set.SetURL = setInfo!["set_url"].ToString();
                    set.DateModified = DateTime.Parse(setInfo!["last_modified_dt"].ToString());
                    set.NumParts = int.Parse(setInfo!["num_parts"].ToString());
                    set.ReleaseYear = int.Parse(setInfo!["year"].ToString());
                }
            }
            else
            {
                var set = new Set
                {
                    SetId = setInfo!["set_num"].ToString(),
                    Name = setInfo!["name"].ToString(),
                    SetURL = setInfo!["set_url"].ToString(),
                    SetImg = setInfo!["set_img_url"].ToString(),
                    DateModified = DateTime.Parse(setInfo!["last_modified_dt"].ToString()),
                    NumParts = int.Parse(setInfo!["num_parts"].ToString()),
                    ReleaseYear = int.Parse(setInfo!["year"].ToString()),
                    ManualPath = "",
                    ManualURL = "",
                    OwnCount = 0,
                    BuildCount = 0
                };
                
                setContext.Add(set);
            }
            
            return context.SaveChanges() > 0;
        }
        
        return false;
    }
}