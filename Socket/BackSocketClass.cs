using System.Net.Sockets;

namespace TicTacToeServer
{
    public class BackSocketClass//储存Socket类
    {
        //返回Socket
        public Socket socket;
        public PlayerClass PlayerClass;

        public BackSocketClass() 
        {
            this.socket = null;
        }

        public BackSocketClass(Socket socket ) 
        {
            this.socket = socket;
        }

        public BackSocketClass(PlayerClass playerClass) 
        {
            this.PlayerClass = playerClass;

        }
    }
}
