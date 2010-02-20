using System;
using System.Windows.Forms;

namespace IceChatPlugin
{
  
    public delegate void ChannelMessageHandler(object sender, PluginArgs e);
    public delegate void ChannelActionHandler(object sender, PluginArgs e);
    public delegate void QueryMessageHandler(object sender, PluginArgs e);
    public delegate void QueryActionHandler(object sender, PluginArgs e);

    public delegate void ChannelJoinHandler(object sender, PluginArgs e);
    public delegate void ChannelPartHandler(object sender, PluginArgs e);
    public delegate void ServerQuitHandler(object sender, PluginArgs e);

    public delegate void InputTextHandler(object sender, PluginArgs e);
    public delegate void ServerRawHandler(object sender, PluginArgs e);


    public interface IPluginIceChat
    {

        void Initialize();
        void Dispose();        

        string Name { get; }
        string Version { get; }
        string Author { get; }

        //sets the MainForm for IceChat
        Form MainForm { get; set; }
        void ShowInfo();
        MenuStrip MainMenuStrip { get; set; }

        //add an item to the mainform menu

        void LoadSettingsForm(System.Windows.Forms.TabControl SettingsTab);
        void LoadColorsForm(System.Windows.Forms.TabControl ColorsTab);
        void SaveColorsForm();
        void SaveSettingsForm();

        void MainProgramLoaded();       //the main icechat form/program has loaded

        bool ChannelMessage(PluginArgs args);       //return whether default message has been overriden
        bool ChannelAction(PluginArgs args);
        bool QueryMessage(PluginArgs args);
        bool QueryAction(PluginArgs args);

        bool ChannelJoin(PluginArgs args);
        bool ChannelPart(PluginArgs args);
        bool ServerQuit(PluginArgs args);
        bool InputText(PluginArgs args);

        void ServerRaw(PluginArgs args);

        event ChannelMessageHandler OnChannelMessage;
        event ChannelActionHandler OnChannelAction;
        event QueryMessageHandler OnQueryMessage;
        event QueryActionHandler OnQueryAction;

        event ChannelJoinHandler OnChannelJoin;
        event ChannelPartHandler OnChannelPart;
        event ServerQuitHandler OnServerQuit;

        event InputTextHandler OnInputText;

        event ServerRawHandler OnServerRaw;

    }

    public class PluginArgs : EventArgs
    {
        public string Message;  
        public string Nick;     
        public string Host;
        public string Channel;
        public string Extra;
        public Form Form;           
        public Object TextWindow;
        public Object Connection;

        public string Command;      //if you wish to return back a command

        public bool isHandled;      //if the default text message is over riden

        public PluginArgs(Object textwindow, string channel, string nick, string host, string message)
        {
            this.Channel = channel;
            this.Nick = nick;
            this.Host = host;
            this.Message = message;
            this.TextWindow = textwindow;
        }

        public PluginArgs(Form form)
        {
            this.Form = form;    
        }
        
        public PluginArgs(Object connection)
        {
            this.Connection = connection;
        }
    }
}
