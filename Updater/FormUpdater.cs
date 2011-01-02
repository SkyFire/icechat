﻿/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2010 Paul Vanderzee <snerf@icechat.net>
 *                                    <www.icechat.net> 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * Please consult the LICENSE.txt file included with this project for
 * more details
 *
\******************************************************************************/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;

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
            
            //delete the current update.xml file if it exists
            if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + "update.xml"))
                File.Delete(currentFolder + System.IO.Path.DirectorySeparatorChar + "update.xml");

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
            this.Cursor = Cursors.WaitCursor;
            
            System.Collections.ArrayList localFiles = new System.Collections.ArrayList();


            //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);            
            //webClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            foreach (string file in listFiles.Items)
            {
                string f = System.IO.Path.GetFileName(file);
                System.Diagnostics.Debug.WriteLine(f);
                //webClient.DownloadFileAsync(new Uri(file), currentFolder + System.IO.Path.DirectorySeparatorChar + f);

                if (File.Exists(currentFolder + System.IO.Path.DirectorySeparatorChar + f))
                    File.Delete(currentFolder + System.IO.Path.DirectorySeparatorChar + f);

                localFiles.Add(f);
                webClient.DownloadFile(file, currentFolder + System.IO.Path.DirectorySeparatorChar + f);                    
                    
            }
            
            this.Cursor = Cursors.Default;
            MessageBox.Show("Completed Download");

            //now see if IceChat is running
            //and close it
            
            buttonDownload.Enabled = false;

            Process[] pArry = Process.GetProcesses();

            foreach (Process p in pArry)
            {
                string s = p.ProcessName;
                s = s.ToLower();

                if (s.CompareTo("icechat2009") == 0)
                {
                    if (Path.GetDirectoryName(p.Modules[0].FileName).ToLower() == Application.StartupPath.ToLower())
                    {
                        MessageBox.Show("Closing IceChat to update it");
                        try
                        {
                            p.Kill();
                            //p.CloseMainWindow();

                            //wait a bit and then copy the files to this folder, and VOILA
                            p.WaitForExit();
                            
                            System.Threading.Thread.Sleep(3000);
                            
                            foreach (string f in localFiles)
                            {
                                if (File.Exists(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + f))
                                    File.Delete(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + f);

                                System.Threading.Thread.Sleep(500);

                                //MessageBox.Show(currentFolder + System.IO.Path.DirectorySeparatorChar + f + ":" + Application.StartupPath + System.IO.Path.DirectorySeparatorChar + f);
                                
                                File.Copy(currentFolder + System.IO.Path.DirectorySeparatorChar + f, Application.StartupPath + System.IO.Path.DirectorySeparatorChar + f);

                                //delete the files out of the update folder
                                File.Delete(currentFolder + System.IO.Path.DirectorySeparatorChar + f);
                            }



                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show(ee.Message + ":" + ee.Source);
                        }
                        
                        
                        MessageBox.Show("Files Updated, you are welcome to Restart IceChat");

                    }
                }
            }

        }

        private void webClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
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