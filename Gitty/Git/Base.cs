using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public class Base
        {
            /// <summary>
            /// opens a bare git repo
            /// </summary>
            /// <param name="git_directory"></param>
            /// <returns></returns>
            public static Base Bare(string gitDirectory, Options options)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// opens a new Git Project from a working directory
            /// you can specify non-standard git_dir and index file in the options
            /// </summary>
            /// <param name="working_directory"></param>
            /// <returns></returns>
            public static Base Open(string working_directory, Options options)
            {
                throw new NotImplementedException();
            }



            public static Base Init(string working_directory, Options options)
            {
                throw new NotImplementedException();
            }

            public static Base Clone(string repository, string name, Options options)
            {
                throw new NotImplementedException();
            }


            private Base()
                : this(null)
            {
            }

            private Base(Options options)
            {
                throw new NotImplementedException();
            }

            private WorkingDirectory _workingDirectory;

            public WorkingDirectory WorkingDirectory
            {
                get { return _workingDirectory; }
                set {
                    throw new NotImplementedException();
                    _workingDirectory = value; 
                }
            }

            private Repository _repository;
            public Repository Repository
            {
                get { return _repository; }
            }

            private Index _index;
            public Index Index
            {
                get { return _index; }
                set {
                    throw new NotImplementedException();
                    _index = value; 
                }
            }

            public void ChangeDirectory(string directory)
            {
                throw new NotSupportedException();
            }

            public int RepoSize()
            {
                throw new NotSupportedException();
            }

            public void Config(string name, string value)
            {
                throw new NotImplementedException();
            }

            public string Config(string name)
            {
                throw new NotImplementedException();
            }

            public Dictionary<string,string> Config()
            {
                throw new NotImplementedException();
            }

            public Object Object(string objectish)
            {
                throw new NotImplementedException();
            }

            public Object GTree(string objectish)
            {
                throw new NotImplementedException();
            }

            public Object GCommit(string objectish)
            {
                throw new NotImplementedException();
            }

            public Object GBlob(string objectish)
            {
                throw new NotImplementedException();
            }

            public Log Log()
            {
                return this.Log(30);
            }

            public Log Log(int count)
            {
                throw new NotImplementedException();
            }

            public Status Status()
            {
                return new Status(this);
            }


        }
    }
}
