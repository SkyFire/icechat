using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace IceChatUpdater
{
    public partial class FormUpdater : Form
    {
        private string currentFolder;
        private int currentFile;

        public FormUpdater(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0)
            {
                foreach (string arg in args)
                    currentFolder = arg;
            }
            else
                currentFolder = Application.StartupPath;

            currentFolder += System.IO.Path.DirectorySeparatorChar + "Update";
            if (!Directory.Exists(currentFolder))
                Directory.CreateDirectory(currentFolder);

            labelFolder.Text = currentFolder;

            CheckForUpdate();

        }

        private void CheckForUpdate()
        {

            //get the current version of IceChat 2009 in the Same Folder
            System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IceChat2009.exe");
            System.Diagnostics.Debug.WriteLine(fv.FileVersion);
            labelCurrent.Text = "Current Version: " + fv.FileVersion;
            double currentVersion = Convert.ToDouble(fv.FileVersion.Replace(".", String.Empty));
            
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.DownloadFile("http://www.icechat.net/update.xml", currentFolder + System.IO.Path.DirectorySeparatorChar + "update.xml");
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(currentFolder + System.IO.Path.DirectorySeparatorChar + "update.xml");
            
            System.Xml.XmlNodeList version = xmlDoc.GetElementsByTagName("version");
            System.Xml.XmlNodeList versiontext = xmlDoc.GetElementsByTagName("versiontext");

            labelLatest.Text = "Latest Version: " + versiontext[0].InnerText;

            if (Convert.ToDouble(version[0].InnerText) > currentVersion)
            {
                XmlNodeList files = xmlDoc.GetElementsByTagName("file");
                foreach (XmlNode node in files)
                {
                    listFiles.Items.Add(node.InnerText);
                }

                buttonDownload.Visible = true;
                labelUpdate.Visible = true;
            }
            else
                labelNoUpdate.Visible = true;
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            //download the files in the File List box
            System.Net.WebClient webClient = new System.Net.WebClient();
            
            //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);            
            //webClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            foreach (string file in listFiles.Items)
            {
                string f = System.IO.Path.GetFileName(file);
                System.Diagnostics.Debug.WriteLine(f);
                //webClient.DownloadFileAsync(new Uri(file), currentFolder + System.IO.Path.DirectorySeparatorChar + f);
                webClient.DownloadFile(file, currentFolder + System.IO.Path.DirectorySeparatorChar + f);                    
                
            }
            MessageBox.Show("Completed Download");
        }

        private void webClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            System.Diagnostics.Debug.WriteLine(e.ProgressPercentage + ":" +e.BytesReceived);
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            System.Diagnostics.Debug.WriteLine("download done:" + e.UserState);
            //go to the next file in the list
            currentFile++;

        }
    }
}
