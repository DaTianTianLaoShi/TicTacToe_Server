using System;
using System.Numerics;
using System.Text;

namespace TicTacToeServer
{
    internal static class ConnectStringHandle
    {
        //翻译包，解析包
        //将接收的包拆分

        public enum OpenPackEnum
        {
            open,
            getNumber,
            write,
            close
        };

        public static OpenPackEnum packEnum = OpenPackEnum.close;


        public static void OpenPack(PlayerClass playerClass,byte[] bytes)
        {
            packEnum = OpenPackEnum.close;
            //判断起始符号
            string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            char[] chars = new char[1024];//可以适当缩小
            int ClassNumber = 0; //写入状态
            int ChangeNumber = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].ToString() == "#")
                {
                    packEnum = OpenPackEnum.open;
                    chars = new char[1024];
                    ClassNumber = 0;
                    ChangeNumber = 0;

                }
                else if (str[i].ToString() == "!")
                {
                    string TestStr = string.Concat<char>(chars);
                    // Debug.Log(TestStr);
                    if (ClassNumber == 1)//坐标同步
                    {
                        ServerVectorSyuc(playerClass, chars);
                    }
                    else if (ClassNumber == 2)//命令同步
                    {
                        CommandSync(playerClass, chars);
                    } else if (ClassNumber==3) 
                    {
                        LogInSync(playerClass, chars);//登录信息
                    }

                    packEnum = OpenPackEnum.close;
                    ClassNumber = 0;
                }
                else if (packEnum == OpenPackEnum.open)
                {
                    packEnum = OpenPackEnum.getNumber;
                    try
                    {
                        ClassNumber = int.Parse(str[i].ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("字符解析错误"+e);
                        //throw;
                    }
                   
                    
                }
                else if (ClassNumber != 0 && packEnum == OpenPackEnum.getNumber)
                {
                    packEnum = OpenPackEnum.write;
                }
                if (packEnum == OpenPackEnum.write)
                {
                    chars[ChangeNumber] = str[i];//分配缓存
                    ChangeNumber += 1;
                }
            }
        }

        //解析游戏坐标包

        public static void ServerVectorSyuc(PlayerClass playerClass, char[] str)
        {
            Vector2 InServerVector2 = InV2ServerSync(str);
            Console.WriteLine("接受到坐标包信息"+InServerVector2.ToString());
            InstructionSet.ChangeVector(playerClass,InServerVector2);
        }


        static Vector2 InV2ServerSync(char[] str)//坐标解码
        {

            //string TestStr = Encoding.UTF8.GetString(str, 0, str.Length);
            string TestStr = string.Concat<char>(str);
            //Debug.Log(TestStr);

            Vector2 Out_vector2 = new Vector2();
            char[] chars = new char[str.Length];
            int number = 0;
            //坐标标准化
            foreach (var item in str)
            {
                string str1 = item.ToString();

                if (str1 != "(" && str1 != ")" && str1 != null)
                {
                    chars[number] = item;

                    number = number + 1;
                }
            }
            string str3 = string.Concat<char>(chars);

            int X_int = 0;
            char[] X_chars = new char[str3.Length];
            string X_string = null;

            char[] Y_chars = new char[str3.Length];
            string Y_string = null;
            //拆分坐标
            bool Is_y = false;

            foreach (var item in str3)
            {
                string str1 = item.ToString();

                if (str1 == ",")
                {
                    X_int = 0;

                    Is_y = true;

                }
                //X_chars
                if (str1 != "," && str1 != null)
                {
                    if (Is_y == false)
                    {
                        X_chars[X_int] = item;
                    }
                    else if (Is_y == true)
                    {
                        Y_chars[X_int] = item;
                    }

                    X_int = X_int + 1;
                }
            }

            X_string = string.Concat<char>(X_chars);
            Y_string = string.Concat<char>(Y_chars);

            Out_vector2.X = Convert.ToSingle(X_string);
            Out_vector2.Y = Convert.ToSingle(Y_string);

            //Debug.Log(Out_vector2);
            return Out_vector2;
        }

        //命令系统包
        static string InCommandServerSync(char[] str)
        {
            string CommandString = "";
            //char[] chars = new char[str.Length];
            int X_int = 0;

            foreach (char item in str)
            {
                string str2 = item.ToString();

                if (str2 != "\0")//过滤多余字符 
                {
                    //chars[X_int]=item;
                    CommandString += str2;
                }

                X_int += 1;
            }
            return CommandString;
        }

        public static void CommandSync(PlayerClass playerClass, char[] str)
        {
            string strs=InCommandServerSync(str);
            Console.WriteLine("接受系统命令包信息"+strs);
            InstructionSet.RunEvent(playerClass, strs);
        }

        //登录系统包

        static string InLogInSync(char[] str) //需要修改
        {
            string logInString = "";

            foreach (char item in str)
            {
                string str2 = item.ToString();

                if (str2 != "\0")//过滤多余字符 
                {
                    logInString += str2;
                }
            }
            return logInString;
        }

        //1.优先开发，功能

        public static void LogInSync(PlayerClass playerClass, char[] str)//登录包解析 
        {
            string InLogString= InLogInSync(str);
            Console.WriteLine("接受到登录包信息"+InLogString);

            //判断玩家当前状态
            InstructionSet.SginIn(playerClass,InLogString);
        }
    }
}
 