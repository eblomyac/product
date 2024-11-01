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
        
        public int DeltaProcent { get; set; }
        public string ProductType { get; set; }
        public string FullName { get; set; }
        
        public bool IsCorrect { get; set; }
        
    }
    public List<CheckResult> Check()
    {
        using (MaconomyBase mb = new MaconomyBase())
        {
            var allArtsName = mb.getTableFromDB("Select * from (SELECT BOMPART.ITEMNUMBER, BOMPART.BomItemPartNumber, " +
                                                "BOMPART.NumberOf,ITEMPOPUP3, SUPPLEMENTARYTEXT4, PopUpItem.Name FROM BOMPART left join ITEMINFORMATION on" +
                                                " ITEMINFORMATION.ITEMNUMBER=BOMPART.ITEMNUMBER left join PopUpItem on ItemInformation.ITEMPOPUP3=PopUpItem.PopupItemNumber and " +
                                                "POPUPTYPENAME='ItemPopupType3' Where BomItemPartNumber='LUMAR' " +
                                                "or BomItemPartNumber='SVETON') " +
                                                "dat where dat.ITEMPOPUP3 in ('9','5','12','15','16','23','28','29','27','20') ");
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
                cr.FullName = MaconomyBase.makeStringRu(art.Field<string>("SUPPLEMENTARYTEXT4"));
                cr.ProductType = MaconomyBase.makeStringRu(art.Field<string>("Name"));
              
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
                if (cr.MaconomyValue == 0.0m)
                {
                    cr.DeltaProcent = 100;
                }
                else
                {
                    cr.DeltaProcent = (int) Math.Round(Math.Abs(1.0m-cr.CrpValue / cr.MaconomyValue)*100);    
                }

                cr.IsCorrect = cr.DeltaProcent <= 2;
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
        t.Columns.Add("Наименование");
        t.Columns.Add("Тип продвижения");
        t.Columns.Add("Норматив Макономи", typeof(decimal));
        t.Columns.Add("Норматив Crp",typeof(decimal));
        t.Columns.Add("Отклонение %", typeof(decimal));
        t.Columns.Add("Абсолютное отклонение", typeof(decimal));
        t.Columns.Add("Коректность", typeof(string));
        
        DataTable tNot = new DataTable("Report not correct");
        tNot.Columns.Add("Артикул");
        tNot.Columns.Add("Наименование");
        tNot.Columns.Add("Тип продвижения");
        tNot.Columns.Add("Норматив Макономи", typeof(decimal));
        tNot.Columns.Add("Норматив Crp",typeof(decimal));
        tNot.Columns.Add("Отклонение %", typeof(decimal));
        tNot.Columns.Add("Абсолютное отклонение", typeof(decimal));
       

        foreach (var cr in toReport)
        {
            DataRow r = t.NewRow();
            r[0] = cr.Article;
            r[1] = cr.FullName;
            r[2] = cr.ProductType;
            r[3] = cr.MaconomyValue;
            r[4] = cr.CrpValue;
            r[5] = cr.DeltaProcent;
            r[6] = cr.Delta;
            
            r[7] = cr.IsCorrect;

            t.Rows.Add(r);

            if (cr.IsCorrect == false)
            {
                DataRow rNot = tNot.NewRow();
                
                rNot[0] = cr.Article;
                rNot[1] = cr.FullName;
                rNot[2] = cr.ProductType;
                rNot[3] = cr.MaconomyValue;
                rNot[4] = cr.CrpValue;
                
                rNot[5] = cr.DeltaProcent;
                rNot[6] = cr.Delta;

                tNot.Rows.Add(rNot);
            }
        }

        DataSet ds = new DataSet();
        ds.Tables.Add(tNot);
        ds.Tables.Add(t);
        return ds;
    }
}