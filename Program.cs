using MySql.Data.MySqlClient;
using System;

namespace TicTacToeServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("链接数据库");
            //MySqlConnect.MySqlInit();
            //MySqlConnect.TestMySqlConnect();

            Console.WriteLine("服务开始执行");
            SocketClass.StartService();//开始链接监听  

            Console.ReadKey();//临死锁定方案
        }
    }
}
