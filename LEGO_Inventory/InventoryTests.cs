using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LEGO_Inventory;

public class InventoryTests
{
    
    [TestClass]
    public class RebrickableApiTests
    {
        [TestMethod]
        public void GetSetPartsTest()
        {
            RebrickableApi api = new RebrickableApi();

            var response = api.GetSetParts("4502-1");
        
            Assert.IsNotNull(response.Result);
        }
    
        [TestMethod]
        public void GetSetInfoTest()
        {
            RebrickableApi api = new RebrickableApi();

            var response = api.GetSetInfo("4502-1");

            Assert.IsNotNull(response.Result);
        }
    
        [TestMethod]
        public void GetSetPartInfoTest()
        {
            RebrickableApi api = new RebrickableApi();

            var response = api.GetPartInfo("3001");
        
            Assert.IsNotNull(response.Result);
            
            Console.WriteLine(response.Result);
        }
    }

    [TestClass]
    public class ImportTests
    {
        [TestMethod]
        public void ImportSetInfoTest()
        {
            var ImportData = new ImportData();
            
            Assert.IsTrue(ImportData.ImportSetInfo("4502-1"));
        }
    }
}