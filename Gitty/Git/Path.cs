using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gitty
{
    public partial class Git
    {
        public class Path
        {
            private FileInfo _path;            

            public Path(string path, bool checkPath)
            {
                if (!checkPath || File.Exists(path))
                    _path = new FileInfo(path);
                else
                    throw new ArgumentException("Path does not exist.", "path");
            }

            public Path(string path)
                : this(path, true)
            {

            }

            
            public bool IsReadable()
            {
                using(FileStream file = new FileStream(_path.FullName, FileMode.OpenOrCreate)){
                    return file.CanRead;
                }
            }

            public bool IsWritable()
            {
                using(FileStream file = new FileStream(_path.FullName, FileMode.Append)){
                    return file.CanWrite;
                }
            }

            public override string ToString()
            {
                return _path.FullName;
            }
            
            
	
        }
    }
}
