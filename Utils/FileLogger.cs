using System;
using System.IO;

namespace ExtrasensoryPerception.Utils;

/// <summary>
/// Saves debug logs to a file in Documents folder.
/// File: Documents/vrisingesp_{dd-MM-yyyy_HH-mm-ss}.txt
/// </summary>
public static class FileLogger
{
    private static StreamWriter? _writer;
    private static string _filePath = "";
    private static readonly object _lock = new();

    public static bool IsActive => _writer != null;
    public static string FilePath => _filePath;

    public static void Start()
    {
        if (_writer != null) return;

        try
        {
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
            _filePath = Path.Combine(docs, $"vrisingesp_{timestamp}.txt");

            _writer = new StreamWriter(_filePath, append: true) { AutoFlush = true };
            _writer.WriteLine($"=== VRisingESP Debug Log - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
            _writer.WriteLine();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[FileLogger] Erro ao criar arquivo: {ex.Message}");
            _writer = null;
        }
    }

    public static void Stop()
    {
        lock (_lock)
        {
            if (_writer == null) return;
            try
            {
                _writer.WriteLine();
                _writer.WriteLine($"=== Log encerrado - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
                _writer.Flush();
                _writer.Close();
            }
            catch { }
            _writer = null;
            _filePath = "";
        }
    }

    public static void Log(string message)
    {
        if (_writer == null) return;
        lock (_lock)
        {
            try
            {
                _writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
            catch { }
        }
    }
}
