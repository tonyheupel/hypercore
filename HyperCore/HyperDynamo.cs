using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Collections;
using System.ComponentModel;

namespace TonyHeupel.HyperCore
{
    /// <summary>
    /// HyperDynamo is a dynamic object that supports JavaScript-like assignment and reading of properties.
    /// <code>
    /// var thing = new HyperDynamo();
    /// thing["prop1"] = "I'm property one";
    /// thing.prop2 = "I'm poperty two";
    /// Console.WriteLine(thing["prop1"]); //Outputs "I'm property one
    /// Console.WriteLine(thing.prop1); //Outputs "I'm property one
    /// Console.WriteLine(thing["prop2"]); //Outputs "I'm property one
    /// Console.WriteLine(thing.prop2); //Outputs "I'm property two
    /// </code>
    /// </summary>
    public class HyperDynamo : DynamicObject, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable, INotifyPropertyChanged
    {
        public HyperDynamo()
            : this(null, null)
        {

        }

        public HyperDynamo(string id)
            : this(id, null)
        {

        }

        public HyperDynamo(IDictionary<string, object> memberProvider)
            : this(null, memberProvider)
        { }

        public HyperDynamo(string id, IDictionary<string, object> memberProvider)
        {
            Id = id;
            MemberProvider = memberProvider;
        }
        public string Id { get; set; }
        public IDictionary<string, object> MemberProvider
        {
            get { return _memberProvider; }
            set
            {
                _memberProvider = value ?? new Dictionary<string, object>();
            }
        }
        // The inner dictionary.
        IDictionary<string, object> _memberProvider = null;

        #region Tony's Indexer coolness so this acts more like JavaScript
        public virtual object this[string name]
        {
            get { return _memberProvider[name]; }
            set 
            {
                bool notifyChange = false;
                if (_memberProvider.ContainsKey(name) && _memberProvider[name] != value) notifyChange = true;

                _memberProvider[name] = value;

                //Support INotifyPropertyChanged
                if (notifyChange && PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }
        #endregion
        
        #region DynamicObject overrides
        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // If the property name is found in the dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return _memberProvider.TryGetValue(binder.Name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            bool notifyChange = true;
            object currentValue;
            if (_memberProvider.TryGetValue(binder.Name, out currentValue))
            {
                notifyChange = currentValue != value;
            }

            _memberProvider[binder.Name] = value;

            //Support INotifyPropertyChanged
            if (notifyChange && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(binder.Name));
            }

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
        #endregion

        #region Dictionary support
        #region IDictionary<string,object> Members

        public void Add(string key, object value)
        {
            _memberProvider.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _memberProvider.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _memberProvider.Keys; }
        }

        public bool Remove(string key)
        {
            return _memberProvider.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _memberProvider.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return _memberProvider.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<string,object>> Members

        public void Add(KeyValuePair<string, object> item)
        {
            _memberProvider.Add(item);
        }

        public void Clear()
        {
            _memberProvider.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _memberProvider.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _memberProvider.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _memberProvider.Count; }
        }

        public bool IsReadOnly
        {
            get { return _memberProvider.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _memberProvider.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _memberProvider.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _memberProvider.GetEnumerator();
        }

        #endregion
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
