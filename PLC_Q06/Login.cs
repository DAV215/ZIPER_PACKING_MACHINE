using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using System.IO;
namespace PLC_Q06
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "r0pEDPEx4rhtBy1y4dTfWXVYwxmTORskniSl4QXo",
            BasePath = "https://datn---packzipperbag-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;
        public static string linkavt = "";
        private void btn_SINGIN_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = client.Get(@"Users/" + textbox_USER.Text);
            MyUser ReUser = res.ResultAs<MyUser>();
            MyUser LoginUser = new MyUser()
            {
                Username = textbox_USER.Text,
                PassWord = textbox_PASSWORD.Text
            };
            if (MyUser.IsEqual(ReUser, LoginUser) == false)
            {
                MyUser.ShowError();
            }
            else
            {
                linkavt = ReUser.Linkavt;
                var thread2 = new System.Threading.Thread(p2 =>
                {
                    Action action = () =>
                    {
                        show_success_noti("Chào mừng quay trở lại.");

                    };
                    this.Invoke(action);


                });
                thread2.Start();

                MAINFORM mainform = new MAINFORM();
                mainform.Show();

            }
        }
        void show_success_noti(string content)
        {
            Success_noti success_Noti = new Success_noti();
            success_Noti.change_label_content(content);
            success_Noti.Show();
        }
        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(ifc);
            }

            catch
            {
                MessageBox.Show("No Internet or Connection Problem", "Warning!");
            }
            username_input = textbox_USER.Text;
        }
        String username_input ;

        public List<String> info()
        {
            FirebaseResponse res = client.Get(@"Users/" + username_input);
            MyUser ReUser = res.ResultAs<MyUser>();
            List<string> info_user = new List<string>();
            String name = ReUser.Username;
            String linkavt = ReUser.Linkavt;
            info_user.Add(name);
            info_user.Add(linkavt);
            return info_user; 
        }
        private void btn_SIGNUP_Click(object sender, EventArgs e)
        {

        }

        public static void CloseForm()
        {
            
        }
    }
}
