using System.Data;
using NUnit.Framework;

namespace ProductLib.Tests
{
    [TestFixture]
    public class ArticleWorkMapper
    {
        [Test]
        public void TestExportImport()
        {
            string value = "2";
            DataTableMapper.ArticleWorkMapper awm = new DataTableMapper.ArticleWorkMapper();
            var table = awm.ExportTemplate();
            DataRow row = table.NewRow();
            foreach (DataColumn column in table.Columns)
            {
                row[column.ColumnName] = value;
            }

            table.Rows.Add(row);
            var result = awm.Import(table);

            Assert.AreEqual(1,result.Count);
            Assert.AreEqual( value, result[0].Article);
            Assert.AreEqual( int.Parse(value), result[0].Count);
        }
    }
}