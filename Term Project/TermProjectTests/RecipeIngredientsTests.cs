using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using TermProject.Models;
using Microsoft.EntityFrameworkCore;

namespace TermProjectTests
{
    public class RecipeIngredientsTests
    {
        TermProjectContext dbContext;
        RecipeIngredient? i;
        List<RecipeIngredient> ingredients;
        [SetUp]
        public void Setup()
        {
            dbContext = new TermProjectContext();
        }

        [Test]
        public void GetAllOfRecipeTest()
        {
            ingredients = dbContext.RecipeIngredients.Where(i => i.RecipeId == 1).OrderBy(i => i.RecipeIngredientId).ToList();
            Assert.AreEqual(13, ingredients.Count());
            Assert.AreEqual(1084, ingredients[0].IngredientId);
        }

        [Test]
        public void GetNeededQuantityTest()
        {
            i = dbContext.RecipeIngredients.Find(1);
            Assert.AreEqual(0.0283495, i.Quantity);
        }
    }
}
