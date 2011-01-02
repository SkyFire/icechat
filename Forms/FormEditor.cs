/******************************************************************************\
 * IceChat 2009 Internet Relay Chat Client
 *
 * Copyright (C) 2011 Paul Vanderzee <snerf@icechat.net>
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace IceChat
{
    public partial class FormEditor : Form
    {
        private IceChatAliases aliasList;
        private IceChatPopupMenus popupList;

        private string[] nickListPopup;
        private string[] consolePopup;
        private string[] channelPopup;
        private string[] queryPopup;

        private string currentPopup;
        private string currentScript;
        private ToolStripMenuItem currentPopupMenu;

        //private IceChatScript.IceChatScript icechatScript;
        
        public FormEditor()
        {            
            InitializeComponent();
            popupTypeToolStripMenuItem.Visible = false;

            tabControlEditor.SelectedIndexChanged += new EventHandler(tabControlEditor_SelectedIndexChanged);
            textAliases.KeyDown += new KeyEventHandler(OnKeyDown);
            textPopups.KeyDown += new KeyEventHandler(OnKeyDown);
            textScripts.KeyDown += new KeyEventHandler(OnKeyDown);

            //load the aliases
            aliasList = FormMain.Instance.IceChatAliases;
            LoadAliases();

            //load the popups
            popupList = FormMain.Instance.IceChatPopupMenus;
            nickListPopup = LoadPopupMenu("NickList");
            consolePopup = LoadPopupMenu("Console");
            channelPopup = LoadPopupMenu("Channel");
            queryPopup = LoadPopupMenu("Query");

            //load the nicklist by default into popup editor
            LoadPopups(nickListPopup);
            nickListToolStripMenuItem.Checked = true;
            currentPopup = "NickList";
            currentPopupMenu = nickListToolStripMenuItem;

            //change the menu bat font
            menuStripMain.Font = new Font(FormMain.Instance.IceChatFonts.FontSettings[7].FontName, FormMain.Instance.IceChatFonts.FontSettings[7].FontSize);

            try
            {
                if (FormMain.Instance.IceChatOptions.ScriptFiles != null)
                {
                    foreach (string script in FormMain.Instance.IceChatOptions.ScriptFiles)
                    {
                        ToolStripMenuItem t = new ToolStripMenuItem(System.IO.Path.GetFileName(script));
                        t.Tag = script;
                        scriptsToolStripMenuItem.DropDownItems.Add(t);
                    }
                    if (scriptsToolStripMenuItem.DropDownItems.Count > 0)
                    {
                        StreamReader stream = new StreamReader(scriptsToolStripMenuItem.DropDownItems[0].Tag.ToString());
                        textScripts.Text = stream.ReadToEnd();
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                        ((ToolStripMenuItem)scriptsToolStripMenuItem.DropDownItems[0]).Checked = true;
                    }
                }
            }
            catch(FileNotFoundException fe)
            {
                FormMain.Instance.WriteErrorFile("FormEditor LoadScripts", fe);
            }
            
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.K)
                {
                    ((TextBox)sender).SelectedText = ((char)3).ToString();
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.B)
                {
                    ((TextBox)sender).SelectedText = ((char)2).ToString();
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.U)
                {
                    ((TextBox)sender).SelectedText = ((char)31).ToString();
                    e.Handled = true;
                }
            }
        }


        private void tabControlEditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlEditor.SelectedTab.Text != "PopupMenus")
            {
                popupTypeToolStripMenuItem.Visible = false;
            }
            else
                popupTypeToolStripMenuItem.Visible = true;

        }

        private void ReLoadAliases()
        {
            if (File.Exists(FormMain.Instance.AliasesFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatAliases));
                TextReader textReader = new StreamReader(FormMain.Instance.AliasesFile);
                aliasList = (IceChatAliases)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
            {
                aliasList = new IceChatAliases();
            }
        }

        private void LoadAliases()
        {
            textAliases.Clear();
            
            //reload the aliases from the actual file
            ReLoadAliases();


            foreach (AliasItem alias in aliasList.listAliases)
            {
                if (alias.Command.Length == 1)
                    textAliases.AppendText(alias.AliasName + " " + alias.Command[0].Replace("&#x3;", ((char)3).ToString()).Replace("&#x2;", ((char)2).ToString()) + Environment.NewLine);
                else
                {
                    //multiline alias
                    textAliases.AppendText(alias.AliasName + " {" + Environment.NewLine);
                    foreach (string command in alias.Command)
                    {
                        textAliases.AppendText(command + Environment.NewLine);
                    }
                    textAliases.AppendText("}" + Environment.NewLine);
                }
            }
        }

        private void LoadPopups(string[] menu)
        {
            textPopups.Clear();
            
            if (menu == null) return;
            
            foreach (string m in menu)
            {
                textPopups.AppendText(m + Environment.NewLine);
            }

            textPopups.SelectionStart = 0;
            textPopups.SelectionLength = 0;

        }

        private void ReLoadPopups()
        {
            if (File.Exists(FormMain.Instance.PopupsFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(IceChatPopupMenus));
                TextReader textReader = new StreamReader(FormMain.Instance.PopupsFile);
                popupList = (IceChatPopupMenus)deserializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
            }
            else
                popupList = new IceChatPopupMenus();

        }

        private string[] LoadPopupMenu(string popupType)
        {
            //reload the popupmenu's file
            ReLoadPopups();

            foreach (PopupMenuItem p in popupList.listPopups)
            {
                if (p.PopupType == popupType)
                    return p.Menu;
            }
            return null;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //save all the settings

            //parse out all the aliases
            textAliases.Text = textAliases.Text.Replace(((char)3).ToString(), "&#x3;").Replace(((char)2).ToString(), "&#x2;");
            
            aliasList.listAliases.Clear();
            
            string[] aliases = textAliases.Text.Trim().Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
            bool isMultiLine = false;
            AliasItem multiLineAlias = null;
            string aliasCommands = "";

            foreach (string alias in aliases)
            {
                if (alias.Length > 0)
                {
                    //check if it is a multilined alias
                    if (alias.EndsWith("{") && !isMultiLine)
                    {
                        //start of a multilined alias
                        isMultiLine = true;
                        multiLineAlias = new AliasItem();
                        multiLineAlias.AliasName = alias.Substring(0, alias.IndexOf(' '));
                        aliasCommands = "";
                    }
                    else if (alias == "}")
                    {
                        //end of multiline alias
                        isMultiLine = false;
                        multiLineAlias.Command = aliasCommands.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        aliasList.AddAlias(multiLineAlias);
                        multiLineAlias = null;
                    }
                    else if (!isMultiLine)
                    {
                        //just a normal alias
                        AliasItem a = new AliasItem();
                        a.AliasName = alias.Substring(0, alias.IndexOf(' '));
                        a.Command = new String[] { alias.Substring(alias.IndexOf(' ') + 1) };
                        aliasList.AddAlias(a);
                        a = null;
                    }
                    else
                    {
                        //add a line to the multiline alias
                        aliasCommands += alias + "\r\n";
                    }
                }
            }
            
            FormMain.Instance.IceChatAliases = aliasList;

            //save the current popup menu
            UpdateCurrentPopupMenus();
                       
            this.Close();
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            System.CodeDom.Compiler.CodeDomProvider cp = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");            
            string[] referenceAssemblies = { "System.dll", "System.Windows.Forms.dll" };
            
            System.CodeDom.Compiler.CompilerParameters par = new System.CodeDom.Compiler.CompilerParameters(referenceAssemblies);            
            par.ReferencedAssemblies.Add(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IceChatScript.dll");
            //par.ReferencedAssemblies.Add(icechatScript.GetType().Assembly.Location);
            par.GenerateExecutable = false;
            par.GenerateInMemory = true;
            par.CompilerOptions = "/target:library";
            par.IncludeDebugInformation = true;
            par.TreatWarningsAsErrors = true;
            
            //par.OutputAssembly = FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "test_script.dll";
            par.MainClass = "IceChat.Script";

            System.Diagnostics.Debug.WriteLine(par.ReferencedAssemblies.Count);

            System.CodeDom.Compiler.CompilerResults err = cp.CompileAssemblyFromSource(par, textScripts.Text);
            if (err.Errors.Count > 0)
            {
                foreach (System.CodeDom.Compiler.CompilerError ce in err.Errors)
                {
                    System.Diagnostics.Debug.WriteLine("Error:" + ce.ToString());
                    FormMain.Instance.WindowMessage(null, "Console", "Error:" + ce.ErrorNumber + ":" + ce.ToString(), 4, true);
                }
                return;
            }

            //System.Reflection.Module module = err.CompiledAssembly.GetModules()[0];
            //Type mt = null;
            //System.Reflection.MethodInfo mi = null;
            //System.Reflection.Assembly a =             

            //AppDomain domain = AppDomain.CreateDomain("IceChat Script");
            //Assembly a = err.CompiledAssembly;
            //Assembly a = Assembly.LoadFrom(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IceChatScript.dll");

            //Assembly assem = Assembly.GetExecutingAssembly();
            //Type thisForm = assem.GetType("FormEditor");

            //Object o = Activator.CreateInstance(thisForm);
            
            object o = err.CompiledAssembly.CreateInstance("IceChat.Script");            
            //Type test = err.CompiledAssembly.GetType("IceChat.Script");
            //Assembly a = err.CompiledAssembly();

            Type test = err.CompiledAssembly.GetType("IceChat.Script");
            //Type test = err.CompiledAssembly.GetType("IceChatScript.IceChatScript");
            
            
            //methodinfo will error if OnText is not found
            MethodInfo[] m = test.GetMethods();
            System.Diagnostics.Debug.WriteLine("show methods");
            foreach (MethodInfo i in m)
            {
                ParameterInfo[] pif = i.GetParameters();
                System.Diagnostics.Debug.WriteLine(i.Name + ":" + pif.Length + ":" + i.DeclaringType);
            }
            System.Diagnostics.Debug.WriteLine("show events:" + test.GetEvents().Length);
            EventInfo[] ee = test.GetEvents();
            foreach (EventInfo eee in ee)
            {
                System.Diagnostics.Debug.WriteLine("event name " + eee.Name + ":" + eee.DeclaringType);
            }

            EventInfo evt = test.GetEvent("OutGoingCommand");
            if (evt != null)
            {
                
                
                //MethodInfo h = typeof(IceChatScript.IceChatScript).GetMethod("SendCommand", BindingFlags.Public | BindingFlags.Instance);
                Type iceType = Type.GetType("IceChatScript.IceChatScript");
                //Type iceType =  icechatScript.GetType();
                //Type iceType = Type.GetType("IceChat.FormEditor");
                //System.Diagnostics.Debug.WriteLine(evt.EventHandlerType);
                //MethodInfo mi = typeof(FormEditor).GetMethod("SendCommand", BindingFlags.Public | BindingFlags.Instance);
                //MethodInfo mi = typeof(IceChatScript.IceChatScript).GetMethod("SendCommand", BindingFlags.Public | BindingFlags.Instance);
                /*
                if (mi != null)
                {
                    System.Diagnostics.Debug.WriteLine("mi is not null:" + mi.DeclaringType);
                }
                 */
                //IceChatScript.IceChatScript ice = new IceChatScript.IceChatScript();
                //ice.OutGoingCommand +=new IceChatScript.IceChatScript.OutGoingCommandDelegate(OutGoingCommand);

                MethodInfo mi = this.GetType().GetMethod("SendCommand", BindingFlags.Public | BindingFlags.Instance);
                if (mi != null)
                {
                    System.Diagnostics.Debug.WriteLine("mi is not null:" + mi.DeclaringType);
                }
                
                Delegate del = Delegate.CreateDelegate(evt.EventHandlerType, this, mi);
                if (del != null)
                {
                    System.Diagnostics.Debug.WriteLine("del is not null");
                }
                
                
                //Delegate del = Delegate.CreateDelegate(evt.EventHandlerType, null, "OutGoingCommand");
                //MethodInfo addHandler = evt.GetAddMethod();
                //object[] addHandlerArgs = { del };
                //addHandler.Invoke(this, addHandlerArgs);
                
                //Delegate del = Delegate.CreateDelegate(evt.EventHandlerType, this, "OutGoingCommand");
                evt.AddEventHandler(o, del);
                
                MethodInfo m2 = test.GetMethod("OnText");
                if (m2 != null)
                {
                    System.Diagnostics.Debug.WriteLine("fire ontext");
                    m2.Invoke(this, new object[] { "this is a test message", "#icechat2009", "Snerf", "icechat@icechat.net", new object() });
                }

                /*
                MethodInfo h = typeof(IceChatScript.IceChatScript).GetMethod("SendCommand", BindingFlags.Public | BindingFlags.Instance);
                if (h != null)
                {
                    System.Diagnostics.Debug.WriteLine("not null");
                    System.Diagnostics.Debug.WriteLine(h.DeclaringType + ":" + h.GetParameters().Length + ":" + h.IsPrivate + ":" + h.IsPublic + ":" + h.IsStatic);
                    //isPublic is true

                    foreach (ParameterInfo pi in h.GetParameters())
                    {
                        System.Diagnostics.Debug.WriteLine("params:" + pi.Name + ":" + pi.ParameterType + ":");
                    }

                    Delegate del = Delegate.CreateDelegate(evt.EventHandlerType, this, "SendCommand");

                    evt.AddEventHandler(o, del);

                    MethodInfo mi = test.GetMethod("OnText");
                    if (mi != null)
                    {
                        System.Diagnostics.Debug.WriteLine("fire ontext");
                        mi.Invoke(o, new object[] { "this is a test message", "#icechat2009", "Snerf", "icechat@icechat.net", new object() });
                    }

                }
                else
                    System.Diagnostics.Debug.WriteLine("handler null");

                if (handler != null)
                {
                    System.Diagnostics.Debug.WriteLine(handler.DeclaringType + ":" + handler.GetParameters().Length);                    
                    foreach (ParameterInfo pi in handler.GetParameters())
                    {
                        System.Diagnostics.Debug.WriteLine("params:" + pi.Name + ":" + pi.ParameterType + ":");
                    }
                    Type tDel = evt.EventHandlerType;
                    Delegate del = Delegate.CreateDelegate( tDel , this, handler);
                    
                    
                    //Delegate del = Delegate.CreateDelegate(evt.EventHandlerType,this, handler);
                    
                    evt.AddEventHandler(this, del);

                    MethodInfo mi = test.GetMethod("OnText");
                    if (mi != null)
                    {
                        System.Diagnostics.Debug.WriteLine("fire ontext");
                        mi.Invoke(o, new object[] { "this is a test message", "#icechat2009", "Snerf", "icechat@icechat.net", new object() });
                    }
                    
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("handler null:" + evt.Name);
                    
                    
                    
                    MethodInfo[] mis = this.GetType().GetMethods();
                    foreach (MethodInfo mi3 in mis)
                    {
                        System.Diagnostics.Debug.WriteLine(mi3.Name);
                    }
                    
                }
                */
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OutGoingCommand Event is null");

            }
            /*
            MethodInfo info = o.GetType().GetMethod("OnText");
            if (info != null)
            {
                System.Diagnostics.Debug.WriteLine("run ontext");
                info.Invoke(o, new object[] { "this is a test message", "#icechat2009", "Snerf", "icechat@icechat.net", new object() });
            }
            */
            /*
            if (module != null)
                mt = module.GetType("IceChatScript.Test");
            else
                System.Diagnostics.Debug.WriteLine("fail module");

            if (mt != null)
                mi = mt.GetMethod("OnTest");
            else
                System.Diagnostics.Debug.WriteLine("fail methodinfo OnTest");

            if (mi != null)
            {
                System.Diagnostics.Debug.WriteLine("this far");
                
                mi.Invoke(mt, new object[] { "this is a test message","#icechat2009","Snerf","icechat@icechat.net", new object() });
            }
            else
                System.Diagnostics.Debug.WriteLine("Invoke Failed");
            */


            /*
            if (mt != null)
                mi = mt.GetMethod("Main");
            else
                System.Diagnostics.Debug.WriteLine("fail methodinfo");

            if (mi != null)
            {
                System.Diagnostics.Debug.WriteLine("this far");
                mi.Invoke(null, new object[] { });
            }
            else
                System.Diagnostics.Debug.WriteLine("Invoke Failed");
            */
        }

        void OutGoingCommand(string command, object connection)
        {
            System.Diagnostics.Debug.WriteLine("out:" + command);            
        }

        public void OutGoingCommand3(string command, object connection)
        {
            System.Diagnostics.Debug.WriteLine("out:" + command);
        }

        public void OutGoingCommand2(string command, object connection)
        {
            System.Diagnostics.Debug.WriteLine("out:" + command);
        }

        private void SendCommand1(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("send command args:");
        }
        
        public void SendCommand(string command, object connection)
        {
            System.Diagnostics.Debug.WriteLine("send command:" + command);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            textScripts.AppendText("using System.Windows.Forms;" + Environment.NewLine);
            textScripts.AppendText("using System;" + Environment.NewLine);
            textScripts.AppendText("namespace IceChat" + Environment.NewLine);
            textScripts.AppendText("{" + Environment.NewLine);
            textScripts.AppendText("  public class Script : IceChatScript.IceChatScript" + Environment.NewLine);
            textScripts.AppendText("  {" + Environment.NewLine);
            textScripts.AppendText("    static IceChatScript.IceChatScript ice = new IceChatScript.IceChatScript();" + Environment.NewLine);
            textScripts.AppendText("    static public void Main()" + Environment.NewLine);
            textScripts.AppendText("    {" + Environment.NewLine);
            textScripts.AppendText("      MessageBox.Show(\"testing\");" + Environment.NewLine);
            textScripts.AppendText("    }" + Environment.NewLine);
            textScripts.AppendText("    static public void OnText(string Message, string Channel, string Nick, string Host, object Server)" + Environment.NewLine);
            textScripts.AppendText("    {" + Environment.NewLine);
            //textScripts.AppendText("      MessageBox.Show(\"ONTEXT:\" + Message);" + Environment.NewLine);
            textScripts.AppendText("      ice.SendCommand(Message, Server);" + Environment.NewLine);
            //textScripts.AppendText("      SendCommand(Message, Server);" + Environment.NewLine);
            textScripts.AppendText("    }" + Environment.NewLine);

            textScripts.AppendText("   }" + Environment.NewLine);
            textScripts.AppendText("}");
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //save what is in the current
            if (tabControlEditor.SelectedTab.Text == "Scripts")
            {
                if (currentScript != null && currentScript.Length > 0)
                {
                    StreamWriter stream = new StreamWriter(currentScript);
                    stream.WriteLine(textScripts.Text);
                    stream.Flush();
                    stream.Close();

                    MessageBox.Show(currentScript + " saved");
                }
                else
                {
                    //ask for a file name
                    FileDialog fd = new SaveFileDialog();
                    fd.DefaultExt = ".cs";
                    fd.Filter = "Script file (*.cs)|*.cs";
                    fd.AddExtension = true;
                    fd.AutoUpgradeEnabled = true;
                    fd.Title = "Where do you want to save the file?";
                    fd.InitialDirectory = FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "Scripts";
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        StreamWriter stream = new StreamWriter(fd.FileName);
                        stream.WriteLine(textScripts.Text);
                        stream.Flush();
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }
                    
                    fd.Dispose();
                    fd = null;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //save what is in the current
            if (tabControlEditor.SelectedTab.Text == "Scripts")
            {
                FileDialog fd = new SaveFileDialog();
                fd.DefaultExt = ".cs";
                fd.Filter = "Script file (*.cs)|*.cs";
                fd.AddExtension = true;
                fd.AutoUpgradeEnabled = true;
                fd.Title = "Where do you want to save the file?";
                fd.InitialDirectory = FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "Scripts";
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter stream = new StreamWriter(fd.FileName);
                    stream.WriteLine(textScripts.Text);
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }

                fd.Dispose();
                fd = null;
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControlEditor.SelectedTab.Text == "Scripts")
            {
                FileDialog fd = new OpenFileDialog();
                fd.DefaultExt = ".cs";
                fd.CheckFileExists = true;
                fd.CheckPathExists = true;
                fd.AddExtension = true;
                fd.AutoUpgradeEnabled = true;
                fd.Filter = "Script file (*.cs)|*.cs";
                fd.Title = "Which file do you want to open?";
                fd.InitialDirectory = FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "Scripts";
                
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    currentScript = fd.FileName;
                    StreamReader stream = new StreamReader(fd.FileName);
                    textScripts.Text = stream.ReadToEnd();
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                    
                    ToolStripMenuItem t = new ToolStripMenuItem(System.IO.Path.GetFileName(fd.FileName));
                    t.Tag = fd.FileName;
                    scriptsToolStripMenuItem.DropDownItems.Add(t);
                    FormMain.Instance.IceChatOptions.ScriptFiles = new string[scriptsToolStripMenuItem.DropDownItems.Count];
                    for (int i = 0; i < scriptsToolStripMenuItem.DropDownItems.Count; i++)
                        FormMain.Instance.IceChatOptions.ScriptFiles[i] = scriptsToolStripMenuItem.DropDownItems[i].Tag.ToString();

                }

                fd.Dispose();
                fd = null;

            
            
            }
        }

        public void icechatScript2_OutGoingCommand(string command, object connection)
        {
            System.Diagnostics.Debug.WriteLine("outgoing:" + command);
        }


        private void testSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
			//
        }
		
        private void UpdateCurrentPopupMenus()
        {
            string[] popups = textPopups.Text.Trim().Split(new String[] { "\r\n" }, StringSplitOptions.None);
         
            if (currentPopup == "NickList")
                nickListPopup = popups;
            if (currentPopup == "Console")
                consolePopup = popups;
            if (currentPopup == "Channel")
                channelPopup = popups;
            if (currentPopup == "Query")
                queryPopup = popups;


            PopupMenuItem p = new PopupMenuItem();
            p.PopupType = currentPopup;
            p.Menu = popups;
            popupList.ReplacePopup(p.PopupType, p);

            FormMain.Instance.IceChatPopupMenus = popupList;

            currentPopupMenu.Checked = false;

        }
        private void nickListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //save the current popup
            if (currentPopup == "NickList") return;

            UpdateCurrentPopupMenus();
            
            currentPopupMenu = nickListToolStripMenuItem;
            currentPopup = "NickList";
            currentPopupMenu.Checked = true;
            LoadPopups(nickListPopup);
        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPopup == "Console") return;

            UpdateCurrentPopupMenus();

            currentPopupMenu = consoleToolStripMenuItem;
            currentPopup = "Console";
            currentPopupMenu.Checked = true;
            LoadPopups(consolePopup);
        }

        private void channelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPopup == "Channel") return;

            UpdateCurrentPopupMenus();

            currentPopupMenu = channelToolStripMenuItem;
            currentPopup = "Channel";
            currentPopupMenu.Checked = true;
            LoadPopups(channelPopup);
        }

        private void queryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPopup == "Query") return;

            UpdateCurrentPopupMenus();

            currentPopupMenu = queryToolStripMenuItem;
            currentPopup = "Query";
            currentPopupMenu.Checked = true;
            LoadPopups(queryPopup);
        }

        private void unloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //unload the currently checked script
            FormMain.Instance.IceChatOptions.ScriptFiles = new string[scriptsToolStripMenuItem.DropDownItems.Count - 1];
            int count = 0;
            ToolStripMenuItem t = null;
            for (int i = 0; i < scriptsToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (!((ToolStripMenuItem)scriptsToolStripMenuItem.DropDownItems[i]).Checked)
                {
                    FormMain.Instance.IceChatOptions.ScriptFiles[count] = scriptsToolStripMenuItem.DropDownItems[i].Tag.ToString();
                    count++;
                }
                else
                    t = (ToolStripMenuItem)scriptsToolStripMenuItem.DropDownItems[i];

            }
            if (t != null)
            {
                scriptsToolStripMenuItem.DropDownItems.Remove(t);
                textScripts.Text = "";
            }
        }

        private void scriptsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(FormMain.Instance.CurrentFolder + System.IO.Path.DirectorySeparatorChar + "Scripts");
            }
            catch { }
        }
    }
}
