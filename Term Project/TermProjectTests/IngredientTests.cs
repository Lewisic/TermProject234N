using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using TermProject.Models;
using Microsoft.EntityFrameworkCore;

namespace TermProjectTests
{
    public class IngredientTests
    {
        TermProjectContext dbContext;
        Ingredient? i;
        List<Ingredient> ingredients;

        [SetUp]
        public void Setup()
        {
            dbContext = new TermProjectContext();
        }

        [Test]
        public void GetAllTest()
        {
            ingredients = dbContext.Ingredients.OrderBy(i => i.IngredientId).ToList();
            Assert.AreEqual(1149, ingredients.Count);
            Assert.AreEqual("Lemon", ingredients[2].Name);
        }

        [Test]
        public void GetAllUnderReorderTest()
        {
            ingredients = dbContext.Ingredients.Where(i => i.OnHandQuantity <= i.ReorderPoint).OrderBy(i => i.IngredientId).ToList();
            Assert.AreEqual(1126, ingredients.Count);
            Assert.AreEqual(14, ingredients[11].IngredientId);
        }

        [Test]
        public void UpdateOnHandQuantityTest()
        {
            i = dbContext.Ingredients.Find(3);
            i.OnHandQuantity = 1;
            dbContext.Ingredients.Update(i);
            dbContext.SaveChanges();
            i = dbContext.Ingredients.Find(3);
            Assert.AreEqual(1, i.OnHandQuantity);
        }

        [Test]
        public void OrderByNameTest()
        {
            ingredients = dbContext.Ingredients.OrderBy(i => i.Name).ToList();
            Assert.AreEqual(1149, ingredients.Count);
            Assert.AreEqual("Abbey Ale", ingredients[1].Name);
        }

    }
}