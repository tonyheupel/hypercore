using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TonyHeupel.HyperCore;
using TonyHeupel.HyperJS;

namespace HyperJS.UnitTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class JSTests
    {
 
        [TestMethod]
        public void BooleanFunctionReturnsFalseProperly()
        {
            Assert.IsFalse(JS.Boolean(null));
            Assert.IsFalse(JS.Boolean(0));
            Assert.IsFalse(JS.Boolean(false));
            Assert.IsFalse(JS.Boolean(String.Empty));
            Assert.IsFalse(JS.Boolean("false"));
            Assert.IsFalse(JS.Boolean(JS.undefined));
            Assert.IsFalse(JS.Boolean(JS.NaN));
        }

        [TestMethod]
        public void BooleanFunctionReturnsTrueProperly()
        {
            Assert.IsTrue(JS.Boolean("False"));
            Assert.IsTrue(JS.Boolean(new object()));
            
            dynamic someThing = new HyperHypo(); //NOTE: May update base class for JS to return undefined instead of throw binding error
            Assert.IsTrue(JS.Boolean(someThing));
            someThing.foobar = "foobar";
            Assert.IsTrue(JS.Boolean(someThing.foobar));
            Assert.IsTrue(JS.Boolean(" "));
        }
    }
}
