using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using UnityEngine;

using Dropship.Utilities;

namespace Dropship;

public static class Logger<T> where T : BasePlugin
{
    private static ManualLogSource instance => PluginSingleton<T>.Instance.Log;

    public static void Error(object logData)
    {
        instance.LogError(logData);
    }

    public static void Debug(object logData)
    {
        instance.LogDebug(logData);
    }

    public static void Info(object logData)
    {
        instance.LogInfo(logData);
    }

    public static void Message(object logData)
    {
        instance.LogMessage(logData);
    }

    public static void Warning(object logData)
    {
        instance.LogWarning(logData);
    }

    internal static void BackupCurrentLog()
    {
        string logBackupPath = getLogBackupPath();

        if (Directory.Exists(logBackupPath))
        {
            string[] logFile = Directory
                //logのバックアップディレクトリ内の全ファイルを取得
                .GetFiles(logBackupPath)
                //.logだけサーチ
                .Where(filePath => Path.GetExtension(filePath) == ".log")
                //日付順に降順でソート
                .OrderBy(filePath => File.GetLastWriteTime(filePath).Date)
                //同じ日付内で時刻順に降順でソート
                .ThenBy(filePath => File.GetLastWriteTime(filePath).TimeOfDay)
                .ToArray();

            if (logFile.Length >= 10)
            {
                File.Delete(logFile[0]);
            }
        }
        else
        {
            Directory.CreateDirectory(logBackupPath);
        }

        string movedLog = string.Concat(
            logBackupPath, @$"\BackupLog {getTimeStmp()}.log");

        File.Copy(getLogPath(), movedLog);
        // replaceLogCustomServerIp(movedLog);
    }

    internal static void Dump()
    {

        string dumpFilePath = string.Concat(
            Environment.GetFolderPath(
                Environment.SpecialFolder.DesktopDirectory), @"\",
            $"DumpedLogs {getTimeStmp()}.zip");

        string tmpLogFile = string.Concat(
            Path.GetDirectoryName(Application.dataPath), @"\BepInEx/tmp.log");

        File.Copy(getLogPath(), tmpLogFile, true);
        // replaceLogCustomServerIp(tmpLogFile);

        using (var dumpedZipFile = ZipFile.Open(
            dumpFilePath, ZipArchiveMode.Update))
        {
            dumpedZipFile.CreateEntryFromFile(
                tmpLogFile, $"DumpedLog {getTimeStmp()}.log");

            string logBackupPath = getLogBackupPath();

            if (Directory.Exists(logBackupPath))
            {
                dumpedZipFile.CreateEntry("BackupLog/");

                string[] logFile = Directory
                    //logのバックアップディレクトリ内の全ファイルを取得
                    .GetFiles(logBackupPath)
                    //.logだけサーチ
                    .Where(filePath => Path.GetExtension(filePath) == ".log")
                    .ToArray();

                foreach (string logPath in logFile)
                {
                    dumpedZipFile.CreateEntryFromFile(
                        logPath, $"BackupLog/{Path.GetFileName(logPath)}");
                }
            }
        }

        File.Delete(tmpLogFile);

        System.Diagnostics.Process.Start(
            "EXPLORER.EXE", $@"/select, ""{dumpFilePath}""");
    }

    private static string getTimeStmp() => DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");

    private static string getLogPath() => string.Concat(
        Path.GetDirectoryName(Application.dataPath), @"\BepInEx/LogOutput.log");

    private static string getLogBackupPath() => string.Concat(
        Path.GetDirectoryName(Application.dataPath), @"\BepInEx/BackupLog");
}
