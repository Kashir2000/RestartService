using Airiti.Common;
using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace RestartService
{
  public class Program
  {
    /// <summary>
    /// 延遲時間
    /// </summary>
    private static readonly int DelayTime = int.TryParse(ConfigurationManager.AppSettings["DelayTime"], out int Value) && Value > 0 ? Value * 1000 : 20000;

    static void Main(string[] args)
    {
      Airiti.Configuration.Debug = (ConfigurationManager.AppSettings["Debug"] ?? string.Empty).ToLower() == "true";
      Airiti.Configuration.DebugPath = ConfigurationManager.AppSettings["DebugPath"] ?? string.Empty;
      if (args.Length > 0)
      {
        string ServiceName = args[0];
        try
        {
          ServiceController service = new ServiceController(ServiceName);

          Debug.WriteLine($"正在嘗試啟動服務：{ServiceName}");
          Thread.Sleep(DelayTime);
          service.Start();
          service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
          Debug.WriteLine($"{ServiceName} 服務已經啟動。");
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"無法啟動服務 {ServiceName}。錯誤：{ex.Message}");
          ErrorLog(ex);
        }
      }
      else
      {
        Debug.WriteLine("請提供服務名稱作為參數。");
      }
    }

    /// <summary>
    /// 寫入錯誤Log
    /// </summary>
    /// <param name="ex"></param>
    private static void ErrorLog(Exception ex)
    {
      LogFile objLogFile = new LogFile("RestartService", "Main");
      objLogFile.StartLog();
      objLogFile.Log(ex);
      objLogFile.EndLog();
    }
  }
}