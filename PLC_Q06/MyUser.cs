using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC_Q06
{
    class MyUser
    {
        public MyUser(string username, string passWord)
        {
            Username = username;
            PassWord = passWord;

        }

        public MyUser()
        {
        }

        public String Username { get; set; }
        public String PassWord { get; set; }
        public String Keydate { get; set; }
        public String Linkavt { get; set; }

        private static string error;
        public static void ShowError()
        {
            System.Windows.Forms.MessageBox.Show(error);
        }
        public static bool IsEqual(MyUser user1, MyUser user2)
        {
            if (user1 == null || user2 == null) 
            { 
                return false;
            }
            if(user1.Username != user2.Username)
            {
                error = "Khong ton tai nguoi dung";
                return false;

            }
            else if(user1.PassWord != user2.PassWord)
            {
                error = "Mat khau khong dung";
                return false;

            }
            return true;
        }
    }
}
