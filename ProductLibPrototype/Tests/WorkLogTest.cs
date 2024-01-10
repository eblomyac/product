using System.Collections.Generic;
using NUnit.Framework;
using ProductLibPrototype.Managers;

namespace ProductLibPrototype.Tests
{
  
    public class WorkLogTest
    {
    
        public void TestWorkLogStatusChange()
        {
            WorkCreateManager wcm = new WorkCreateManager();

            WorkCreateTemplate wct = new WorkCreateTemplate();
            wct.Article = "TEST";
            wct.OrderNumber = 101;
            wct.Count = 1;
            wct.SingleCost = 50;
            wct.PostKey = "TST";

            var works = wcm.CreateWorks(new List<WorkCreateTemplate>() {wct});
            
            
            
            
        }
    }
}