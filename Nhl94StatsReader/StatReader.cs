using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class StatReader : IStatReader, IDisposable
    {
        #region Properties

            FileStream _fileStream;
            private String _saveStatePath;
            // Flag: Has Dispose already been called? 
            bool disposed = false;

        #endregion

        #region Constructors

        public StatReader()
            {
            }

            public StatReader(String SaveStatePath)
            {
                SetSaveStatePath(SaveStatePath);
            }

            ~StatReader()
            {
                Dispose();
            }

        #endregion        

        #region Methods
        
            public void SetSaveStatePath(String SaveStatePath)
            {
                if (_saveStatePath == null)
                {
                    _saveStatePath = SaveStatePath;
                    _fileStream = File.OpenRead(_saveStatePath);
                }
            }

            public byte ReadStat(long offset)
            {
                byte _result;

                BinaryReader w = new BinaryReader(_fileStream);
                _fileStream.Seek(offset, SeekOrigin.Begin);
                _result = w.ReadByte();

                return _result;

            }

            public void Close()
            {
                _fileStream.Close();
            }

        public void Dispose()
        {
            Dispose(true);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _fileStream.Close();
            }

            Console.WriteLine("Disposing");

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
    }

    #endregion

}
