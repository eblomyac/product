using System.Data;
using KSK_LIB.Maconomy;

namespace ProtoLib.Managers;

public class PlanChecker
{
    public class CheckResult
    {
        public string Article { get; set; }
        public string WorkType { get; set; }
        public decimal CrpValue { get; set; }
        public decimal MaconomyValue { get; set; }
        public decimal Delta { get; set; }
        
        public bool IsCorrect { get; set; }
        
    }
    public List<CheckResult> Check()
    {
        using (MaconomyBase mb = new MaconomyBase())
        {
            var allArtsName = mb.getTableFromDB("Select * from (SELECT BOMPART.ITEMNUMBER, BOMPART.BomItemPartNumber, BOMPART.NumberOf,ITEMPOPUP3 FROM BOMPART left join ITEMINFORMATION on    ITEMINFORMATION.ITEMNUMBER=BOMPART.ITEMNUMBER Where BomItemPartNumber='LUMAR' or BomItemPartNumber='SVETON') dat where dat.ITEMPOPUP3 in ('9','5','12','15','16','23','28','29','27','20') ");
            List<CheckResult> result = new List<CheckResult>();

            CrpManager crpManager = new CrpManager();
            var crpArticles = crpManager.CrpArticles();

            var crpData = crpManager.LoadAticleDatas(crpArticles);
            foreach (DataRow art in allArtsName.Rows)
            {
                CheckResult cr = new CheckResult();
                cr.Article =  MaconomyBase.makeStringRu(art.Field<string>("ITEMNUMBER"));
                cr.WorkType = art.Field<string>("BomItemPartNumber");
                cr.MaconomyValue = art.Field<decimal>("NumberOf");
                
                var crpArticle = crpData.Where(x => x.Article == cr.Article).ToList();
                if (crpArticle.Count==0)
                {
                    cr.CrpValue = 0;
                }
                else
                {
                  cr.CrpValue = crpArticle.Sum(x => x.SingleCost);
                }

                cr.Delta = Math.Abs(cr.MaconomyValue - cr.CrpValue);
                cr.IsCorrect = cr.Delta < cr.MaconomyValue * 0.02m;
                result.Add(cr);
            }

            return result;
        }
    }

    public DataSet CheckReport(List<CheckResult> reportData, string filter = "")
    {
        List<CheckResult> toReport = null;
        if (string.IsNullOrEmpty(filter))
        {
            toReport = reportData;
        }
        else
        {
            toReport = reportData.Where(x => x.WorkType == filter).ToList();
        }

        DataTable t = new DataTable("Report");
        t.Columns.Add("Артикул");
        t.Columns.Add("Норматив Макономи", typeof(decimal));
        t.Columns.Add("Норматив Crp",typeof(decimal));
        t.Columns.Add("Дельта", typeof(decimal));
        t.Columns.Add("Коректность", typeof(string));
        
        DataTable tNot = new DataTable("Report not correct");
        tNot.Columns.Add("Артикул");
        tNot.Columns.Add("Норматив Макономи", typeof(decimal));
        tNot.Columns.Add("Норматив Crp",typeof(decimal));
        tNot.Columns.Add("Дельта", typeof(decimal));

        foreach (var cr in toReport)
        {
            DataRow r = t.NewRow();
            r[0] = cr.Article;
            r[1] = cr.MaconomyValue;
            r[2] = cr.CrpValue;
            r[3] = cr.Delta;
            r[4] = cr.IsCorrect;

            t.Rows.Add(r);

            if (cr.IsCorrect == false)
            {
                DataRow rNot = tNot.NewRow();
                
                rNot[0] = cr.Article;
                rNot[1] = cr.MaconomyValue;
                rNot[2] = cr.CrpValue;
                rNot[3] = cr.Delta;

                tNot.Rows.Add(rNot);
            }
        }

        DataSet ds = new DataSet();
        ds.Tables.Add(tNot);
        ds.Tables.Add(t);
        return ds;
    }
}