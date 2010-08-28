HyperCore
===============

This release includes:
  HyperHypo --> A dynamic class that enables JavaScript-like
                syntax with object definition and behavior 
                with prototypes--all within C#!
                This is the core of a HyperJS in my HyperActive
                playground right now; a JS.cs implementation 
                within C# (so no separate language, just a style choice).
                Thus, the name: 
                    Hyper - more than the C# dynamic type
                            since it supports ["name"] 
                            property declaration and notation;
                    Hypo  - less than JavaScript since
                            I'm not really defining a new
                            runtime/VM or language that can
                            have a full syntax (for now;
                            maybe more F# lexer/parser fun
                            is in order at some point).
                HyperHypo is currently implemented as a subclass
                of HyperDynamo that requires that a 
                HyperDictionary is the IDictionary<string, object>
                MemberProvider (see below for those).
                

  HyperDynamo --> A dynamic object class that is similar to 
                  ExpandoObject in that you can get/set members
                  dynamically and iterate over the members; 
                  however, HyperDynamo instances can use  
                  key/indexer notation to get and set members as 
                  well as access them via dot-notation as any 
                  other member.  
                  If you want, you can also
                  specify an IDictionary<string, object> that
                  provides the key/value pairs of members and 
                  values (by default it simply uses 
                  Dictionary<string, object>).
                  This is good for a type of meta programming
                  where member names are in strings and you
                  want to add them and have them available via
                  dot-notation (reading config settings from
                  a database, enabling notation similar to 
                  typed-datasets without requiring a schema to
                  be generated, etc.).
                  
                  For example:
                  
                  dynamic foo = new HyperDynamo();
                  foo["bar"] = "baz";
                  foo.fizz = "buzz";
                  Console.WriteLine(foo.bar);  // Outputs "baz".
                  
                  for (object o in foo)
                  {
                    Console.WriteLine(o);
                  }
                  /* Outputs:
                     [bar, baz]
                     [fizz, buzz]
                  */


  HyperDictionary --> A Dictionary implementation that supports 
                     "inheritance" by letting you specify a "parent"
                     HyperDictionary (similar to JavaScript prototypes).  
                     The effect is similar to appSettings for 
                     web.config in ASP.NET in that you can "add" 
                     key/value pairs that either add the item or 
                     override an already existing key/value pair 
                     with that key.  
                     You can also "remove" a setting or, in the 
                     case of a value that implements IEnumerable, 
                     you can "extend" a setting.
                     The intent is any configuration settings with
                     overrides, maybe even a Localization Resource
                     Dictionary, etc.


  HyperDictionaryEnumerator	--> An implementation of 
                                IEnumerator<KeyValuePair<string, object>
                                to support enumerating through Keys in the 
                                HyperDictionary as if it was all one Dictionary.

