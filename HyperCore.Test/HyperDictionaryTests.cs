using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
// using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TonyHeupel.HyperCore.UnitTest
{
    /// <summary>
    /// Tests for the HyperDictionary class
    /// </summary>
    [TestFixture]
    public class HyperDictionaryTests
    {
        #region Core Inheritance Tests
        [Test]
        public void DefaultConstructorLeavesIdNull()
        {
            Assert.IsNull(new HyperDictionary().Id, "Default constructor should leave the Id property null");
        }

        [Test]
        public void SingleArgumentConstructorSetsId()
        {
            Assert.AreEqual("Foo", new HyperDictionary("Foo").Id, "Constructor with one argument should set the Id property");
        }

        [Test]
        public void OwnedPropertiesWork()
        {
            var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary();
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            Assert.AreEqual("foo value", one["foo"]);
            Assert.AreEqual("bar value", one["bar"]);
            Assert.AreSame(baz, one["baz"]);

            Assert.IsTrue(one.HasOwnProperty("foo"));
            Assert.IsTrue(one.HasOwnProperty("bar"));
            Assert.IsTrue(one.HasOwnProperty("baz"));
        }

        [Test]
        public void InheritedPropertiesWork()
        {
            var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            one.AddChild(two);
            two["whatever"] = "big W";

            Assert.AreEqual("foo value", two["foo"]);
            Assert.AreEqual("bar value", two["bar"]);
            Assert.AreEqual("big W", two["whatever"]);

            Assert.IsFalse(two.HasOwnProperty("foo"));
            Assert.IsFalse(two.HasOwnProperty("bar"));
            Assert.IsFalse(two.HasOwnProperty("baz"));
            Assert.IsTrue(two.HasOwnProperty("whatever"));

            try
            {
                var whatever = one["whatever"];
                Assert.Fail("Not supposed to reach here since this object should not have this property");
            }
            catch (IndexOutOfRangeException)
            {
                //Pass!
            }
        }

        [Test]
        public void OverriddenPropertiesWork()
        {
            var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            one.AddChild(two);
            two["bar"] = "two's bar value";

            //Other verification (sanity)
            Assert.AreEqual("foo value", two["foo"]);
            Assert.AreSame(baz, two["baz"]);

            Assert.IsFalse(two.HasOwnProperty("foo"));
            Assert.IsFalse(two.HasOwnProperty("baz"));

            //Interesting assertions for this test
            Assert.AreEqual("two's bar value", two["bar"]);
            Assert.AreEqual("bar value", one["bar"]);
            Assert.IsTrue(two.HasOwnProperty("bar"));
        }

        [Test]
        public void RemovedPropertiesWork()
        {
            var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            one.AddChild(two);
            two.RemoveProperty("bar");

            //Other verification (sanity)
            Assert.AreEqual("foo value", two["foo"]);
            Assert.AreSame(baz, two["baz"]);

            Assert.IsFalse(two.HasOwnProperty("foo"));
            Assert.IsFalse(two.HasOwnProperty("baz"));

            //Interesting assertions for this test
            Assert.AreEqual("bar value", one["bar"]);
            Assert.IsTrue(two.HasOwnProperty("bar"));

            try
            {
                var bar = two["bar"];
                Assert.Fail("Should not get this far since this object had this property removed");
            }
            catch (IndexOutOfRangeException)
            {
                //Pass!
            }
            catch (Exception)
            {
                Assert.Fail("Raised an exception that was not IndexOutOfRangeException");
            }

        }

        [Test]
        public void ExtendedPropertiesWork()
        {
            var baz = new string[] { "hello", "there" };
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            one.AddChild(two);
            two.ExtendProperty("baz", new object[] { 2, 3, 4 }); //NOTE: Integers, not strings!

            //Other verification (sanity)
            Assert.AreEqual("foo value", two["foo"]);
            Assert.AreEqual("bar value", two["bar"]);

            Assert.IsFalse(two.HasOwnProperty("foo"));
            Assert.IsFalse(two.HasOwnProperty("bar"));

            //Interesting assertions for this test
            Assert.IsTrue(two.HasOwnProperty("baz"));

            Assert.AreEqual("hello there 2 3 4", String.Join(" ", two["baz"] as IEnumerable<object>));
            Assert.AreEqual("hello there", String.Join(" ", one["baz"] as IEnumerable<object>));
        }

        [Test]
        public void ThreeLevelPropertiesWork()
        {
            var baz = new string[] { "hello", "there" };
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            var twoPrime = new HyperDictionary("two'");
            var three = new HyperDictionary("three");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            one.AddChildren(new HyperDictionary[] { two, twoPrime });
            two.ExtendProperty("baz", new object[] { 2, 3, 4 }); //NOTE: Integers, not strings!

            three.InheritsFrom(two);
            three["bar"] = "three bar value";
            three.ExtendProperty("baz", new object[] { "three-baz" });

            //Other verification (sanity)
            Assert.AreEqual("foo value", two["foo"]);
            Assert.AreEqual("bar value", two["bar"]);

            Assert.IsFalse(two.HasOwnProperty("foo"));
            Assert.IsFalse(two.HasOwnProperty("bar"));

            //Interesting assertions for this test
            Assert.AreSame(one["foo"], twoPrime["foo"]);
            Assert.AreSame(one["bar"], twoPrime["bar"]);
            Assert.AreSame(one["baz"], twoPrime["baz"]);
  
            Assert.IsTrue(two.HasOwnProperty("baz"));
            Assert.IsTrue(three.HasOwnProperty("baz"));
            Assert.IsTrue(three.HasOwnProperty("bar"));
            Assert.IsFalse(three.HasOwnProperty("foo"));

            Assert.AreEqual("foo value", three["foo"]);
            Assert.AreEqual("three bar value", three["bar"]);

            Assert.AreEqual("hello there 2 3 4", String.Join(" ", two["baz"] as IEnumerable<object>));
            Assert.AreEqual("hello there", String.Join(" ", one["baz"] as IEnumerable<object>));
            Assert.AreEqual("hello there 2 3 4 three-baz", String.Join(" ", three["baz"] as IEnumerable<object>));
        }

        [Test]
        public void InheritsFromDoesNotAddItemToChildrenOfParent()
        {
            var one = new HyperDictionary();
            var two = new HyperDictionary();
            two.InheritsFrom(one);

            var oneChildrenInfo = one.GetType().GetField("children", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var oneChildren = oneChildrenInfo.GetValue(one) as IEnumerable<HyperDictionary>;
            Assert.IsFalse(oneChildren.Contains(two));
            
        }

        [Test]
        public void AddChildAddsItemToChildrenOfParent()
        {
            var one = new HyperDictionary();
            var two = new HyperDictionary();
            one.AddChild(two);

            var oneChildrenInfo = one.GetType().GetField("children", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var oneChildren = oneChildrenInfo.GetValue(one) as IEnumerable<HyperDictionary>;
            Assert.IsTrue(oneChildren.Contains(two));

        }
        #endregion

        #region ICollection tests
        #region Values
        [Test]
        public void ICollectionValuesOwnedPropertiesWork()
        {
            var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary();
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            var values = one.Values;

            Assert.IsTrue(values.Contains("foo value"));
            Assert.IsTrue(values.Contains("bar value"));
            Assert.IsTrue(values.Contains(baz));
        }

        [Test]
        public void ICollectionValuesInheritedPropertiesWork()
        {
            //var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            //one["baz"] = baz;

            two.InheritsFrom(one);
            two["whatever"] = "big W";

            var ones = one.Values;
            var twos = two.Values;

            Assert.IsTrue(ones.Contains("foo value"));
            Assert.IsTrue(ones.Contains("bar value"));
            //Assert.IsTrue(ones.Contains(baz));
			Assert.IsFalse(ones.Contains("big W"));

            Assert.IsTrue(twos.Contains("foo value"));
            Assert.IsTrue(twos.Contains("bar value"));
            //Assert.IsTrue(twos.Contains(baz));
            Assert.IsTrue(twos.Contains("big W"));
        }

        [Test]
        public void ICollectionValuesOverriddenPropertiesWork()
        {
            //var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            //one["baz"] = baz;

            two.InheritsFrom(one);
            two["bar"] = "two's bar value";

            var ones = one.Values;
            var twos = two.Values;

            Assert.IsTrue(ones.Contains("foo value"));
            Assert.IsTrue(ones.Contains("bar value"));
            //Assert.IsTrue(ones.Contains(baz));

            Assert.IsTrue(twos.Contains("foo value"));
            //Assert.IsTrue(twos.Contains(baz));

            Assert.IsTrue(twos.Contains("two's bar value"));
			Assert.IsFalse(twos.Contains("bar value"));
        }

        [Test]
        public void ICollectionValuesRemovedPropertiesWork()
        {
            //var baz = new Uri("http://blog.tonyheupel.com");
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            //one["baz"] = baz;

            two.InheritsFrom(one);
            two.RemoveProperty("bar", "two's bar value"); //You can still assign a value even though you are removing it

            var ones = one.Values;
            var twos = two.Values;

            Assert.IsTrue(ones.Contains("foo value"));
            Assert.IsTrue(ones.Contains("bar value"));
            //Assert.IsTrue(ones.Contains(baz));

            Assert.IsTrue(twos.Contains("foo value"));
            //Assert.IsTrue(twos.Contains(baz));

            Assert.IsFalse(twos.Contains("two's bar value"));
            Assert.IsFalse(twos.Contains("bar value"));

            try
            {
                var bar = two["bar"];
                Assert.Fail("Should not get this far since this object had this property removed");
            }
            catch (IndexOutOfRangeException)
            {
                //Pass!
            }
            catch (Exception)
            {
                Assert.Fail("Raised an exception that was not IndexOutOfRangeException");
            }

        }

        [Test]
        public void ICollectionValuesExtendedPropertiesWork()
        {
            var baz = new string[] { "hello", "there" };
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            one["baz"] = baz;

            two.InheritsFrom(one);
            var twoBaz = new object[] { 2, 3, 4 }; //NOTE: Integers, not strings!
            two.ExtendProperty("baz", twoBaz);

            var ones = one.Values;
            var twos = two.Values;

            Assert.IsTrue(ones.Contains(baz));
            var tb = twos.ElementAt(0) as IEnumerable<object>;
            Assert.AreEqual("hello there 2 3 4", String.Join(" ", tb));
        }

        [Test]
        public void ICollectionValuesThreeLevelPropertiesWork()
        {
            var baz = new string[] { "hello", "there" };
            var one = new HyperDictionary("one");
            var two = new HyperDictionary("two");
            var twoPrime = new HyperDictionary("two'");
            var three = new HyperDictionary("three");
            one["foo"] = "foo value";
            one["bar"] = "bar value";
            one["baz"] = baz;

            one.AddChildren(new HyperDictionary[] { two, twoPrime });
            two.ExtendProperty("baz", new object[] { 2, 3, 4 }); //NOTE: Integers, not strings!

            three.InheritsFrom(two);
            three["bar"] = "three bar value";
            three.ExtendProperty("baz", new object[] { "three-baz" });

            var ones = one.Values;
            var twos = two.Values;
            var twoPrimes = twoPrime.Values;
            var threes = three.Values;

            //Other verification (sanity)
            Assert.IsTrue(twos.Contains("foo value"));
            Assert.IsTrue(twos.Contains("bar value"));

            //Interesting assertions for this test
            Assert.IsTrue(twoPrimes.Contains("foo value"));
            Assert.IsTrue(twoPrimes.Contains("bar value"));
            Assert.IsTrue(twoPrimes.Contains(baz));

            Assert.IsTrue(threes.Contains("foo value"));
            Assert.IsTrue(threes.Contains("three bar value"));
            Assert.IsFalse(threes.Contains("bar value"));

            //Elements are in order of keys added, starting with the child and then moving up
            var ob = ones.ElementAt(2) as IEnumerable<object>; 
            var tb = twos.ElementAt(0) as IEnumerable<object>;
            var tpb = twoPrimes.ElementAt(2) as IEnumerable<object>;
            var thb = threes.ElementAt(1) as IEnumerable<object>;
            Assert.AreEqual("hello there 2 3 4", String.Join(" ", tb));
            Assert.AreEqual("hello there", String.Join(" ", ob));
            Assert.AreEqual("hello there 2 3 4 three-baz", String.Join(" ", thb));
        }

        #endregion

        #region CopyTo
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ICollectionCopyToThrowsArgumentIndexExceptionWithLessThanZeroArrayIndex()
        {
            new HyperDictionary().CopyTo(new KeyValuePair<string, object>[] { }, -1);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ICollectionCopyToThrowsArgumentIndexExceptionWithEmptyDestinationArray()
        {
            var d = new HyperDictionary();
            d.Add("foo", "bar");
            d.CopyTo(new KeyValuePair<string, object>[] { }, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ICollectionCopyToThrowsArgumentIndexExceptionWithTooSmallDestinationArray()
        {
            var d = new HyperDictionary();
            d.Add("foo", "bar");
            d.Add("baz", "boo");
            d.CopyTo(new KeyValuePair<string, object>[1], 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ICollectionCopyToThrowsArgumentIndexExceptionWithArrayIndexPastEnd()
        {
            var d = new HyperDictionary();
            d.Add("foo", "bar");
            d.Add("baz", "boo");
            d.CopyTo(new KeyValuePair<string, object>[2], 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ICollectionCopyToThrowsArgumentNullExceptionWithNullDestinationArray()
        {
            new HyperDictionary().CopyTo(null, 0);
        }

        [Test]
        public void ICollectionCopyToWithEmptyCollectionWorks()
        {
            var a = new KeyValuePair<string, object>[3];
            a[0] = new KeyValuePair<string, object>("first", null);
            a[1] = new KeyValuePair<string, object>("second", "i'm second");
            new HyperDictionary().CopyTo(a, 2);
            Assert.AreEqual("first", a[0].Key);
            Assert.AreEqual("second", a[1].Key);
            Assert.IsNull(a[2].Key);
            Assert.IsNull(a[2].Value);
        }

        [Test]
        public void ICollectionCopyToWithPositiveArrayIndexWorks()
        {
            var a = new KeyValuePair<string, object>[3];
            a[0] = new KeyValuePair<string, object>("first", null);
            a[1] = new KeyValuePair<string, object>("second", "i'm second");
            var d = new HyperDictionary();
            d.Add("2nd", "HyperDictionary second");
            d.Add("3rd", "HyperDictionary third");
            d.CopyTo(a, 1);
            Assert.AreEqual("first", a[0].Key);
            Assert.AreEqual("2nd", a[1].Key);
            Assert.AreEqual("HyperDictionary second", a[1].Value);
            Assert.AreEqual("3rd", a[2].Key);
            Assert.AreEqual("HyperDictionary third", a[2].Value);
        }

        [Test]
        public void ICollectionCopyToWithZeroArrayIndexWorks()
        {
            var a = new KeyValuePair<string, object>[3];
            a[0] = new KeyValuePair<string, object>("first", null);
            a[1] = new KeyValuePair<string, object>("second", "i'm second");
            a[2] = new KeyValuePair<string, object>("third", "i'm third");
            var d = new HyperDictionary();
            d.Add("1st", "HyperDictionary first");
            d.Add("2nd", "HyperDictionary second");
            
            d.CopyTo(a, 0);
            Assert.AreEqual("1st", a[0].Key);
            Assert.AreEqual("HyperDictionary first", a[0].Value);
            Assert.AreEqual("2nd", a[1].Key);
            Assert.AreEqual("HyperDictionary second", a[1].Value);
            Assert.AreEqual("third", a[2].Key);
            Assert.AreEqual("i'm third", a[2].Value);
        }
        #endregion
        #endregion
    }
}
