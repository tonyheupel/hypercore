using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TonyHeupel.HyperCore
{
    public class HyperHypo : HyperDynamo
    {
        public HyperHypo() : this(null) { }

        public HyperHypo(HyperHypo prototype)
        {
            this.MemberProvider = new HyperDictionary();
            Prototype = prototype;
        }

        private HyperHypo _prototype = null;
        public HyperHypo Prototype
        {
            get { return _prototype; }
            set
            {
                _prototype = value;
                if (_prototype == null) return;

                //Use InheritsFrom so we don't create some crazy all-encompassing graph of things
                this.MemberProvider.InheritsFrom(_prototype.MemberProvider);
            }
        }
        public new HyperDictionary MemberProvider
        {
            get { return base.MemberProvider as HyperDictionary; }
            set { base.MemberProvider = value; }
        }

        public virtual bool HasOwnProperty(string name)
        {
            return this.MemberProvider.HasOwnProperty(name);
        }
    }
}
