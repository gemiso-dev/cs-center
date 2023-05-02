using NLog;

namespace sequence_maker.Services
{
    public interface ILogManager
    {
        Logger Logger { get; }
    }
}
