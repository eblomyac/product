﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using ProtoLib.Managers;
using ProtoLib.Model.DailyReport;

namespace ProtoLib.Model
{
    
    public class BaseContext:DbContext
    {
        public delegate void postChanged();

        private bool isStatusChanged = false;

        public static event postChanged PostWorkStatusChanged;
       
        private User? ContextOwner;

        public string accName
        {
            get
            {
                if (this.ContextOwner != null)
                {
                    return this.ContextOwner.AccName;
                }
                else
                {
                    return "";
                }
            }
        }

        public DbSet<Work> Works { get; set; }
         public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
         public DbSet<PostCreationKey> PostKeys { get; set; }
        public DbSet<WorkStatusLog> WorkStatusLogs { get; set; }
        public DbSet<WorkIssue> Issues { get; set; }
        public DbSet<WorkIssueTemplate> IssueTemplates { get; set; }
        public DbSet<PostStatistic> PostStatistics { get; set; }
        public DbSet<WorkIssueLog> WorkIssueLogs { get; set; }
        
        public DbSet<StoredImage> Images { get; set; }
        

        public DbSet<TechCard> TechCards { get; set; }
        public DbSet<WorkPriority> WorkPriorities { get; set; }
        public DbSet<TechCardPost> TechCardPosts { get; set; }
        public DbSet<TechCardLine> TechCardLines { get; set; }
        
        public DbSet<OTKAvailableOperation> OTKAvailableOperations { get; set; }
        public DbSet<OTKCheck> OTKChecks { get; set; }
        public DbSet<OTKCheckLine> OTKCheckLines { get; set; }
        public DbSet<OTKWorker> OTKWorkers { get; set; }
        public DbSet<OTKTargetValue> OTKTargetValues { get; set; }
        
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferLine> TransferLines { get; set; }
        public DbSet<DailySource> DailySources { get; set; }
        public DbSet<ProductionLine> ProductionLines { get; set; }
        public DbSet<MaconomyMovementTransaction> MaconomyMovementTransactions { get; set; }
        
        public DbSet<AdditionalCostTemplate> AdditionalCostTemplates { get; set; }
        public DbSet<AdditionalCost> AdditionalCosts { get; set; }
        public DbSet<OperatorCountChangeRecord> OperatorCountChangeRecords { get; set; }
        public DbSet<CostReportRecord> CostReportRecords { get; set; }
        
        public DbSet<ArticleOrderDaily> ReportArticleOrderDailies { get; set; }
        public DbSet<LineDaily> ReportLineDailies { get; set; }
        public DbSet<PostDaily> ReportPostDailies { get; set; }

        
       
        public DbSet<ProductWorker> ProductWorkers { get; set; }
        public DbSet<ProductTarget> ProductTargets { get; set; }
        public DbSet<ProductCalendarRecord> ProductCalendarRecords { get; set; }
        
        public DbSet<ProductionPlan>    ProductionPlans { get; set; }
     
        public BaseContext()
        {
            
        }


        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {
                //string connectionString = Constants.Database.ConnectionString;
                //optionsBuilder.UseSqlServer(connectionString);
              
               optionsBuilder.UseSqlServer("Server=kdb1.kck2.ksk.ru;Database=protoproduct-dev;User=sa;Password=-c2h5oh-");
              //  #else
              optionsBuilder.UseSqlServer("Server=kdb1.kck2.ksk.ru;Database=protoproduct;User=sa;Password=-c2h5oh-");
               #if DEBUG
               //      optionsBuilder.UseSqlServer("Server=kdb1.kck2.ksk.ru;Database=protoproduct-dev;User=sa;Password=-c2h5oh-");
                #endif
                
               
            }
        }
        private User SystemUser()
        {var exist = this.Users.FirstOrDefault(x => x.AccName == "system");
            if (exist == null)
            {
                exist = new User();
                
                exist.AccName = "system";
                exist.Name = "Система";
                exist.Mail = "product_post@ksk.ru";
                this.Users.Add(exist);
                this.SaveChanges();
            }

            return exist;
        }
        private void WorkStatusLog()
        {
            var changedWorks = this.ChangeTracker.Entries()
                .Where(x => x.Entity.GetType() == typeof(Work)).ToList();
               
            List<WorkStatusLog> logs = new List<WorkStatusLog>();
            foreach (var change in changedWorks)
            {
                if (change.Entity is Work w)
                {
                    var statusChange = change.Properties.FirstOrDefault(x => x.Metadata.Name == "Status");
               
                    
                    var log = new WorkStatusLog();
                    log.Stamp = DateTime.Now;
                    log.WorkId = w.Id;
                    log.Article = w.Article;
                    log.PostId = w.PostId;
                    log.OrderNumber = w.OrderNumber;
                    log.EditedBy = ContextOwner.AccName;
                    log.SingleCost = w.SingleCost;
                    log.Count = w.Count;
                    log.ProductionLineId = w.ProductLineId;
                    log.OrderLineNumber = w.OrderLineNumber;
                    log.MovedFrom = w.MovedFrom;
                    log.MovedTo = w.MovedTo;
                    if (change.State == EntityState.Modified && statusChange != null)
                    {
                        log.NewStatus = (WorkStatus)statusChange.CurrentValue;
                        log.PrevStatus = (WorkStatus) statusChange.OriginalValue;
                    }else if (change.State == EntityState.Added && statusChange!=null)
                    {
                      
                        log.NewStatus = (WorkStatus)statusChange.CurrentValue;
                        log.PrevStatus = WorkStatus.unkown;
                    }else if (change.State == EntityState.Deleted && statusChange!=null)
                    {
                       
                        log.NewStatus = (WorkStatus.unkown);
                        log.PrevStatus =(WorkStatus) statusChange.OriginalValue;

                    }
                    else
                    {
                        log.NewStatus = w.Status;
                        log.PrevStatus = w.Status;
                    }

                 

                    if ( log.PrevStatus != log.NewStatus)
                    {
                        logs.Add(log);
                        isStatusChanged = true;
                        //StartPostStatUpdate();

                    }
                    else
                    {
                        Debug.WriteLine("");
                    }
                    
                }
            }
            this.WorkStatusLogs.AddRange(logs);
            

        }

        

        private void StartPostStatUpdate()
        {
            BaseContext.PostWorkStatusChanged?.Invoke();
        }

        public BaseContext(string accName, ServiceProvider provider=null)
        {
            
            if (String.IsNullOrEmpty(accName))
            {
                this.ContextOwner = SystemUser();
            }
            else
            {
                this.ContextOwner = this.Users.FirstOrDefault(x => x.AccName == accName);    
            }
            this.SavingChanges+= OnSavingChanges;
            this.SavedChanges+= OnSavedChanges;
        }

        private void OnSavedChanges(object? sender, SavedChangesEventArgs e)
        {
            if (isStatusChanged)
            {
                this.StartPostStatUpdate();
            }
        }

        private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
        {
            WorkStatusLog();
        }
        

    }
}