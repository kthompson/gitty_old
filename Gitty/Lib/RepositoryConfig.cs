using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gitty.Lib
{
    public class RepositoryConfig
    {
        public sealed class Constants
        {
            public static readonly string MagicEmptyValue = "%%magic%%empty%%";
            public static readonly string RemoteSection = "remote";
            public static readonly string BranchSection = "branch";
        }
        

        public RepositoryConfig(Repository repo)
            : this(OpenUserConfig(), new FileInfo(Path.Combine(repo.Directory.FullName, "config")))
        {
         
        }

        public RepositoryConfig(RepositoryConfig @base, FileInfo configFile)
        {

        }

        public CoreConfig Core { get; private set; }

        public static RepositoryConfig OpenUserConfig()
        {
            throw new NotImplementedException();
        }

        public Repository Repository { get; private set; }

        internal void Load()
        {
            throw new NotImplementedException();
        }

        internal void Create()
        {
            throw new NotImplementedException();
        }

        public string GetString(string section, string subsection, string name)
        {
            string val = GetRawString(section, subsection, name);
            if (Constants.MagicEmptyValue.Equals(val))
            {
                return "";
            }

            return val;
        }

        private string GetRawString(string section, string subsection, string name)
        {
            throw new NotImplementedException();
        }

        class Entry {
            public string Prefix { get; set; }
            public string Base { get; set; }
            public string ExtendedBase { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Suffix { get; set; }

		    public bool Match(String aBase, String aExtendedBase, String aName) {
			    return eq(this.Base, aBase) 
                    && eq(this.ExtendedBase, aExtendedBase)
					&& eq(this.Name, aName);
		    }

		    private static bool eq(String a, String b) {
			    if (a == b)
				    return true;
			    if (a == null || b == null)
				    return false;
			    return a.Equals(b);
		    }
	    }


        internal void Save()
        {
            throw new NotImplementedException();
        }
    }
}
