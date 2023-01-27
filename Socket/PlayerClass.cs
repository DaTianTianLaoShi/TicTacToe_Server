using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TicTacToeServer.Game;

namespace TicTacToeServer
{
    public enum LinkStatus//链接状态
    {
        Null,
        Signin,//登录中
        Online,//在线
        InPassword,//密码验证状态
       // register,//注册状态
        registerid,//注册id
        registerPossword,//注册密码
        InGameIng,//登录预备状态
        SignOut
    }

    public class PlayerClass
    {
        //字段
        LinkStatus PlayerLinkStatus=LinkStatus.Null;//状态
        string ID = null;//玩家ID
        byte[] Data = null;
        Socket PlayerSocket=null;//Socket链接
        int RoomID = -1; 
        bool IsOnline = true;//玩家是否在线


        public string iD { get => ID; set => ID = value; }
        public Socket playerSocket { get => PlayerSocket; set => PlayerSocket = value; }
        public LinkStatus playerLinkStatus { get => PlayerLinkStatus; set => PlayerLinkStatus = value; }
        public byte[] data { get => Data; set => Data = value; }
        public int roomID { get => RoomID; set => RoomID = value; }
        public bool isOnline { get => IsOnline; set => IsOnline = value; }

        public PlayerClass(Socket socket) 
        {
            playerSocket = socket;
        }

        public PlayerClass(string ID ,Socket socket) 
        {
            iD = ID;
            playerSocket=socket;
        }

        public void ToSignin()//进入登录状态,表示没有ID,没有验证密码
        {
            playerLinkStatus=LinkStatus.Signin;
            data=new byte[1024];
        }

        public void ToInPassword(string id) //进入密码验证模式
        {
            playerLinkStatus = LinkStatus.InPassword;
            iD = id;
        }
        
        public async void VerificationStatus()//每隔一段时间验证是否还在线 
        {
            await Task.Run(() => 
            {
                while (this.playerLinkStatus!=LinkStatus.SignOut)
                {
                    SocketClass.PlayerOnline(this);  
                    Thread.Sleep(3000*10);
                    if (IsOnline == false)//是否在线 
                    {
                        SocketClass.KickOutPlayer(this);
                        //Room room= RoomSystem.GetRoom(this);
                        //if (room!=null)
                        //{
                        //    room.CloseRoom();
                        //}
                        ////清理列表中的玩家
                        //SocketClass.CloseScoket(this);
                        this.ExitGame();
                        break;
                    }
                    else 
                    {
                        Console.WriteLine("确认玩家在线");
                    }
                    isOnline = false;
                }
                //发送验证客户端信息
                //3秒后验证
                //如果验证没有通过则关闭当前玩家链接
            });
        }

        //状态切换
        public void ToOnline()
        {
            playerLinkStatus=LinkStatus.Online;
        }

        public void ToRegister() 
        {
            playerLinkStatus = LinkStatus.registerid;
        }

        public void ToRegisterPasserWord()
        {
            playerLinkStatus = LinkStatus.registerPossword;
        }

        public void JoInRoom()//玩家加入游戏中
        {
            playerLinkStatus = LinkStatus.InGameIng;
        }

        public void ExitGame() //玩家断开链接
        {
            playerLinkStatus = LinkStatus.SignOut;
        }
    }
}
