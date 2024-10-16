
using System.Data;
using KSK_LIB.Maconomy;

namespace ProtoLib.Managers;

public class MaconomyOrderMaxCountManager
{
    private DataTable _data = null;
    private Task<DataTable> _loadTask = null;
    private List<string> _orders = null;
    
    public  MaconomyOrderMaxCountManager(string order)
    {
        this._orders = new List<string>() { order };
        this._loadTask = this.LoadData(this._orders);
        if (this._loadTask.Status == TaskStatus.Created)
        {
            this._loadTask.Start();
        }
    }
    public  MaconomyOrderMaxCountManager(List<string> orders)
    {
        this._orders = orders;
        this._loadTask = this.LoadData(this._orders);
        if (this._loadTask.Status == TaskStatus.Created)
        {
            this._loadTask.Start();
        }
        
    }

    public async Task<List<int>> GetLinesNumber(long orderNumber, string article)
    {
        if (this._loadTask.Status == TaskStatus.Faulted)
        {
            this._loadTask = this.LoadData(_orders);
        }
        if (this._loadTask.Status != TaskStatus.RanToCompletion)
        {
            await Task.Delay(15);
            return await GetLinesNumber(orderNumber, article);
        }
        else
        {
            if (this._data == null)
            {
                this._data = this._loadTask.Result;
            }
            var rows =  this._data.Select($"TransactionNumber={orderNumber} and ITEMNUMBER='{article}'");
            List<int> result = new List<int>();
            foreach (DataRow row in rows)
            {
                result.Add((int)row.Field<decimal>("LINENUMBER"));
            }

            return result;
        }
    }

    public async Task <decimal> GetCount(long orderNumber, int line)
    {
        if (this._loadTask.Status == TaskStatus.Faulted)
        {
            this._loadTask = this.LoadData(_orders);
        }
        if (this._loadTask.Status != TaskStatus.RanToCompletion)
        {
            await Task.Delay(15);
            return await GetCount(orderNumber, line);
        }
        else
        {
            if (this._data == null)
            {
                this._data = this._loadTask.Result;
            }
            var rows =  this._data.Select($"TransactionNumber={orderNumber} and LINENUMBER={line}");
            if (rows.Length == 0)
            {
                return 0;
            }
            else
            {
                return rows[0].Field<decimal>("NUMBEROF");
            }
        }
        
    }
  

    private Task<DataTable> LoadData(List<string> orders)
    {
        return new Task<DataTable>(() =>
        {
            using (MaconomyBase mb = new MaconomyBase())
            {
                string order = String.Join(',', orders.Select(x => $"'{x}'"));
                return mb.getTableFromDB(
                        $"SELECT ProductionLine.TransactionNumber,LINENUMBER, ITEMNUMBER, NUMBEROF as numberof from ProductionLine left join ProductionVoucher on ProductionLine.TransactionNumber = ProductionVoucher.TransactionNumber where ProductionLine.TransactionNumber in ({order})");
                
            }
        });
    }

}