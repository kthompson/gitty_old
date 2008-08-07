using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gitty.Util;
using Gitty.Exceptions;

namespace Gitty.Lib
{
    public class LockFile
    {
        private FileInfo refFile;
        private FileInfo lockFile;

        private FileStream os;

        private FileLock fLck;

        public LockFile(FileInfo file)
        {
            refFile = file;
            lockFile = PathUtil.CombineFilePath(refFile.Directory, refFile.Name + ".lock");
        }

        public bool Lock()
        {
            lockFile.Directory.Create();
            try
            {
                os = lockFile.Create();
            }
            catch (Exception)
            {
            }

            if (os != null)
            {
                fLck = FileLock.TryLock(os);
                if (fLck == null)
                {
                    // We cannot use unlock() here as this file is not
                    // held by us, but we thought we created it. We must
                    // not delete it, as it belongs to some other process.
                    //
                    try
                    {
                        os.Close();
                    }
                    catch (Exception)
                    {
                        // Fail by returning haveLck = false.
                    }
                    os = null;
                }

            }
            return fLck != null;
        }

        private void Unlock()
        {
            throw new NotImplementedException();
        }

        public bool Commit()
        {
            throw new NotImplementedException();
        }

        internal void Write(byte[] content)
        {
            throw new NotImplementedException();
        }

        public class FileLock : IDisposable
        {
            public FileStream FileStream { get; private set; }
            public bool Locked { get; private set; }

            private FileLock(FileStream fs)
            {
                this.FileStream = fs;
                this.FileStream.Lock(0, fs.Length);
                this.Locked = true;
            }

            public static FileLock TryLock(FileStream fs)
            {
                try
                {
                    return new FileLock(fs);
                }
                catch (IOException)
                {

                    return null;
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                this.Release();
            }

            public void Release()
            {
                this.FileStream.Unlock(0, this.FileStream.Length);
                this.Locked = false;
            }

            #endregion
        }
        
    }
}
