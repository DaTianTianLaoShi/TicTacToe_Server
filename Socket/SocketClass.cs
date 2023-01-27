using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TicTacToeServer.Game;

namespace TicTacToeServer
{
    internal static class SocketClass
    {
        public static Socket ServiceScoket = null;

        //public static Dictionary<string, PlayerClass> ServiceScoketList = null;

        public static List<PlayerClass> ServiceScoketList = null;

        public static void StartService()//开始 
        {
            //初始化数组
            ServiceScoketList = new List<PlayerClass>();

            ServiceScoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            IPHostEntry here = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localaddress = here.AddressList[1];

            Console.WriteLine("当前ip号:" + localaddress);

            IPAddress ConnectIPAddress = IPAddress.Parse(localaddress.ToString());

            IPEndPoint iPEndPoint = new IPEndPoint(ConnectIPAddress, 1000);
            try
            {
                ServiceScoket.Bind(iPEndPoint);//绑定端口
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                //Console.WriteLine("按任意建结束");
                //Console.ReadKey();
                //throw;
            }


            ServiceScoket.Listen(50);//大概能链接25个

            Console.WriteLine("开始监听链接");

            StartAccept();
        }


        static void StartAccept() //监听客户端链接
        {
            ServiceScoket.BeginAccept(AcceptCallBack, null);
        }

        static void AcceptCallBack(IAsyncResult async)//服务回调
        {
            Console.WriteLine("发现客户端链接");
            Socket HavePlayerSocket = null;
            //判断是否重复
            try
            {
                HavePlayerSocket = ServiceScoket.EndAccept(async);

                StartAccept();//重复开始监听
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.ReadKey();

                StartAccept();

                //throw;
            }
            if (HavePlayerSocket != null)
            {
                
                bool IsRepeat=false;

                foreach (var item in ServiceScoketList)
                {
                    if (item.playerSocket==HavePlayerSocket)
                    {
                        IsRepeat = true;

                        Console.WriteLine("链接状态"+IsRepeat);
                    }
                }
                if (IsRepeat == false)
                {
                    PlayerClass playerClass = new PlayerClass(HavePlayerSocket);

                    playerClass.VerificationStatus();

                    playerClass.ToSignin();//进入登录状态

                    ServerSocketBeginResvice(playerClass);//与客户端建立链接

                    SendPlayerSignIn(playerClass);//向玩家发送切换登录界面的信息

                    ServiceScoketList.Add(playerClass);//将玩家填入列表
                }
                else 
                {
                    Console.WriteLine("客户端重复添加失败");
                }
               
            }
        }

        public static void ServerSocketBeginResvice(PlayerClass playerClass) 
        {
            try
            {
                playerClass.playerSocket.BeginReceive(playerClass.data, 0, playerClass.data.Length, SocketFlags.None, ServerSocketBeginResviceCallBack, playerClass);
            }
            catch (Exception e)
            {

                Console.WriteLine("建立监听失败：" + e);
                //throw;
            }
            
        }
        static void ServerSocketBeginResviceCallBack(IAsyncResult result)//监听客户端信息 回调
        {
            PlayerClass playerClass= result.AsyncState as PlayerClass;

            ConnectStringHandle.OpenPack(playerClass,playerClass.data);//接受到的信息处理

            playerClass.data = new byte[1024];

            try
            {
                playerClass.playerSocket.EndReceive(result);
            }
            catch (Exception e)
            {

                Console.WriteLine(e+"客户端链接回调错误");
                //throw;
            }
           

            if (playerClass.playerLinkStatus == LinkStatus.SignOut)
            {
                Room room = RoomSystem.GetRoom(playerClass);

                if (room != null)//有则删除 
                {
                    room.CloseRoom();
                }
                SocketClass.CloseScoket(playerClass);
            }
            else 
            {
                ServerSocketBeginResvice(playerClass);
            }
        }

        //系统命令
        //系统命令类型
        //1.系统命令
        //2.游戏内部指令。
        public static void StartBeginSend(PlayerClass PlayerClass, int type,string str) //异步数据发送
        {
            Console.WriteLine("发送数据");

            byte[] bytes = Encoding.UTF8.GetBytes("#" + type.ToString() + str + "!");//形成数据包

            try
            {
                PlayerClass.playerSocket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SeverBeginSendCallBack,PlayerClass);
            }
            catch (Exception e)
            {
                Console.WriteLine("数据发送错误"+e);
                //throw;
            }
        }

        static void SeverBeginSendCallBack(IAsyncResult result)
        {
            PlayerClass playerClass = result.AsyncState as PlayerClass;

            try
            {
                playerClass.playerSocket.EndSend(result);

                if ( playerClass.playerLinkStatus == LinkStatus.SignOut)
                {
                    Room room = RoomSystem.GetRoom(playerClass);

                    if (room != null)//有则删除 
                    {
                        room.CloseRoom();
                    }
                    SocketClass.CloseScoket(playerClass);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e+"Socket已经被释放");
                //throw;
            }
        }
        //断开链接处理

        public static void CloseScoket(PlayerClass playerClass) 
        {
            //playerClass.playerSocket.BeginDisconnect();
            playerClass.playerSocket.Close();
            ServiceScoketList.Remove(playerClass);
        }

        //封装好用的API
        //MySqlAPi
        public static void SendPlayerSignIn(PlayerClass playerClass) //向链接着发送玩家登录信息
        {
            StartBeginSend(playerClass, 2, "Signin");
        }

        public static void SendPlayerHavePlayer(PlayerClass playerClass)//数据库里面有玩家
        {
            StartBeginSend(playerClass,2,"HavePlayer");
        }

        public static void SendPlayerNotHavePlayer(PlayerClass playerClass) //数据库里面没有玩家
        {
            StartBeginSend(playerClass, 2, "NotHavePlayer");
        }

        public static void SendPlayerPlayerOnline(PlayerClass playerClass) 
        {
            StartBeginSend(playerClass,2,"PlayerOnline_Ed");
        }

        public static void SendPlayerRegisteridHavePlayer(PlayerClass playerClass) //id重复
        {
            StartBeginSend(playerClass, 2, "RegisterHaveId");
        }

        public static void SendPlayerRegisteridNotHavePlayer(PlayerClass playerClass) //id没有重复
        {
            StartBeginSend(playerClass, 2, "RegisterNotHaveId");
        }
        public static void SendPlayerRegisteridNull(PlayerClass playerClass) 
        {
            StartBeginSend(playerClass, 2, "RegisterNullHaveId");
        }

        public static void SendPasswordSuccess(PlayerClass playerClass) 
        {
            StartBeginSend(playerClass, 2,"RegisterPosswordSuccess");
        } //密码设置成功

        public static void SendRegisterPosswordNull(PlayerClass playerClass) 
        {
            StartBeginSend(playerClass, 2, "RegisterPosswordNull");
        }

        public static void SendPlayerPlayerPasswordTrue(PlayerClass playerClass) //数据库里面没有玩家
        {
            StartBeginSend(playerClass, 2, "PasswordTrue");
        }

        public static void SendPlayerPlayerPasswordFalse(PlayerClass playerClass) //数据库里面没有玩家
        {
            StartBeginSend(playerClass, 2, "PasswordFalse");
        }

        public static void SendPlayerStartJoinRoom(PlayerClass playerClass) 
        {
            StartBeginSend(playerClass, 2, "JoinRoom");
        }
        public static void SendPlayerBuildRoomSuccess(PlayerClass playerClass) 
        {
            StartBeginSend(playerClass, 2, "BuildRoomSuccess");
        }

        public static void SendPlayerJoinRoomSuccess(PlayerClass playerClass) 
        {
            StartBeginSend(playerClass, 2, "JoinRoomSuccess");
        }

        //同步游戏坐标
        public static void SendPlayerVector2(PlayerClass playerClass,string vector2) 
        {
            StartBeginSend(playerClass,1,vector2.ToString());
        }

        public static void SendPlayerWin(PlayerClass playerClass)//发送玩家胜利
        {
            StartBeginSend(playerClass, 2, "win");
        }

        public static void SendPlayerLost(PlayerClass playerClass) //发送玩家失败
        {
            StartBeginSend(playerClass,2,"Over");
        }
        public static void CloseRoom(PlayerClass playerClass)
        {
            StartBeginSend(playerClass, 2, "CloseRoom");
        }

        //验证玩家是否在线
        public static void PlayerOnline(PlayerClass playerClass) //
        {
            StartBeginSend(playerClass, 2, "IsOnline");
        }
       
        public static void KickOutPlayer(PlayerClass playerClass)  //网络状态差没有接收到报文，踢掉玩家
        {
            StartBeginSend(playerClass, 2, "Klick");
        }

        public static void SendPlayerAllGameList(PlayerClass playerClass,string AllPlayerString) 
        {
            StartBeginSend(playerClass,4,AllPlayerString);
        }

        //类型五游戏房间内显示

        public static void SendPlayerRoomViewChangeID(PlayerClass playerClass,string ID) 
        {
            StartBeginSend(playerClass, 5, ID);
        }

    }
}
