using System.Collections.Generic;
using TicTacToeServer.Game;

namespace TicTacToeServer
{

    public class RoomViewTable
    {
        string Playerid="";
        string roomID = "";

        public RoomViewTable(Room room) 
        {
            this.Playerid = room.PlayerClass1.iD;
            this.roomID = room.ID.ToString();
        }

        public string GetRoomString()//将当前的房间信息转换成字符串 
        {
            string str = "";

            str = "[p"+ Playerid + ",r"+roomID+"];";//房主id,房间id

            return str;
        }
    }
    public static class RoomSystem
    {
        //房间列表
        public static List<Room> RoomList = new List<Room>();

        public static int id = 0;
        //创建房间
        public static void AddList(PlayerClass playerClass) 
        {
            Room room = new Room(id,playerClass);
            RoomList.Add(room);
            id++;
            if (id>9999) 
            {
                id = 0;
            }
        }
        //销毁房间
        public static void RemoveAddList(int id) 
        {
            Room room=null;

            foreach (var item in RoomList)
            {
                if (item.ID==id)
                {
                    room = item;
                    break;
                }
            }

            if (room!=null)
            {
                RoomList.Remove(room);
            }
        }


        //返回所有房间列表,返回的列表为string
        public static string GetAllPlayerListString() 
        {
            string AllplayerString = "";

            foreach (var item in RoomList)
            {
                if (item.PlayerClass1 == null || item.PlayerClass2 == null)//只显示没满员的房间
                {
                    RoomViewTable roomView = new RoomViewTable(item);
                    AllplayerString += roomView.GetRoomString();
                }  
            }

            return AllplayerString;
        }

        public static Room GetRoom(PlayerClass playerClass) //找到玩家再哪个房间
        {
            Room room = null;
            foreach (var item in RoomList)
            {

                if (item.PlayerClass1!=null)
                {
                    if (playerClass.iD == item.PlayerClass1.iD)
                    {
                        room = item;
                        break;
                    }
                }
                if (item.PlayerClass2!=null)
                {
                    if (playerClass.iD == item.PlayerClass2.iD)
                    {
                        room = item;
                        break;
                    }
                }
            }
            return room;
        }

        public static Room getIDRoom(string ID) 
        {
            Room room = null;

            foreach (var item in RoomList)
            {
                if (item.ID.ToString()==ID)
                {
                    room = item;
                    break ;
                }
            }

            return room;
        }

        public static Room GetPlayerIDRoom(string PlayerID) 
        {
            Room room = null;

            foreach (var item in RoomList)
            {
                if (item.PlayerClass1.iD == PlayerID)
                {
                    room = item;
                    break;
                }
            }

            return room;
        }

        public static void JoinIn(PlayerClass playerClass,string PlayerID) //玩家加入房间
        {
            //Room room= getIDRoom(ID);
            Room room= GetPlayerIDRoom(PlayerID);

            if (room!=null) 
            {
                room.PlayerAdd(playerClass);

            }

            if (SocketClass.ServiceScoket!=null)
            {
                //发送加入房间成功
                SocketClass.SendPlayerJoinRoomSuccess(playerClass);
            }

        }
    }
}
