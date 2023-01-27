using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeServer
{
    public class MySQLCallBack
    {
        PlayerClass PlayerClass_Obj;

        MySqlCommand MySqlCommand_Obj;

        MySqlConnection MySqlConnection_obj;

        public PlayerClass playerClass_Obj { get => PlayerClass_Obj; set => PlayerClass_Obj = value; }
        public MySqlCommand mySqlCommand_Obj { get => MySqlCommand_Obj; set => MySqlCommand_Obj = value; }
        public MySqlConnection mySqlConnection_obj { get => MySqlConnection_obj; set => MySqlConnection_obj = value; }

        public MySQLCallBack(PlayerClass playerClass,MySqlCommand mySqlCommand,MySqlConnection mySqlConnection) 
        {
            this.PlayerClass_Obj = playerClass;
            this.mySqlCommand_Obj = mySqlCommand;
            this.mySqlConnection_obj = mySqlConnection;
        }
    }
}
