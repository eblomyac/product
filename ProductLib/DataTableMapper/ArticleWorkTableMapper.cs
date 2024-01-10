using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ProductLib.Work;

namespace ProductLib.DataTableMapper
{
    public class ArticleWorkMapper: IDataTableMapper<ArticleWork>

    {
        public List<ArticleWork> Import(DataTable table)
        {
            return TableMapper<ArticleWork>.ReadThroughJson(table);
        }

        public DataTable ExportTemplate()
        {
            return TableMapper<ArticleWork>.TemplateExport();
        }
    }
}