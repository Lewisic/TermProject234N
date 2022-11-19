using TermProject.Models;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace TermProject.AdditionalModels
{
    public class ShoppingListItem
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public string IngredientType { get; set; }
        public double ReorderPoint { get; set; }
        public double OnHandQuantity { get; set; }
        public double ScheduledBatchQuantity { get; set; }
        public double OnOrderQuantity { get; set; }
        public string Units { get; set; }
        public List<string> UnitChoices { get; set; }
        public decimal CostPerUnit { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        public override string ToString()
        {
            return IngredientId + ", " + Name + ", " + Version + ", " + IngredientType + ", " +
                ReorderPoint + ", " + OnHandQuantity + ", " + ScheduledBatchQuantity + ", " +
                OnOrderQuantity + ", " + Units + ", " + CostPerUnit + ", " + SupplierId + ", " + SupplierName;
        }
    }
}
