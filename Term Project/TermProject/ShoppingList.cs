using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TermProject.UnitConversion;

using TermProject.Models;
using TermProject;

namespace TermProject.AdditionalModels
{
    public class ShoppingList
    {
        private List<ShoppingListItem> shoppingList = new List<ShoppingListItem>();  
        private DateTime startDate;
        private DateTime endDate;
        private List<Supplier> suppliers;
            
        public List<ShoppingListItem> Items
        {
            get
            {
                return shoppingList;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return this.endDate;
            }
        }

        public List<Supplier> Suppliers
        {
            get
            {
                return this.suppliers;
            }
        }

        public ShoppingList(TermProjectContext dbContext)
        {
            List<Ingredient> ingredients = dbContext.Ingredients
                .Include("IngredientType").Include("UnitType")
                .ToList();

            List<Ingredient> reOrderIngredients = ingredients
                .Where(i => i.OnHandQuantity <= i.ReorderPoint).ToList();

            foreach (Ingredient i in reOrderIngredients)
            {
                ShoppingListItem item = CreateShoppingListItem(dbContext, i);
                shoppingList.Add(item);
            }

            this.suppliers = dbContext.Suppliers.OrderBy(s => s.Name).ToList();
        }

        public ShoppingList(TermProjectContext dbContext, DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;

            List<Ingredient> ingredients = dbContext.Ingredients
                .Include("IngredientType").Include("UnitType").Include("IngredientInventoryAddition")
                .ToList();

            List<Ingredient> reOrderIngredients = ingredients
                .Where(i => i.OnHandQuantity < i.ReorderPoint).ToList();

            foreach (Ingredient i in reOrderIngredients)
            {
                ShoppingListItem item = CreateShoppingListItem(dbContext, i);
                shoppingList.Add(item);
            }

            // get a list of the batches that are scheduled during the time frame
            List<Batch> scheduledBatches = dbContext.Batches
                .Where(b => ((b.ScheduledStartDate.HasValue && !b.StartDate.HasValue) &&
                            (b.ScheduledStartDate >= startDate && b.ScheduledStartDate <= endDate))).ToList();

            foreach (Batch b in scheduledBatches)
            {
                double batchVolume = b.Volume;
                Recipe r = dbContext.Recipes.Find(b.RecipeId);
                double scalingFactor = batchVolume / r.Volume;
                var recipeIngredients = dbContext.RecipeIngredients.
                    Where(rI => rI.RecipeId == r.RecipeId).
                    GroupBy(rI => rI.IngredientId).
                    Select(rIGrouped => new
                    {
                        IngredientId = rIGrouped.Key,
                        ScaledQuantity = rIGrouped.Sum(i => i.Quantity * scalingFactor)
                    })
                    ;
                foreach (var rI in recipeIngredients)
                {
                    //Console.WriteLine(rI.IngredientId + " " + rI.ScaledQuantity);
                    ShoppingListItem item = shoppingList.Find(i => i.IngredientId == rI.IngredientId);
                    Ingredient ingredient;
                    if (item == null)
                    {
                        ingredient = ingredients
                            .Where(i => i.IngredientId == rI.IngredientId)
                            .SingleOrDefault();

                        item = CreateShoppingListItem(dbContext, ingredient);
                        shoppingList.Add(item);
                    }
                    item.ScheduledBatchQuantity += rI.ScaledQuantity;
                }
            }

            // get a list of what's been ordered and is scheduled to be delivered during the time frame
            var orderedIngredients = dbContext.IngredientInventoryAdditions.
            Where(oI => !oI.TransactionDate.HasValue &&
                        (oI.EstimatedDeliveryDate >= startDate && oI.EstimatedDeliveryDate <= endDate)).
            GroupBy(oI => oI.IngredientId).
            Select(oIGrouped => new
            {
                IngredientId = oIGrouped.Key,
                OrderedQuantity = oIGrouped.Sum(i => i.Quantity)
            });


            // Add what's on order to any of the ingredients in the list
            // ?????? I wonder if I should remove ingredients from the list?
            foreach (var oI in orderedIngredients)
            {
                ShoppingListItem item = shoppingList.Find(i => i.IngredientId == oI.IngredientId);
                if (item != null)
                    item.OnOrderQuantity += oI.OrderedQuantity;
            }
            
            // couldn't get anonymous type to serialize
            this.suppliers = dbContext.Suppliers.OrderBy(s => s.Name).ToList();
                
        }


        private ShoppingListItem CreateShoppingListItem(TermProjectContext dbContext, Ingredient i)
        {
            ShoppingListItem item = new ShoppingListItem();
            item.IngredientId = i.IngredientId;
            item.Name = i.Name;
            item.Version = (int)i.Version;
            item.OnHandQuantity = i.OnHandQuantity;
            item.ReorderPoint = i.ReorderPoint;
            item.IngredientType = i.IngredientType.Name;
            item.ScheduledBatchQuantity = 0;
            item.OnOrderQuantity = 0;
            string unit = i.UnitType.Name;
            string defaultUnit = dbContext.AppConfigs.Find(1).DefaultUnits;
            switch (unit.ToLower())
            {
                case "weight":
                    item.UnitChoices = WeightConvert.Units;
                    item.Units = WeightConvert.GetUnit(defaultUnit);
                    break;
                case "fluid":
                    item.UnitChoices = FluidConvert.Units;
                    item.Units = FluidConvert.GetUnit(defaultUnit);
                    break;
                case "each":
                    item.UnitChoices = new List<string> { "Each" };
                    item.Units = "Each";
                    break;
            }
            IngredientInventoryAddition inventory = i.IngredientInventoryAdditions
                .Where(inv => inv.QuantityRemaining > 0)
                .OrderBy(inv => inv.TransactionDate).FirstOrDefault();
            if (inventory != null)
            {
                item.SupplierId = inventory.SupplierId;
                item.SupplierName = dbContext.Suppliers.Find(item.SupplierId).Name;
                item.CostPerUnit = inventory.UnitCost;
            }
            return item;
        }
    }
}
