using System;
using System.Windows.Forms;

namespace IceChatPlugin
{
  

    public delegate void OutGoingCommandHandler(object sender, PluginArgs e);


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

        PluginArgs ChannelMessage(PluginArgs args);       //return whether default message has been overriden
        PluginArgs ChannelAction(PluginArgs args);
        PluginArgs QueryMessage(PluginArgs args);
        PluginArgs QueryAction(PluginArgs args);

        PluginArgs ChannelJoin(PluginArgs args);
        PluginArgs ChannelPart(PluginArgs args);
        PluginArgs ServerQuit(PluginArgs args);
        PluginArgs InputText(PluginArgs args);

        void ServerRaw(PluginArgs args);

        event OutGoingCommandHandler OnCommand;

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

        //public bool isHandled;      //if the default text message is over riden

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
