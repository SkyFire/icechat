using System;
using System.Windows.Forms;

namespace IceChatPlugin
{
    public class Plugin : IPluginIceChat
    {
        private string m_Name;
        private string m_Author;
        private string m_Version;

        private Form m_MainForm;
        private MenuStrip m_MenuStrip;
        
        //all the events get declared here
        public event OutGoingCommandHandler OnCommand;

        public Plugin()
        {
            //set your default values here
            m_Name = "Default Plugin";
            m_Author = "Default Author";
            m_Version = "1.0";
        }

        public void Dispose()
        {

        }

        public void Initialize()
        {

        }

        //declare the standard properties
        public string Name
        {
            get { return m_Name; }
        }

        public string Author
        {
            get { return m_Author; }
        }

        public string Version
        {
            get { return m_Version; }
        }

        public Form MainForm
        {
            get { return m_MainForm; }
            set { m_MainForm = value; }
        }

        public MenuStrip MainMenuStrip
        {
            get { return m_MenuStrip; }
            set { m_MenuStrip = value; }
        }
    
        //declare the standard methods
        public void ShowInfo()
        {
            MessageBox.Show(m_Name + " Loaded", m_Name + " " + m_Author);
        }
        
        public void LoadSettingsForm(TabControl SettingsTab)
        {
            //when the Settings Form gets loaded, ability to add tabs

        }
        
        public void LoadColorsForm(TabControl OptionsTab)
        {
            //when the Options Form gets loaded, ability to add tabs

        }

        public void MainProgramLoaded()
        {

        }

        public void SaveColorsForm()
        {

        }

        public void SaveSettingsForm()
        {
        
        }

        //declare all the necessary events

        public PluginArgs ChannelMessage(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ChannelAction(PluginArgs args)
        {
            return args;
        }

        public PluginArgs QueryMessage(PluginArgs args)
        {
            return args;
        }

        public PluginArgs QueryAction(PluginArgs args)
        {
            return args;
        }
        
        public PluginArgs ChannelJoin(PluginArgs args)
        {
            return args;
        }
        
        public PluginArgs ChannelPart(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ServerQuit(PluginArgs args)
        {
            return args;
        }
        //args.Connection   -- current connection
        //args.Extra        -- command data 
        public PluginArgs InputText(PluginArgs args)
        {
            return args;
        }

        public void ServerRaw(PluginArgs args)
        {

        }
    }
}
