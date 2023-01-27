using System;
using System.Numerics;
using TicTacToeServer.Game;

namespace TicTacToeServer
{

    //链接处理脚本
    public static class InstructionSet
    {
        //登录逻辑处理
        public static void SginIn(PlayerClass playerClass, string str) 
        {
            //验证方法暂时空
            //判断是id还是Password
            //转入MySql系统
            if (playerClass.playerLinkStatus == LinkStatus.Signin)
            {
                PlayerMysqlHandle.TestHavePlayerID(playerClass, str);
            }
            else if (playerClass.playerLinkStatus == LinkStatus.InPassword)
            {
                PlayerMysqlHandle.TestPlayerPassword(playerClass, str);

            } else if (playerClass.playerLinkStatus == LinkStatus.registerid)
            {
                PlayerMysqlHandle.TestHavePlayerID(playerClass, str);

            } else if (playerClass.playerLinkStatus == LinkStatus.registerPossword)
            {
                PlayerMysqlHandle.AddPlayerData(playerClass, str);//写入密码

            } else if (playerClass.playerLinkStatus==LinkStatus.InGameIng)//在进入房间状态收到信息 
            {
                RoomSystem.JoinIn(playerClass,str);
                
            }
        }
        //系统指令系统
        public static void RunEvent(PlayerClass playerClass, string str)//执行指令系统 
        {
            if (str == "register")//玩家进入注册模式 
            {
                playerClass.ToRegister();//进入注册模式

            }
            if (str == "BuildRoom")//建立房间 
            {
                RoomSystem.AddList(playerClass);//新建房间，加入列表

                string RoomList = RoomSystem.GetAllPlayerListString();

                foreach (PlayerClass item in SocketClass.ServiceScoketList)
                {
                    SocketClass.SendPlayerAllGameList(item, RoomList);//更新所有列表
                }

                SocketClass.SendPlayerBuildRoomSuccess(playerClass);//向所有玩家发送建立房间成功
            }
            if (str=="RemoveRoom") 
            {
                 RoomSystem.GetRoom(playerClass).CloseRoom();
            }
            if (str=="JoinInRoom") 
            {
                playerClass.JoInRoom();//玩家进入房间状态
            }
            if (str=="Over") 
            {
                RoomSystem.GetRoom(playerClass).GameOver(playerClass);
            }
            if (str=="PlayerSuccess")
            {
                RoomSystem.GetRoom(playerClass).PlayerSuccess(playerClass);
            }
            if (str=="GetRoomList")//拉取所有房间数据
            {
                SocketClass.SendPlayerAllGameList(playerClass, RoomSystem.GetAllPlayerListString());//得到所有列表
            }
            if (str=="GetOtherPlayerID") 
            {
                Room room= RoomSystem.GetRoom(playerClass);

                if (room!=null)
                {
                    Console.WriteLine(room);
                    room.UpdateView();
                }
            }
            if (str== "Close")
            {
                //玩家断开链接
                //检查是否有玩家房间
                //Room room= RoomSystem.GetRoom(playerClass);

                //if (room!=null)//有则删除 
                //{
                //    room.CloseRoom();
                //}
                //SocketClass.CloseScoket(playerClass);

                playerClass.ExitGame();
            }

            if (str== "Client_Online") 
            {
                playerClass.isOnline = true;
            }
        }

        //坐标指令系统，得到玩家再那个房间
        public static void ChangeVector(PlayerClass playerClass,Vector2 Intovector2)//同步坐标 
        {
            Room room= RoomSystem.GetRoom(playerClass);
            
            room.SyncVector(playerClass,Intovector2);
        }

    }
}
