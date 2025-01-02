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

                if (set.DateModified >= DateTime.Parse(setInfo!["last_modified_dt"]!.ToString()))
                {
                    set.Name = setInfo!["name"]!.ToString();
                    set.SetImg = setInfo!["set_img_url"]!.ToString();
                    set.SetURL = setInfo!["set_url"]!.ToString();
                    set.DateModified = DateTime.Parse(setInfo!["last_modified_dt"]!.ToString());
                    set.NumParts = int.Parse(setInfo!["num_parts"]!.ToString());
                    set.ReleaseYear = int.Parse(setInfo!["year"]!.ToString());
                }
            }
            else
            {
                var set = new Set
                {
                    SetId = setInfo!["set_num"]!.ToString(),
                    Name = setInfo!["name"]!.ToString(),
                    SetURL = setInfo!["set_url"]!.ToString(),
                    SetImg = setInfo!["set_img_url"]!.ToString(),
                    DateModified = DateTime.Parse(setInfo!["last_modified_dt"]!.ToString()),
                    NumParts = int.Parse(setInfo!["num_parts"]!.ToString()),
                    ReleaseYear = int.Parse(setInfo!["year"]!.ToString()),
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

    public bool ImportSetParts(string setId)
    {
        RebrickableApi api = new RebrickableApi();

        JsonObject? setParts = api.GetSetParts(setId).Result;
        int saveCount = 0;

        using (var context = new InventoryContext())
        {
            var setContext = context.Set<Set>();

            if (setContext.Any(s => s.SetId == setId))
            {
                var set = setContext.First(s => s.SetId == setId);

                var brickContext = context.Set<Brick>();
                var setBrickContext = context.Set<SetBrick>();

                foreach (var part in setParts!["results"]!.AsArray())
                {
                    Brick brick;

                    if (!brickContext.Any(b => b.PartNum == part!["part"]!["part_num"]!.ToString()
                                               && b.ColorId == part!["color"]!["id"]!.ToString()))
                    {
                        var partNum = part!["part"]!["part_num"]!.ToString();
                        var name = part!["part"]!["name"]!.ToString();
                        var partUrl = part!["part"]!["part_url"]!.ToString();
                        var partImg = part!["part"]!["part_img_url"]!.ToString();
                        var colorId = part!["color"]!["id"]!.ToString();
                        var colorName = part!["color"]!["name"]!.ToString();
                        var rgb = part!["color"]!["rgb"]!.ToString();
                        var isTrans = part!["color"]!["is_trans"]!.ToString().Equals("true");
                        var count = 0;

                        brick = new Brick
                        {
                            PartNum = partNum,
                            Name = name,
                            PartURL = partUrl,
                            PartImg = partImg,
                            ColorId = colorId,
                            ColorName = colorName,
                            RGB = rgb,
                            IsTrans = isTrans,
                            Count = count
                        };

                        brickContext.Add(brick);
                    }
                    else
                    {
                        brick = brickContext.First(b => b.PartNum == part!["part"]!["part_num"]!.ToString()
                                                        && b.ColorId == part["color"]!["id"]!.ToString());
                    }


                    var PartNum = brick!.PartNum;
                    var ColorId = brick!.ColorId;
                    var SetId = set.SetId;
                    var Count = 0;
                    var SpareCount = 0;

                    var isSpare = part!["is_spare"]!.ToString().Equals("true");

                    if (isSpare)
                    {
                        SpareCount = int.Parse(part!["quantity"].ToString());
                    }
                    else
                    {
                        Count = int.Parse(part!["quantity"].ToString());
                    }

                    if (!setBrickContext.Any(sb => sb.PartNum == PartNum
                                                   && sb.ColorId == ColorId
                                                   && sb.SetId == SetId))
                    {
                        SetBrick setBrick = new SetBrick
                        {
                            PartNum = PartNum,
                            ColorId = ColorId,
                            SetId = SetId,
                            Count = Count,
                            SpareCount = SpareCount,
                        };
                        
                        setBrickContext.Add(setBrick);
                    }
                    else
                    {
                        SetBrick setBrick = setBrickContext.First(sb => sb.PartNum == PartNum
                                                               && sb.ColorId == ColorId
                                                               && sb.SetId == SetId);
                        
                        if(isSpare)
                            setBrick.SpareCount = SpareCount;
                        else
                            setBrick.Count = Count;
                    }


                    saveCount += context.SaveChanges();
                }
            }
            else
            {
                throw new Exception($"No set found with ID {setId} in database");
            }

            return saveCount > 0;
        }

        return false;
    }

    // public bool ImportBrick(Brick brick, string colorId)
    // {
    //     RebrickableApi api = new RebrickableApi();
    //     
    //     
    //     JsonObject? bricks = api.GetPartInfo(brick.PartNum).Result;
    //
    //     using (var context = new InventoryContext())
    //     {
    //         var brickContext = context.Set<Brick>();
    //
    //
    //         if (!brickContext.Any(b => b.PartNum == brick.PartNum
    //                                    && b.ColorId == colorId)
    //         {
    //             brick = new Brick
    //             {
    //                 PartNum = part!["id"]!.ToString(),
    //                 Name = part!["name"]!.ToString(),
    //                 PartURL = part!["part_url"]!.ToString(),
    //                 PartImg = part!["part_img_url"]!.ToString(),
    //                 ColorId = part!["color"]!["id"]!.ToString(),
    //                 ColorName = part!["color"]!["name"]!.ToString(),
    //                 RGB = part!["color"]!["RGB"]!.ToString(),
    //                 IsTrans = part!["color"]!["is_trans"]!.ToString().Equals("true"),
    //                 Count = 0
    //             };
    //
    //             brickContext.Add(brick);
    //         }
    //         else
    //         {
    //             brick = brickContext.First(b => b.PartNum == part!["id"]!.ToString()
    //                                             && b.ColorId == part["color"]!["id"]!.ToString());
    //         }
    //     }
    //
    //     return false;
    // }
}