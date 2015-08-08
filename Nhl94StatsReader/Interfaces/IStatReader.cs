
namespace Nhl94StatsReader
{
    public interface IStatReader
    {
        byte ReadStat(long offset);
        int ReadlLittleEndian(long offset1, long offset2);
        void Close();
    }
}
