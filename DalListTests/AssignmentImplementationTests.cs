using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Tests
{
    [TestClass()]
    public class AssignmentImplementationTests
    {
        [TestMethod()]
        public void CreateTest()
        {
            Assert.AreNotEqual(3,3);
        }
    }
}