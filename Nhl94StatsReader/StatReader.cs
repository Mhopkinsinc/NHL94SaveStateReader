using System;
using System.IO;
using NLog;


namespace Nhl94StatsReader
{
    public class StatReader : IStatReader, IDisposable
    {
        #region Properties

        FileStream _fileStream;
        private String _saveStatePath;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors       

        public StatReader(String SaveStatePath)
        {
            SetSaveStatePath(SaveStatePath);
        }

        #endregion

        #region Methods

        private void SetSaveStatePath(String SaveStatePath)
        {
            if (_saveStatePath == null)
            {
                _saveStatePath = SaveStatePath;

                try
                {
                    _fileStream = File.OpenRead(_saveStatePath);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    throw;
                }

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

        public int ReadlLittleEndian(long offset1, long offset2)
        {

            string statValueHex;
            int result;

            statValueHex = ReadStat(offset2).ToString("X");
            statValueHex += ReadStat(offset1).ToString("X");
            result = Convert.ToInt32(statValueHex, 16);

            return result;            

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
                    logger.Info("StatReader Dispose() _fileStream.Close(), disposing={0}, dispsedValue={1}", disposing, disposedValue);
                }

                _fileStream = null;
                disposedValue = true;
            }
        }

        ~StatReader()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.            
            Dispose(false);
            logger.Info("Dispose(false) Called");
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.            
            Dispose(true);
            logger.Info("Public Dispose(true) Called");
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    #endregion

}
