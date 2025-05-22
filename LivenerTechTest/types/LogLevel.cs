// ReSharper disable InconsistentNaming

namespace LivenerTechTest.types;

public enum LogLevel
{
    STREAM_START,
    STREAM_STOP,
    STREAM_RESTART,
    STREAM_RECONNECT,
    USER_JOIN,
    USER_LEAVE,
    INFO,
    DEBUG,
    WARNING,
    ERROR,
    CRITICAL,
    UNKNOWN
}