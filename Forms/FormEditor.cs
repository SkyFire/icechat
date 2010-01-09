using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

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
        private ToolStripMenuItem currentPopupMenu;

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

        private void LoadAliases()
        {
            textAliases.Clear();

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

        private string[] LoadPopupMenu(string popupType)
        {
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
            par.ReferencedAssemblies.Add(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "IPluginIceChat.dll");
            par.GenerateExecutable = false;
            par.GenerateInMemory = true;
            par.OutputAssembly = Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "test_script.dll";
            
            System.CodeDom.Compiler.CompilerResults err = cp.CompileAssemblyFromSource(par, textScripts.Text);
            if (err.Errors.Count > 0)
            {
                foreach (System.CodeDom.Compiler.CompilerError ce in err.Errors)
                {
                    System.Diagnostics.Debug.WriteLine("Error:" + ce.ToString());
                }
                return;
            }

            System.Reflection.Module module = err.CompiledAssembly.GetModules()[0];
            Type mt = null;
            System.Reflection.MethodInfo mi = null;
            
            if (module != null)
                mt = module.GetType("IceChatScript.IceChatScript");
            else
                System.Diagnostics.Debug.WriteLine("fail module");

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

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textScripts.AppendText("using System.Windows.Forms;" + Environment.NewLine);
            textScripts.AppendText("using System;" + Environment.NewLine);
            textScripts.AppendText("namespace IceChatScript" + Environment.NewLine);
            textScripts.AppendText("{" + Environment.NewLine);
            textScripts.AppendText("  public class IceChatScript" + Environment.NewLine);
            textScripts.AppendText("  {" + Environment.NewLine);
            textScripts.AppendText("    static public void Main()" + Environment.NewLine);
            textScripts.AppendText("    {" + Environment.NewLine);
            textScripts.AppendText("      MessageBox.Show(\"testing\");" + Environment.NewLine);
            textScripts.AppendText("    }" + Environment.NewLine);
            textScripts.AppendText("   }" + Environment.NewLine);
            textScripts.AppendText("}");

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //save what is in the current
            if (tabControlEditor.SelectedTab.Text == "Scripts")
            {
                StreamWriter stream = new StreamWriter("script.txt");
                stream.WriteLine(textScripts.Text);
                stream.Flush();
                stream.Close();

                MessageBox.Show("script.txt Saved");
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("script.txt"))
            {
                StreamReader stream = new StreamReader("script.txt");
                textScripts.Text = stream.ReadToEnd();
                stream.Close();
            }
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

    }
}
