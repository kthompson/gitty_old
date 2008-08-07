using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Gitty.Util
{
    /// <summary>
    /// Static class used for obataining locks on files for system-wide locking
    /// </summary>
    public static class FileLock
    {

        #region Constructors

        static FileLock()
        {
            LockTable = new Dictionary<string, FileStream>();
            Lock = new ReaderWriterLock();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Obtains a lock for the specified file and returns true if successful
        /// If the file doesn't exist then it will be created
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FileStream ObtainLock(string file)
        {
            try
            {
                Lock.AcquireReaderLock(Timeout.Infinite);

                //If the lock already exists return failure
                if (LockTable.ContainsKey(file))
                    return null;


                LockCookie cookie = new LockCookie();
                try
                {
                    //Upgrade to a writer lock since we're going to add it
                    cookie = Lock.UpgradeToWriterLock(Timeout.Infinite);

                    //Obtain a lock on the file or create it
                    FileStream stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                    LockTable.Add(file, stream);
                    return stream;
                }
                catch (System.IO.IOException)
                {
                }
                catch (System.ArgumentOutOfRangeException)
                {
                }
                finally
                {
                    //simply downgrade and we'll release the reader lock later
                    Lock.DowngradeFromWriterLock(ref cookie);
                }

                return null;
            }
            finally
            {
                Lock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Releases the specified FileLock if held by the application
        /// </summary>
        /// <param name="file"></param>
        public static void ReleaseLock(string file)
        {
            ReleaseLock(file, false);
        }

        /// <summary>
        /// Releases the specified FileLock if held by the application
        /// </summary>
        /// <param name="file"></param>
        /// <param name="delete"></param>
        public static void ReleaseLock(string file, bool delete)
        {
            try
            {
                Lock.AcquireWriterLock(Timeout.Infinite);
                FileStream stream = LockTable[file];

                if (stream != null)
                {
                    stream.Close();
                    LockTable.Remove(file);

                    if (delete)
                        File.Delete(file);
                }
            }
            finally
            {
                Lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Releases all FileLocks held
        /// </summary>
        public static void ReleaseLocks()
        {
            try
            {
                Lock.AcquireWriterLock(Timeout.Infinite);

                //Close all the streams
                foreach (FileStream stream in LockTable.Values)
                    stream.Close();

                //Clear all the locks
                LockTable.Clear();
            }
            finally
            {
                Lock.ReleaseWriterLock();
            }
        }

        #endregion

        #region Properties and Members

        private static Dictionary<string, FileStream> LockTable;
        private static ReaderWriterLock Lock;

        #endregion

    }

}
