namespace MobileAPI.Middlewares;

public static class LogTemplates
{
    public const string File =
@"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}]
{Message:lj}
{NewLine}--------------------------------{NewLine}";

    public const string Console =
@"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}";
}

