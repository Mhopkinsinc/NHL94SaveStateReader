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

        #endregion

        #region Constructors

        public StatReader()
            {
            }

            public StatReader(String SaveStatePath)
            {
                SetSaveStatePath(SaveStatePath);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _fileStream.Close();
                }                

                disposedValue = true;
            }
        }

        ~StatReader()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    #endregion

}
