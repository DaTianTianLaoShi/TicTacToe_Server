namespace TicTacToeServer
{
    public static class PlayerMysqlHandle
    {
        //检查是否有玩家ID
        public static void TestHavePlayerID(PlayerClass playerClass ,string PlayerID) 
        {

            if (PlayerID == "")
            {
                SocketClass.SendPlayerRegisteridNull(playerClass);
            } else  
            {
                bool isHavePlayer = MySqlConnect.ISHavePlayerID(PlayerID);

                if (playerClass.playerLinkStatus == LinkStatus.registerid)//如果是注册状态
                {
                    if (isHavePlayer == true)//如果玩家有注册
                    {
                        SocketClass.SendPlayerRegisteridHavePlayer(playerClass);

                    }
                    else if (isHavePlayer == false)
                    {
                        playerClass.iD = PlayerID;//写入ID
                                                  
                        playerClass.ToRegisterPasserWord();//切换状态

                        SocketClass.SendPlayerRegisteridNotHavePlayer(playerClass);
                    }
                }
                else if(playerClass.playerLinkStatus == LinkStatus.Signin)//如果是登录状态
                {
                    if (isHavePlayer == true)//如果玩家有注册
                    {

                        //判断玩家是否在登录，如果账号已经登录则登录失败
                        bool IsOnline=false;

                        foreach (PlayerClass item in SocketClass.ServiceScoketList)
                        {
                            if (item.iD==PlayerID)
                            {
                                IsOnline = true;
                            }
                        }

                        if (IsOnline == false)
                        {
                            //更改玩家的模式
                            playerClass.ToInPassword(PlayerID);

                            SocketClass.SendPlayerHavePlayer(playerClass);
                        }
                        else 
                        {
                            SocketClass.SendPlayerPlayerOnline(playerClass);
                        }
                    }
                    else if (isHavePlayer == false)
                    {
                        SocketClass.SendPlayerNotHavePlayer(playerClass);

                        playerClass.ToSignin();
                    }
                }
            }
        }
        //检测注册
        //检查玩家密码是否正确
        public static void TestPlayerPassword(PlayerClass playerClass, string PassWord) 
        {
            bool IsTrue= MySqlConnect.ISPlayerPassWordSuccess(playerClass.iD,PassWord);

            if (IsTrue == true) 
            {
                playerClass.ToOnline();
                SocketClass.SendPlayerPlayerPasswordTrue(playerClass);
            }
            else if(IsTrue==false)
            {
                SocketClass.SendPlayerPlayerPasswordFalse(playerClass);
            }
        }

        //为新用户写入密码

        public static void AddPlayerData(PlayerClass playerClass,string PassWord) 
        {
            if (PassWord == "")
            {
                SocketClass.SendRegisterPosswordNull(playerClass);//玩家密码为空
                playerClass.ToRegister();
            }
            else 
            {
                MySqlConnect.AddPlayerData(playerClass, PassWord);
                
                playerClass.ToRegister();
                //客户端返回   
            }
        }
    }
}
