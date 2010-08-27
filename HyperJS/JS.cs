using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TonyHeupel.HyperCore;

namespace TonyHeupel.HyperJS
{
    /// <summary>
    /// The Global object.
    /// JS.cs - Playing with JavaScript-type object creation within C# using closures
    /// and dynamic types with my HyperHypo (more than C#, less than JavaScript) class.
    /// </summary>
    public class JS : HyperHypo
    {
        /// <summary>
        /// cs is the instance accessor for the JS class so that instead of using
        /// new Image(), you can use JS.cs.Image()
        /// </summary>
        public static dynamic cs { get { return _globalObject; } }
        private static dynamic _globalObject = new JS();
        
        private JS ()
	    {
            dynamic that = this;  //Fun JavaScript trick to make this a dynamic so we can just bind things to it

            #region Just playing around...
            that.Image = new Func<int, int, dynamic>(delegate(int width, int height)
            {
                //dynamic img = new HyperHypo(Object());
                dynamic img = new HyperHypo();
                img.Prototype = Object(img);
                img.width = width;
                img.height = height;

                img.doStuff = new Func<string, string>(name => string.Format("[name: {0}, width: {1}, height: {2}]", name, img.width, img.height));

                //Private variables?
                var _iamprivate = "private var";

                img.getPrivate = new Func<string>(delegate() { return _iamprivate; });
                img.setPrivate = new Func<string, object>(delegate(string newPrivate) { _iamprivate = newPrivate; return null; });

                return img;
            });
            #endregion Just playing around...
        }

        #region Just playing around
        public static dynamic Image()
        {
            return Image(0, 0);
        }
        public static dynamic Image(int width, int height)
        {
            return cs.Image(width, height);
        }
        #endregion
        #region undedfined
        private class Undefined
        {
            public override string ToString()
            {
                return "undefined";
            }
        }
        private static readonly Undefined _undefined = new Undefined();
        public static dynamic undefined { get { return _undefined; } }
        #endregion

        #region NaN - TODO: redefine in terms of something else?
        private class NaNClass
        {
            public override string ToString()
            {
                return "NaN";
            }
        }
        private static readonly NaNClass _nan = new NaNClass();
        public static dynamic NaN { get { return _nan; } }
        #endregion

        #region Infinity - TODO: redefine this in terms of Numeric later and reference Numeric.POSITIVE_INFINITY constant?
        private class InfinityClass
        {
            public override string ToString()
            {
                return "Infinity";
            }
        }
        private static readonly InfinityClass _infinity = new InfinityClass();
        public static dynamic Infinity { get { return _infinity; } }
        #endregion

        #region junk - Java-related items
        #region Packages - TODO: redefine this in terms of Object later?
        private class JavaPackage
        {
            string Name { get; set; }
            private List<JavaPackage> _children = new List<JavaPackage>();
            IEnumerable<JavaPackage> Packages
            {
                get
                {
                    return _children;
                }
            }
            //TODO: Figure out what to do here--probably nothing
            public override string ToString()
            {
                return "JavaPackage";
            }
        }
        private static readonly JavaPackage _packages = new JavaPackage();
        public static dynamic Packages { get { return _packages; } }

        public static dynamic java { get { return _packages; } } //TODO: Fix this so there is a java.* package and that this returns that
        #endregion
        #endregion

        #region Global Functions

        #region Global Objects

        public static dynamic Array()
        {
            throw new NotImplementedException();
        }

        public static dynamic Boolean()
        {
            throw new NotImplementedException();
        }

        public static dynamic Date()
        {
            throw new NotImplementedException();
        }

        public static dynamic Error()
        {
            throw new NotImplementedException();
        }

        public static dynamic EvalError()
        {
            throw new NotImplementedException();
        }

        public static dynamic Function()
        {
            throw new NotImplementedException();
        }

        public class Math : HyperHypo
        {

        }

        public static dynamic Number()
        {
            throw new NotImplementedException();
        }

        #region Object
        public static dynamic Object(HyperHypo self = null)
        {
            dynamic o = new HyperHypo();
            self = self ?? o;
            o.toString = new Func<string>(delegate() { return BaseToString(self); });

            return o;
        }

        private static string BaseToString(HyperHypo hh)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Number of members: {0}", hh.Count));
            foreach (object o in hh)
            {
                sb.AppendLine(o.ToString());
            }

            return sb.ToString();
        }
        #endregion

        public static dynamic RangeError()
        {
            throw new NotImplementedException();
        }

        public static dynamic ReferenceError()
        {
            throw new NotImplementedException();
        }

        public static dynamic RegExp()
        {
            throw new NotImplementedException();
        }

        public static dynamic String()
        {
            throw new NotImplementedException();
        }

        public static dynamic SyntaxError()
        {
            throw new NotImplementedException();
        }

        public static dynamic TypeError()
        {
            throw new NotImplementedException();
        }

        public static dynamic URIError()
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}
