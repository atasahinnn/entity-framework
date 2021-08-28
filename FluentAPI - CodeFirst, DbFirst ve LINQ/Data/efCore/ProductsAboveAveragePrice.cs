using System;
using System.Collections.Generic;

#nullable disable

namespace EF_OneToMany.Data.efCore
{
    public partial class ProductsAboveAveragePrice
    {
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
