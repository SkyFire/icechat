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
using System.Collections.Generic;
using System.Text;

namespace IceChat
{
    public class User : IComparable
    {
        private string nickName;
        private IRCConnection connection;

        //public string Host = "";
        public bool[] Level;

        public int CompareTo(object obj)
        {
            User u = (User)obj;

            int compareNickValue = 0;
            int thisNickValue = 0;

            bool[] userCompareLevel = new bool[u.Level.Length];
            bool[] thisCompareLevel = new bool[this.Level.Length];
            
            for (int i = 0; i < userCompareLevel.Length; i++)
                userCompareLevel[i] = u.Level[i];
            
            for (int i = 0; i < thisCompareLevel.Length; i++)
                thisCompareLevel[i] = this.Level[i];

            Array.Reverse(userCompareLevel);
            Array.Reverse(thisCompareLevel);

            for (int i = userCompareLevel.Length - 1; i >= 0; i--)
            {
                //System.Diagnostics.Debug.WriteLine("checking1:" + i + ":" + u.Level[i].ToString() + ":" + u.ToString() + ":" + connection.ServerSetting.StatusModes[1][i].ToString());
                if (userCompareLevel[i])
                {
                    compareNickValue = i + 1;
                    break;
                }
            }

            for (int i = thisCompareLevel.Length - 1; i >= 0; i--)
            {
                //System.Diagnostics.Debug.WriteLine("checkin2:" + i + ":" + this.Level[i].ToString() + ":" + this.ToString() + ":" + connection.ServerSetting.StatusModes[1][i].ToString());
                if (thisCompareLevel[i])
                {
                    thisNickValue = i + 1;
                    break;
                }
            }

            //System.Diagnostics.Debug.WriteLine("compare:" + compareNickValue + ":" + thisNickValue + ":" + u.nickName + ":" + this.nickName);

            if (compareNickValue > thisNickValue)
                return 1;
            else if (compareNickValue == thisNickValue)
                return this.nickName.CompareTo(u.nickName);
            else
                return -1;
        }

        public User(string nick, IRCConnection connection)
        {
            this.connection = connection;
            Level = new bool[connection.ServerSetting.StatusModes[0].Length];
            for (int i = 0; i < this.Level.Length; i++)
            {
                if (nick.StartsWith(connection.ServerSetting.StatusModes[1][i].ToString()))
                {
                    this.Level[i] = true;
                    nick = nick.Substring(1);
                }
            }
            nickName = nick;

        }
        /*
        public User(string nick, string host, IRCConnection connection)
        {
            this.connection = connection;
            //this.Host = host;

            Level = new bool[connection.ServerSetting.StatusModes[0].Length];

            for (int i = 0; i < this.Level.Length; i++)
            {
                if (nick.StartsWith(connection.ServerSetting.StatusModes[1][i].ToString()))
                {
                    this.Level[i] = true;
                    nick = nick.Substring(1);
                }
            }
            nickName = nick;

        }
        */
        public override string ToString()
        {
            for (int i = 0; i < this.Level.Length; i++)
            {
                if (this.Level[i] == true)
                    return connection.ServerSetting.StatusModes[1][i] + nickName;
            }

            return nickName;
        }

        public string NickName
        {
            get
            {
                return this.nickName;
            }
            set
            {
                string n = value;
                for (int i = 0; i < connection.ServerSetting.StatusModes[1].Length; i++)
                {
                    if (n.StartsWith(connection.ServerSetting.StatusModes[1][i].ToString()))
                    {
                        this.Level[i] = true;
                        n = n.Substring(1);
                    }
                }
                nickName = n;

            }
        }

    }


}