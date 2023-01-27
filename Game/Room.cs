using System;
using System.Numerics;

namespace TicTacToeServer.Game
{
    public enum RoomStatic
    {
        Null,
        
        WaitPlayer,//等待玩家
        
        //SyncBlack,//同步黑棋

        //SyncWrite,//同步白棋

        blackwin,//黑方赢

        Writewin//白方赢
    }


    public class Room
    {

        
        PlayerClass playerClass1 = null;//房主

        PlayerClass playerClass2 = null;//另外一个房间

        int iD = 0;

        RoomStatic roomStatic = RoomStatic.Null;

        public PlayerClass PlayerClass1 { get => playerClass1; set => playerClass1 = value; }
        public PlayerClass PlayerClass2 { get => playerClass2; set => playerClass2 = value; }
        public int ID { get => iD; set => iD = value; }
        public RoomStatic RoomStatic { get => roomStatic; set => roomStatic = value; }

        public Room(int ID ,PlayerClass playerClass) 
        {
            this.ID = ID;
            
            this.playerClass1 = playerClass;

            RoomStatic=RoomStatic.WaitPlayer;//切换到等待玩家模式
        }


        public void PlayerAdd(PlayerClass playerClass)// 另外一个玩家加入
        {
            this.playerClass2 = playerClass;
            //roomStatic=RoomStatic.SyncBlack;//黑先手
        }

        public void CloseRoom() 
        {
            //向着玩家发送房间关闭

            if (playerClass1!=null)
            {
                SocketClass.CloseRoom(playerClass1);
            }
            if (playerClass2 != null) 
            {
                SocketClass.CloseRoom(playerClass2);
            }
            //再列表删除自己
            RoomSystem.RoomList.Remove(this);
        }

        public void SyncVector(PlayerClass playerClass, Vector2 vector2)//同步坐标
        {

            if (playerClass == playerClass1)
            {
                SocketClass.SendPlayerVector2(playerClass2, vector2.ToString());
            } else if (playerClass==playerClass2) 
            {
                SocketClass.SendPlayerVector2(playerClass1, vector2.ToString());
            }
        }

        public void PlayerSuccess(PlayerClass playerClass) //玩家胜利
        {
            if (playerClass == playerClass1)
            {
                RoomStatic=RoomStatic.blackwin;
            } else if (playerClass==playerClass2) 
            {
                RoomStatic = RoomStatic.Writewin;
            }
        }

        public void GameOver(PlayerClass playerClass) //玩家胜利
        {
            if (playerClass == playerClass1)
            {
                SocketClass.SendPlayerLost(playerClass2);
            }
            else 
            {
                SocketClass.SendPlayerLost(playerClass1);
            }

            PlayerSuccess(playerClass);
            
            CloseRoom();//游戏结束关闭房间
        }

        public void UpdateView()//更新玩家显示, 
        {
            //if (playerClass.iD == playerClass1.iD)//证明是房主发来信息，拉去玩家2，建立的时候玩家2不可能进入房间，所以不做处理
            //{
                //if (playerClass2!=null) 
                //{
                //    SocketClass.SendPlayerRoomViewChangeID(playerClass1, playerClass2.iD);
                //    SocketClass.SendPlayerRoomViewChangeID(playerClass2, playerClass2.iD);
                //}       
              //  Console.WriteLine("不做处理");
            // }
            //else //如果不是说明是房间2玩家
            //{
            if (playerClass1 != null && playerClass2 != null)
            {
                SocketClass.SendPlayerRoomViewChangeID(playerClass1, playerClass2.iD);

                SocketClass.SendPlayerRoomViewChangeID(playerClass2, playerClass1.iD);

                Console.WriteLine("做出处理");
            }
            else 
            {
                Console.WriteLine("不做处理");
            }
            //}
        }
    }
}
