using System;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", ConfigFileExtension = "config", Watch = true)]
public class Logger
{
    private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
    private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

    public static void WriteInfo(string info)
    {
        //Logger.WriteInfo(info);
        if (loginfo.IsInfoEnabled)
        {
            loginfo.Info(info);
        }
    }

    public static void WriteError(string error)
    {
        //Logger.WriteInfo(error);
        if (logerror.IsErrorEnabled)
        {
            logerror.Error(error);
        }
    }

    public static void WriteError(string info, Exception ex)
    {
        //Logger.WriteInfo(info);
        if (logerror.IsErrorEnabled)
        {
            logerror.Error(info, ex);
        }
    }
}