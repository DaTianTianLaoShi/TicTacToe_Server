using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace TicTacToeServer
{
    public static class MySqlConnect
    {
        public static MySqlConnection mySqlConnection;

        static string Constr = "server=127.0.0.1;User Id=root;password=Aa13039608078;Database=player;charset=utf8";//密码和数据库名称需要设置

        static string SelectTablestr= "select * from playerdata";//链接数据库

        //static string Constr = "server=127.0.0.1;User Id=root;password=Aa13039608078;Database=cat;charset=utf8";

        //static string SelectTablestr = "select * from familycat";

        public static bool isConnectMysqlOk = false; 
        public static void MySqlInit()//数据库初始化 
        {
            mySqlConnection = new MySqlConnection(Constr);


            try
            {
                mySqlConnection.Open();
                isConnectMysqlOk=mySqlConnection.Ping();
            }
            catch (Exception e)
            {
                Console.WriteLine("数据库链接失败"+e);
                //Console.ReadKey();
                throw;
            }

            if (isConnectMysqlOk == true)
            {
                Console.WriteLine("数据库已连接");
                MySqlCommand myselect = new MySqlCommand(SelectTablestr, mySqlConnection);
            }
            else 
            {
                Console.WriteLine("数据库连接错误");
            }

        }

        //发起链接
        public static MySqlConnection ConnectMySql() 
        {
            
            MySqlConnection mySqlConnection = new MySqlConnection(Constr);

            mySqlConnection.Open();

            return mySqlConnection;
        }

        //断开链接
        public static void DisconnectMySql(MySqlConnection mySqlConnection) 
        {
            mySqlConnection.Close();
        }

        public static void TestMySqlConnect()//验证数据库链接 
        {
            DataSet ds = new DataSet();
            MySqlDataAdapter da = new MySqlDataAdapter(SelectTablestr, mySqlConnection);
            da.Fill(ds);
            Console.WriteLine("验证数据数据库第一行数据:\n");
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                Console.WriteLine(ds.Tables[0].Rows[0][i]);
            }
        }

        static string operationMySql(string selstr) 
        {
            string Returnstring = "Null";

            MySqlConnection MySqlConnection= ConnectMySql();

            MySqlDataAdapter da = new MySqlDataAdapter(selstr, MySqlConnection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (ds.Tables[0].Rows.Count >= 1) //检查是否为空
                {
                    Returnstring = ds.Tables[0].Rows[0][i].ToString();
                }

            }

            DisconnectMySql(MySqlConnection);

            return Returnstring;
        }

        //检索玩家数据库
        public static bool ISHavePlayerID(string playerId)  //检查是否有玩家ID
        {
           
            string selstr = "select id from player.playerdata where id = \"" + playerId + "\";";

            string id = operationMySql(selstr);

            bool isHavePlayer = false;

            if (id == "Null")
            {
                isHavePlayer = false;
            }
            else 
            {
                isHavePlayer = true;
            }
            
            return isHavePlayer;
        }

        //检查密码是否正确
        public static bool ISPlayerPassWordSuccess(string playerId,string password) 
        {
            bool isTrue = false;
            //抽取对应ID的密码
            string selstr = "select password from player.playerdata where id = \"" + playerId + "\";";

            string Returnpassword= operationMySql(selstr);

            if (password==Returnpassword) 
            {
                isTrue = true;
            }

            return isTrue;
        }

        //新玩家信息写入数据库
        public static void AddPlayerData(PlayerClass playerClass, string password) 
        {
            string IntoString = "( " + playerClass.iD + "," + password + " );";

            string selstr = "INSERT INTO player.playerdata VALUES" + IntoString;

            //MySqlDataAdapter da = new MySqlDataAdapter(selstr, mySqlConnection);

            MySqlConnection sqlConnection= ConnectMySql();

            MySqlCommand myselect = new MySqlCommand(selstr, sqlConnection);

            //开启异步
            myselect.BeginExecuteNonQuery(AddDataCallBack, new MySQLCallBack(playerClass,myselect, sqlConnection));
           //myselect.ExecuteNonQuery();

        }

        static void AddDataCallBack(IAsyncResult asyncResult) 
        {
            MySQLCallBack mySQLCallBack = asyncResult.AsyncState as MySQLCallBack;

            SocketClass.SendPasswordSuccess(mySQLCallBack.playerClass_Obj);//向玩家发送设置密码成功

            mySQLCallBack.mySqlCommand_Obj.EndExecuteNonQuery(asyncResult);

            //DisconnectMySql(mySQLCallBack.mySqlCommand_Obj);
        }
        //删除玩家数据,暂时没有使用
        public static void RemovePlayerData(string playerId) 
        {
            string selstr = "delete from player.playerdata where id = \" " + playerId + "\" ;";
            //MySqlDataAdapter da = new MySqlDataAdapter(selstr, mySqlConnection);
            MySqlCommand myselect = new MySqlCommand(selstr, mySqlConnection);
            
            myselect.ExecuteNonQuery();

        }
        //修改玩家密码
        public static void UpdateData(string playerId, string password)
        {
            string selstr = "update player.playerdata set password =" + password + " where id= \"" + playerId + " \";";
            //MySqlDataAdapter da = new MySqlDataAdapter(selstr, mySqlConnection);
            MySqlCommand myselect = new MySqlCommand(selstr, mySqlConnection);

            myselect.ExecuteNonQuery();
        }
    }
}
