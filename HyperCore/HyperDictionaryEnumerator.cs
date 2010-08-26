using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TonyHeupel.HyperCore
{
    //TODO: Determine if we can meaningfully use the IEnumerator<KeyValuePair<string, Tuple<PropertyAction, object>>>, or just eliminate it
    public class HyperDictionaryEnumerator : IEnumerator<KeyValuePair<string, object>>, IEnumerator<KeyValuePair<string, Tuple<PropertyAction, object>>>
    {
        public HyperDictionaryEnumerator(HyperDictionary hyperDictionary) : this(hyperDictionary, true) { }

        public HyperDictionaryEnumerator(HyperDictionary hyperDictionary, bool includeParents)
        {
            if (hyperDictionary == null) throw new ArgumentNullException("The parameter must be non-null");

            _hyperDictionary = hyperDictionary;

            _currentEnumerator = includeParents ? _hyperDictionary.Keys.GetEnumerator() : _hyperDictionary.OwnKeys.GetEnumerator();
        }

        private HyperDictionary _hyperDictionary = null;
        private IEnumerator<string> _currentEnumerator = null;

        protected KeyValuePair<string, object> GetCurrent()
        {
            return new KeyValuePair<string, object>(_currentEnumerator.Current, _hyperDictionary[_currentEnumerator.Current]);
        }

        //TODO: Determine if the Enumerator this supports is even needed
        protected KeyValuePair<string, Tuple<PropertyAction, object>> GetCurrentTuple()
        {
            return new KeyValuePair<string, Tuple<PropertyAction, object>>(_currentEnumerator.Current, _hyperDictionary.GetPropertyTuple(_currentEnumerator.Current));
        }

        #region IEnumerator<KeyValuePair<string, object>> Members

        public KeyValuePair<string, object> Current
        {
            get { return GetCurrent(); }
        }

        #endregion

        #region IEnumerator<KeyValuePair<string, Tuple<PropertyAction, object>>> Members
        //TODO: Determine if this member is even needed
        KeyValuePair<string, Tuple<PropertyAction, object>> IEnumerator<KeyValuePair<string, Tuple<PropertyAction, object>>>.Current
        {
            get { return GetCurrentTuple(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //Do nothing for now
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return GetCurrent(); }
        }

        public bool MoveNext()
        {
            return _currentEnumerator.MoveNext();
        }

        public void Reset()
        {
            _currentEnumerator.Reset();
        }

        #endregion
    }
}
