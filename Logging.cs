using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IceChat
{
    public class Logging : IDisposable
    {
        //a logging class to handle window logging        
        private System.IO.FileStream logFile;
        private int lastDayWritten;
        private ConsoleTab _consoleTab = null;
        private IceTabPage _tabPage = null;


        public Logging(ConsoleTab consoleTab)
        {
            this._consoleTab = consoleTab;

            CreateConsoleLog();
        }

        private void CreateConsoleLog()
        {
            string logFolder = FormMain.Instance.LogsFolder;

            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            logFolder += Path.DirectorySeparatorChar + _consoleTab.Connection.ServerSetting.ServerName;

            string date = "-" + System.DateTime.Now.ToString("yyyy-MM-dd");

            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            if (FormMain.Instance.IceChatOptions.SeperateLogs)
                logFile = new FileStream(logFolder + Path.DirectorySeparatorChar + "Console" + date + ".log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
            else
                logFile = new FileStream(logFolder + Path.DirectorySeparatorChar + "Console.log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);

            lastDayWritten = DateTime.Now.Day;

            if (logFile.Length == 0)
                AddFileHeader();

        }

        public Logging(IceTabPage tabPage)
        {
            this._tabPage = tabPage;

            CreateStandardLog();            
        }

        private void CreateStandardLog()
        {
            string logFolder = FormMain.Instance.LogsFolder;

            string date = "-" + System.DateTime.Now.ToString("yyyy-MM-dd");

            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            //set the log folder to the server name
            if (_tabPage.WindowStyle != IceTabPage.WindowType.Debug)
                logFolder += Path.DirectorySeparatorChar + _tabPage.Connection.ServerSetting.ServerName;

            if (_tabPage.WindowStyle == IceTabPage.WindowType.Channel)
            {
                logFolder += Path.DirectorySeparatorChar + "Channel";
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                if (FormMain.Instance.IceChatOptions.SeperateLogs)
                    logFile = new FileStream(logFolder + Path.DirectorySeparatorChar + _tabPage.TabCaption + date + ".log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                else
                    logFile = new FileStream(logFolder + Path.DirectorySeparatorChar + _tabPage.TabCaption + ".log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
            }
            else if (_tabPage.WindowStyle == IceTabPage.WindowType.Query)
            {
                logFolder += Path.DirectorySeparatorChar + "Query";
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                if (FormMain.Instance.IceChatOptions.SeperateLogs)
                    logFile = new FileStream(logFolder + Path.DirectorySeparatorChar + _tabPage.TabCaption + date + ".log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                else
                    logFile = new FileStream(logFolder + Path.DirectorySeparatorChar + _tabPage.TabCaption + ".log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
            }
            else if (_tabPage.WindowStyle == IceTabPage.WindowType.Debug)
            {
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                logFile = new FileStream(FormMain.Instance.LogsFolder + Path.DirectorySeparatorChar + "debug.log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
            }

            lastDayWritten = DateTime.Now.Day;

            if (logFile.Length == 0)
                AddFileHeader();

        }

        internal void WriteLogFile(string message)
        {
            if (logFile != null)
            {
                //check if we need to make a new log file for a new day
                if (DateTime.Now.Day != lastDayWritten && FormMain.Instance.IceChatOptions.SeperateLogs)
                {
                    logFile.Close();
                    logFile.Dispose();

                    if (_consoleTab != null)
                        CreateConsoleLog();
                    else if (_tabPage != null)
                        CreateStandardLog();
                }
                logFile.Write(System.Text.Encoding.Default.GetBytes(message + "\r\n"), 0, message.Length + 2);
                logFile.Flush();
            }
        }

        internal void AddFileHeader()
        {
            if (_tabPage != null)
            {
                WriteLogFile("Session Start: " + DateTime.Now.ToString("ddd mmm dd hh:mm:ss yyyy"));
                if (_tabPage.WindowStyle == IceTabPage.WindowType.Channel)
                    WriteLogFile("Session Ident: " + _tabPage.TabCaption);
            }
            else if (_consoleTab != null)
            {
                WriteLogFile("Session Start: " + DateTime.Now.ToString("ddd mmm dd hh:mm:ss yyyy"));
            }
        }

        public void Dispose()
        {
            if (_tabPage != null)
                System.Diagnostics.Debug.WriteLine("Dispose Logging Class TabPage:" + _tabPage.TabCaption);
            else if (_consoleTab != null)
                System.Diagnostics.Debug.WriteLine("Dispose Logging Class Console:" + _consoleTab.Connection.ServerSetting.ServerName);
            
            if (logFile != null)
            {
                try
                {
                    logFile.Flush();
                    logFile.Close();
                    logFile.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}
