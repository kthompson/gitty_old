using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gitty
{
    public partial class Git
    {
        public class Base
        {

            public static Base Bare(string git_dir)
            {
                return Bare(git_dir, null);
            }
            /// <summary>
            /// opens a bare git repo
            /// </summary>
            /// <param name="git_directory"></param>
            /// <returns></returns>
            public static Base Bare(string git_dir, Options opts)
            {
                Options defaults = new Options { { "repository", new FileInfo(git_dir).FullName } };
                Options git_options = defaults.Merge(opts);
                return new Base(git_options);
            }

            public static Base Open(string working_directory)
            {
                return Open(working_directory, null);
            }

            /// <summary>
            /// opens a new Git Project from a working directory
            /// you can specify non-standard git_dir and index file in the options
            /// </summary>
            /// <param name="working_directory"></param>
            /// <returns></returns>
            public static Base Open(string working_directory, Options opts)
            {
                Options defaults = new Options {{"working_directory" , File.ExpandPath(working_directory)}};
                Options git_options = defaults.Merge(opts);
                return new Base(git_options);
            }



            public static Base Init(string working_directory, Options opts)
            {
                Options defaults = new Options { { "working_directory", File.ExpandPath(working_directory) },
                                                 { "repository", File.Join(working_directory, ".git")}
                };
                Options git_options = defaults.Merge(opts);

                
                if (git_options["working_directory"].IsNotNullOrEmpty())
                {
                    if (!Directory.Exists(git_options["working_directory"].As<string>()))
                        Directory.CreateDirectory(git_options["working_directory"].As<string>());

                    if(git_options["repository"].IsNullOrEmpty())
                        git_options["repository"] = File.Join(working_directory, ".git");

                }


                Repository.Init(git_options["repository"].As<string>());

                return new Base(git_options);
            }

            public static Base Clone(string repository, string name)
            {
                return Clone(repository, name, null);
            }

            public static Base Clone(string repository, string name, Options options)
            {
                return new Base(new Lib(null, options["logger"]).Clone(repository, name, options));
            }


            private Base()
                : this(null)
            {
            }

            private Base(Options options)
            {
                string working_dir = options["working_directory"].As<string>();
                if (working_dir.IsNotNullOrEmpty())
                {
                    if (options["repository"])
                        options["repository"] = File.Join(working_dir, ".git");

                    if (options["index"])
                        options["index"] = File.Join(working_dir, ".git", "index");

                    if (!options["logger"])
                    {

                    }
                }
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
