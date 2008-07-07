using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public class Options : NestedDictionaryBase<string, object, NestedDictionary<string, object>>
    {
        public Options()
        {
        }

        public Options Merge(Options m)
        {
            return Options.Merge(this, m);
        }

        public static Options Merge(Options options1, Options options2)
        {
            Options options = new Options();

            if (options1 != null)
            {
                foreach (KeyValuePair<string, NestedDictionary<string, object>> kv in options1)
                {
                    options.Add(kv.Key, kv.Value);
                }
            }

            if (options2 != null)
            {
                foreach (KeyValuePair<string, NestedDictionary<string, object>> kv in options2)
                {
                    if (options.ContainsKey(kv.Key))
                        options[kv.Key] = kv.Value;
                    else
                        options.Add(kv.Key, kv.Value);
                }
            }
            return options;
        }

        
    }
}
