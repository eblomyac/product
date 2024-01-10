using System;

namespace ProductLib.Work
{
    public  class OrderWork:Work, IEquatable<OrderWork> {
        public override string FullName
        {
            get
            {
                return string.Join(' ', Prefix, OrderNumber);
            }
        }
        public long OrderNumber { get; set; }
        public string ProductionLine { get;  set; }

        public OrderWork(long orderNumber, string productionLine):base()
        {
            this.OrderNumber = orderNumber;
            this.ProductionLine = productionLine;
            this.Prefix = Constants.Work.OrderWorkPrefix;
        }

        public OrderWork():base()
        {
            this.OrderNumber = 0;
            this.ProductionLine = "";
            this.Prefix = Constants.Work.OrderWorkPrefix;
        }

        public bool Equals(OrderWork? other)
        {
            if (other == null)
            {
                return false;}
            return this.OrderNumber == other.OrderNumber && this.ProductionLine == other.ProductionLine;
        }
    }
}