using System.Data;
using KSK_LIB.Maconomy;
using ProtoLib.Model;

namespace ProtoLib.Managers;

public class SuggestOperatorWork
{
    public  List<long> LoadSuggestions()
    {
        List<long> result = new List<long>();
        using (MaconomyBase mb = new MaconomyBase())
        {
            using (BaseContext c = new BaseContext())
            {
                DateTime d = DateTime.Today.AddMonths(-3);
                var macOrders = mb.getTableFromDB(
                    $"SELECT PRODUCTIONLINE.TRANSACTIONNUMBER, PRODUCTIONLINE.LINENUMBER from PRODUCTIONLINE left join PRODUCTIONVOUCHER on PRODUCTIONLINE.TRANSACTIONNUMBER=PRODUCTIONVOUCHER.TRANSACTIONNUMBER where PRODUCTIONVOUCHER.closed=0 and PRODUCTIONVOUCHER.CREATEDDATE>'{d:yyyy.MM.dd}'");
                var orders = macOrders.DefaultView.ToTable(true, "TRANSACTIONNUMBER");
                List<long> orderNumbers = new List<long>();
                foreach (DataRow r in orders.Rows)
                {
                    orderNumbers.Add((long)r.Field<decimal>("TRANSACTIONNUMBER"));
                }

                var existWorks = c.Works.Where(x => orderNumbers.Contains(x.OrderNumber)).ToList();

                foreach (var orderNumber in orderNumbers)
                {
                    if (macOrders.Select($"TRANSACTIONNUMBER={orderNumber}").Count() != existWorks
                            .Where(x => x.OrderNumber == orderNumber).Select(x => x.OrderLineNumber).Distinct()
                            .Count())
                    {
                        result.Add(orderNumber);
                    }
                }

            }

            return result;
        }

    }
}