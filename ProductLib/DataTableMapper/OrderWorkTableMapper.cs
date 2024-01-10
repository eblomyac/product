using System.Collections.Generic;
using System.Data;
using ProductLib.Work;

namespace ProductLib.DataTableMapper
{
    public class OrderWorkTableMapper:IDataTableMapper<OrderWork>
    {
        public List<OrderWork> Import(DataTable table)
        {
            return DataTableMapper.TableMapper<OrderWork>.ReadThroughJson(table);
        }

        public DataTable ExportTemplate()
        {
            return DataTableMapper.TableMapper<OrderWork>.TemplateExport();
        }
    }
}