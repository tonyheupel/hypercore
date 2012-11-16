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

            this.hyperDictionary = hyperDictionary;

            this.currentEnumerator = includeParents ? hyperDictionary.Keys.GetEnumerator() : hyperDictionary.OwnKeys.GetEnumerator();
        }

        private HyperDictionary hyperDictionary = null;
        private IEnumerator<string> currentEnumerator = null;

        protected KeyValuePair<string, object> GetCurrent()
        {
            return new KeyValuePair<string, object>(currentEnumerator.Current, hyperDictionary[currentEnumerator.Current]);
        }

        //TODO: Determine if the Enumerator this supports is even needed
        protected KeyValuePair<string, Tuple<PropertyAction, object>> GetCurrentTuple()
        {
            return new KeyValuePair<string, Tuple<PropertyAction, object>>(currentEnumerator.Current, hyperDictionary.GetPropertyTuple(currentEnumerator.Current));
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
            return currentEnumerator.MoveNext();
        }

        public void Reset()
        {
            currentEnumerator.Reset();
        }

        #endregion
    }
}
