using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace TonyHeupel.HyperCore
{
    /// <summary>
    /// Describes the action for the property relative to the object's parent.
    /// Add is the same as Add or Override, Remove essentially hides the item,
    /// and Extend requires an IEnumerable of object.
    /// </summary>
    public enum PropertyAction
    {
        Add,
        Remove,
        Extend
    }

    /// <summary>
    /// HyperDictionary is a class describing a level of dictionary that
    /// supports inhertied, overridden, and extended values.  Values 
    /// for a given level of HyperDictionary either live directly on that instance
    /// or in a parent, or both.
    /// </summary>
    public class HyperDictionary : IDictionary<string, object>
    {
        public string Id { get; set; }

        #region Fields
        //TODO: Consider creating a class instead of just using a Tuple
        private Dictionary<string, Tuple<PropertyAction, object>> properties = new Dictionary<string, Tuple<PropertyAction, object>>();

        private HyperDictionary parent = null;

        private IList<HyperDictionary> children = new List<HyperDictionary>();
        #endregion

        #region Constructors
        public HyperDictionary()
            : this(null)
        {

        }
        public HyperDictionary(string id)
        {
            Id = id;
        }
        #endregion

        #region Property Magic
        public object this[string name]
        {
            get
            {
                return GetProperty(name);
            }
            set
            {
                //By default, simply use the "Add" action; this will be true in most cases
                AddProperty(name, value);
            }
        }

        public object GetProperty(string name)
        {
            var property = GetPropertyTuple(name);

            return GetPropertyFromTuple(name, property);
        }

        protected object GetPropertyFromTuple(string name, Tuple<PropertyAction, object> property)
        {
            if (property.Item1 == PropertyAction.Remove) throw new IndexOutOfRangeException(String.Format("Object \"{0}\" has removed the property with name \"{1}\"", Id, name));

            if (property.Item1 == PropertyAction.Extend)
            {
                return GetExtendedProperty(name);
            }

            return property.Item2;
        }

        #region Tuple Getters
        public Tuple<PropertyAction, object> GetPropertyTuple(string name)
        {
            if (!HasOwnProperty(name)) return GetAncestorPropertyTuple(name);

            //Do this once to prevent multiple lookups
            return GetOwnPropertyTuple(name);
        }

        public Tuple<PropertyAction, object> GetAncestorPropertyTuple(string name)
        {
            if (parent == null) throw new IndexOutOfRangeException(String.Format("Object \"{0}\" and it's ancestors do not have a property with name \"{1}\"", Id, name));

            return parent.GetPropertyTuple(name);
        }
        public Tuple<PropertyAction, object> GetOwnPropertyTuple(string name)
        {
            return properties[name];
        }

        public IEnumerable<KeyValuePair<string, Tuple<PropertyAction, object>>> OwnTuples
        {
            get { return properties; }
        }
        #endregion
        public ICollection<string> OwnKeys
        {
            get { return properties.Keys; }
        }

        public bool TryGetProperty(string name, out object value)
        {
            Tuple<PropertyAction, object> tuple;

            if (TryGetPropertyTuple(name, out tuple))
            {
                try
                {
                    value = GetPropertyFromTuple(name, tuple);
                    return true;
                }
                catch (IndexOutOfRangeException)
                {
                    //Likely removed
                }
            }

            value = null;
            return false;
        }

        public bool TryGetPropertyTuple(string name, out Tuple<PropertyAction, object> value)
        {
            try
            {
                value = GetPropertyTuple(name);
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                value = null;
                return false;
            }
        }

        private IEnumerable<object> GetExtendedProperty(string name)
        {
            //First, go up to parents and then union with our Own property values
            object outValues;
            IEnumerable<object> values;
            if (parent != null && parent.TryGetProperty(name, out outValues))
            {
                //Got it
                values = outValues as IEnumerable<object>;
            }
            else
            {
                //Initialize to empty?  Maybe shouldn't yet...
                values = new object[] { };
            }


            if (HasOwnProperty(name) && properties[name].Item1 == PropertyAction.Extend)
            {
                values = values.Union(properties[name].Item2 as IEnumerable<object>);
            }

            return values;
        }

        public bool HasOwnProperty(string name)
        {
            return properties.ContainsKey(name);
        }

        public bool ClearOwnProperty(string name)
        {
            if (!HasOwnProperty(name)) throw new IndexOutOfRangeException(String.Format("Object \"{0}\" does not have an owned property with name \"{1}\"", Id, name));

            return properties.Remove(name);
        }
                
        public void AddProperty(string name, object value)
        {
            properties[name] = new Tuple<PropertyAction, object>(PropertyAction.Add, value);
        }

        public bool RemoveProperty(string name)
        {
            return RemoveProperty(name, null);
        }

        public bool RemoveProperty(string name, object value)
        {
            try
            {
                properties[name] = new Tuple<PropertyAction, object>(PropertyAction.Remove, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ExtendProperty(string name, IEnumerable<object> values)
        {
            properties[name] = new Tuple<PropertyAction, object>(PropertyAction.Extend, values);
        }
        #endregion

        #region Children and Parents
        public void AddChild(HyperDictionary child)
        {
            children.Add(child);
            child.parent = this;
        }

        public void AddChildren(IEnumerable<HyperDictionary> children)
        {
            //Can't just use AddRange because of _parent setting in AddChild
            foreach (HyperDictionary child in children)
            {
                AddChild(child);
            }
        }

        /// <summary>
        /// InheritsFrom simply sets the parent but does not add this HyperDictionary
        /// to the Children collection in the Parent; this is a one-way connection
        /// only up to the parent.
        /// </summary>
        /// <param name="parent"></param>
        public void InheritsFrom(HyperDictionary parent)
        {
            this.parent = parent;
        }
        #endregion


        #region IDictionary<string,object> Members

        public void Add(string key, object value)
        {
            AddProperty(key, value);
        }

        public bool ContainsKey(string key)
        {
            try
            {
                var o = this[key];
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }

        }

        public ICollection<string> Keys
        {
            get 
            {
                //return new List<string>(this.Select(kvp => kvp.Key));
                var removedKeys = new HashSet<string>();
                var keys = new HashSet<string>();
                var current = this;
                HyperDictionary nextParent = null;
                do
                {
                    nextParent = current.parent;
                    foreach (string key in current.properties.Keys)
                    {
                        if (!removedKeys.Contains(key) && !keys.Contains(key))
                        {
                            object o;
                            if (this.TryGetProperty(key, out o))
                            {
                                keys.Add(key);
                            }
                            else
                            {
                                //Removed, so add it here
                                removedKeys.Add(key);
                            }
                        }
                    }
                    current = nextParent;
                } while (nextParent != null);

                return keys;
            }
        }

        public bool Remove(string key)
        {
            return ClearOwnProperty(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return TryGetProperty(key, out value);
        }

        public ICollection<object> Values
        {
            get 
            {
                return new List<object>(this.Select(kvp => kvp.Value));
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,object>> Members

        public void Add(KeyValuePair<string, object> item)
        {
            AddProperty(item.Key, item.Value);
        }

        public void Clear()
        {
            //Specifically do NOT allow this instance to clear a parent instance
            properties.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            try
            {
                var o = this[item.Key];
                return Object.Equals(o, item.Value);
            }
            catch(IndexOutOfRangeException)
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("The destination array must be a non-null array");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("The arrayIndex must be zero or greater.");

            var keys = this.Keys;
            if (keys.Count > array.Length - arrayIndex) throw new ArgumentOutOfRangeException("The destination array does not have enough room to complete this operation.");

            foreach (var key in keys)
            {
                array[arrayIndex++] = new KeyValuePair<string, object>(key, this[key]);
            }
        }

        public int Count
        {
            get { return this.Keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; } //Not sure when I would set this to true? 
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return RemoveProperty(item.Key, item.Value);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return new HyperDictionaryEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new HyperDictionaryEnumerator(this);
        }

        #endregion
    }

    
}
