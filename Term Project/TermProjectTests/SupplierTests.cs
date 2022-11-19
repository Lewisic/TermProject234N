using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using TermProject.Models;
using Microsoft.EntityFrameworkCore;

namespace TermProjectTests
{
    public class SupplierTests
    {
        TermProjectContext dbContext;
        Supplier s;
        List<Supplier> suppliers;

        [SetUp]
        public void Setup()
        {
            dbContext = new TermProjectContext();
        }

        [Test]
        public void GetAllSuppliersTest()
        {
            suppliers = dbContext.Suppliers.OrderBy(s => s.SupplierId).ToList();
            Assert.AreEqual(6, suppliers.Count);
            Assert.AreEqual("Malteurop Malting Company", suppliers[1].Name);
        }

        [Test]
        public void GetSupplierAddressTest()
        {
            s = dbContext.Suppliers.Where(s => s.Name == "Malteurop Malting Company").SingleOrDefault();
            Assert.AreEqual(2, s.SupplierId);
            List<SupplierAddress> a = dbContext.SupplierAddresses.Include("Address").Where(a => a.SupplierId == s.SupplierId).ToList();
            Console.Write(a[0].Address.StreetLine1);
            Assert.AreEqual("3830 W. Grant Street", a[0].Address.StreetLine1);
        }

        [Test]
        public void GetSupplierAddressTypeTest()
        {
            s = dbContext.Suppliers.Where(s => s.Name == "Malteurop Malting Company").SingleOrDefault();
            Assert.AreEqual(2, s.SupplierId);
            List<SupplierAddress> a = dbContext.SupplierAddresses.Include("AddressType").Where(a => a.SupplierId == s.SupplierId).ToList();
            Console.Write(a[0].AddressType.Name);
            Assert.AreEqual("billing", a[0].AddressType.Name);
            Assert.AreEqual("mailing", a[1].AddressType.Name);
        }
    }
}
