/******************************************************************************\
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
        private string currentFolder;
        
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

        public string CurrentFolder
        {
            get { return currentFolder; }
            set { currentFolder = value; }
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

        public PluginArgs ChannelKick(PluginArgs args)
        {
            return args;
        }

        public PluginArgs ServerNotice(PluginArgs args)
        {
            return args;
        }

        public PluginArgs UserNotice(PluginArgs args)
        {
            return args;
        }

        public PluginArgs CtcpMessage(PluginArgs args)
        {
            //args.Extra        -- ctcp message 
            return args;
        }

        public PluginArgs CtcpReply(PluginArgs args)
        {
            //args.Extra        -- ctcp message 
            return args;
        }

        public PluginArgs ServerMessage(PluginArgs args)
        {
            
            return args;
        }

        public void NickChange(PluginArgs args)
        {

        }

        public void ServerRaw(PluginArgs args)
        {

        }

        public void ServerError(PluginArgs args)
        {

        }
    }
}
