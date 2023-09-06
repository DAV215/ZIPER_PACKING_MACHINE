using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActUtlTypeLib;
using ACTPCUSBLib;
using System.Drawing.Imaging;
using Bunifu.UI.WinForms;
using System.Threading.Tasks;
using System.Threading;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using System.Net;

namespace PLC_Q06
{
    public partial class MAINFORM : Form
    {
        public MAINFORM()
        {
            InitializeComponent();
            panel_plc_STATUS.Visible = false;
            panel_TEST_THEOKHAU.Visible = false;

            btn_calib_LOADCELL.Value = false;
            btn_CHUYENHE_TEST.Value = false;
            BTN_khauxa.Value = false;
            foreach(Control control in panel_TEST_CHITIET.Controls)
            {
                if(control  is BunifuToggleSwitch button)
                {
                    button.Value = false;
                }
            }
        }
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "r0pEDPEx4rhtBy1y4dTfWXVYwxmTORskniSl4QXo",
            BasePath = "https://datn---packzipperbag-default-rtdb.firebaseio.com"
        };
        IFirebaseClient ADMIN;

        public ActUtlType plc = new ActUtlType();
        public ActMLQCPUQUSB plc_2 = new ActMLQCPUQUSB();
        private void bunifuFormDock1_FormDragging(object sender, Bunifu.UI.WinForms.BunifuFormDock.FormDraggingEventArgs e)
        {

        }

        private void bunifuColorTransition1_ColorChanged(object sender, Bunifu.UI.WinForms.BunifuColorTransition.ColorChangedEventArgs e)
        {

        }

        private void BTN_CONNECT_Click(object sender, EventArgs e)
        {
            plc.ActLogicalStationNumber = Convert.ToInt32(Txt_Diachi.Text);
            plc.Open();
            String CPU_name;
            int CPU_code;
            int checkcpu = plc.GetCpuType(out CPU_name, out CPU_code);
            label_CPUNAME_1.Text = CPU_name;
            label_CPUCODE_1.Text = CPU_code.ToString();

            if (CPU_code != 0)
            {
                show_success_noti("Kết nối PLC thành công !");
                bunifuTransition1.ShowSync(panel_plc_STATUS, false, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.Scale);

            }
            else MessageBox.Show("Kiểm tra đường truyền và Logiacal !");
            
        }

        private void btn_runPLC_Click(object sender, EventArgs e)
        {
            plc.SetCpuStatus(0);
            MessageBox.Show("PLC ĐANG CHẠY !", "Trạng thái PLC:", MessageBoxButtons.OKCancel);
        }

        private void btn_pausePLC_Click(object sender, EventArgs e)
        {
            var confirm_pause_PLC =   MessageBox.Show("PLC sẽ tạm dừng hoạt động !", "Xác nhận tạm dừng PLC", MessageBoxButtons.OKCancel);
            if(confirm_pause_PLC == DialogResult.OK)
            {
                plc.SetCpuStatus(2);
                MessageBox.Show("PLC ĐANG TẠM DỪNG !", "Trạng thái PLC:", MessageBoxButtons.OKCancel);
            }
        }

        private void btn_stopPLC_Click(object sender, EventArgs e)
        {
            var confirm_pause_PLC = MessageBox.Show("PLC sẽ dừng hoạt động !", "Xác nhận dừng PLC", MessageBoxButtons.OKCancel);
            if (confirm_pause_PLC == DialogResult.OK)
            {
                plc.SetCpuStatus(1);
                MessageBox.Show("PLC ĐANG TẮT !", "Trạng thái PLC:", MessageBoxButtons.OKCancel);
            }
        }
        void get_expantion_module()
        {
            plc.Connect();
        }

        

        private void Btn_Disconnect_Click(object sender, EventArgs e)
        {
            plc.Close();
            String CPU_name;
            int CPU_code;
            int checkcpu = plc.GetCpuType(out CPU_name, out CPU_code);
            Label_CPUNAME.Text = CPU_name;
            Label_CPUCODE.Text = CPU_code.ToString();

            if (CPU_code == 0)
            {
                MessageBox.Show("Đã ngắt kết nối PLC !");

            }
            else MessageBox.Show("PLC chưa ngắt kết nối hoặc đã tắt !");
        }
        int plc_status;
        public int check_plc_active()
        {
            String CPU_name;
            int CPU_code;
            int checkcpu = plc.GetCpuType(out CPU_name, out CPU_code);

            if(CPU_code == 0) plc_status = 0;
            else plc_status = 1;
            return plc_status;
        }
        private void btn_connect_page_Click(object sender, EventArgs e)
        {
            page_MAIN.SetPage(0);
        }
        private void btn_monitor_page_Click(object sender, EventArgs e)
        {
            int tab;
            if (check_plc_active() == 0) tab = 5;
            else tab = 1;
            page_MAIN.SetPage(tab);

            if(tab == 1)
            {
                timer_monitor.Start();
               
            }

        }
        private void btn_setting_page_Click(object sender, EventArgs e)
        {
            //if (check_plc_active() == 0) page_MAIN.SetPage(5);
            //else tab_page = 2;
            if (check_plc_active() == 0) page_MAIN.SetPage(5);
            else
            {
                if (adminlogin == true)
                {
                    page_MAIN.SetPage(2);
                }
                else
                {
                    page_MAIN.SetPage(6);
                    tab_page = 2;
                }
            }
        }
        int tab_page;
        private void btn_edit_page_Click(object sender, EventArgs e)
        {
            if (adminlogin == true)
            {
                page_MAIN.SetPage(4);
            }
            else
            {
                page_MAIN.SetPage(6);
                tab_page=4;
            }
        }

        private void btn_testing_page_Click(object sender, EventArgs e)
        {
            if (check_plc_active() == 0) page_MAIN.SetPage(5);
            else
            {
                if (adminlogin == true)
                {
                    page_MAIN.SetPage(4);
                }
                else
                {
                    page_MAIN.SetPage(6);
                    tab_page=4;
                }
            }
            
        }
        float read_realnumber(string deviceAddress)
        {
            
            short plc_return;
            plc.ReadDeviceRandom2(deviceAddress, 2, out plc_return);

            float realNumber = (float)plc_return;
            return realNumber;
        }
        float read_Float_PLC(string add)
        {
            int[] address = new int[2];
            plc.ReadDeviceBlock(add, 2, out address[0]);
            byte[] highWordbyte = BitConverter.GetBytes(address[1]);
            byte[] lowWordbyte = BitConverter.GetBytes(address[0]);
            byte[] valueOFfloatbyte = { lowWordbyte[0], lowWordbyte[1], highWordbyte[0], highWordbyte[1] };
            float value = BitConverter.ToSingle(valueOFfloatbyte, 0);
            return value;
        }

        int[] write_float_PLC(float value)
        {
            byte[] floatbyte = BitConverter.GetBytes(value);
            byte[] highWordBytes = {floatbyte[2], floatbyte[3]};
            byte[] lowWordBytes = {floatbyte[1], floatbyte[0]};
            int[] returnValue = {BitConverter.ToInt16(lowWordBytes, 0), BitConverter.ToInt16(highWordBytes, 0)};
            return returnValue;
        }
        void timer_monitor_tick()
        {
           
        }
        int Tiendo;
        private void timer_monitor_Tick(object sender, EventArgs e)
        {
            //Read khoi luong tinh
            float Kluongtinh = read_Float_PLC("D0");
            label_KLuongtinh.Text = Convert.ToString(Kluongtinh);

            //Read Khoi luong dang can
            float dangcan = read_Float_PLC("D10");
            try
            {
                Cirkle_progress_DANGCAN.Value = Math.Abs(Convert.ToInt32(dangcan));
                if (dangcan - Convert.ToInt32(dangcan) < 0)
                {
                    Cirkle_progress_DANGCAN.SubScriptText = Convert.ToString("." + Math.Round(Convert.ToInt32(dangcan) - dangcan, 2) * 100);
                    label_dangcan.Text = Convert.ToString(dangcan);
                }
                else
                {
                    Cirkle_progress_DANGCAN.SubScriptText = Convert.ToString("." + Math.Round(dangcan - Convert.ToInt32(dangcan), 2) * 100);
                    label_dangcan.Text = Convert.ToString(dangcan);

                }
                Cirkle_progress_DANGCAN.Maximum = Convert.ToInt32(Kluongtinh);
            }
            catch (Exception ex) { }
            //Trang thai MAY
            int STT_MACHINE;
            plc.GetDevice("M0",out STT_MACHINE);

            if(STT_MACHINE == 1)
            {
                if(btn_calib_LOADCELL.Value == true)
                {
                    label_MACHINE_STT.Text = "Đang hiệu chỉnh LOADCELL";

                }
                else
                    label_MACHINE_STT.Text = "Đang hoạt động";

            }
            else
            {
                label_MACHINE_STT.Text = "Đang tắt";

            }
            //Kiem tra Tien do

            int T0, T1, T2, T3, T4, khautron, khauxa;
            plc.GetDevice("M3", out khautron);
            if (khautron == 1)
            {
                if (dangcan > Kluongtinh) Tiendo = 30;
                else Tiendo = 10;
            }
            else
            if (khautron == 0)
            {
                plc.GetDevice("M5", out khauxa);

                if (khauxa == 1)
                {
                    plc.GetDevice("T0", out T0);
                    plc.GetDevice("T1", out T1);
                    plc.GetDevice("T2", out T2);
                    plc.GetDevice("T3", out T3);
                    plc.GetDevice("T4", out T4);

                    if (T0 != 0) Tiendo = 40;
                    if (T1 != 0) Tiendo = 60;
                    if (T2 != 0) Tiendo = 70;
                    if (T3 != 0) Tiendo = 80;
                    if (T4 != 0) Tiendo = 90;
                    if (T4 != 0)
                    {
                        int CamBien_DongBi;
                        plc.GetDevice("X0A", out CamBien_DongBi);

                        if (CamBien_DongBi == 1) Tiendo = 100;
                        else Tiendo = 90;
                    }

                }

            }
            Cirkle_progress_TIENTRINH.Value = Tiendo;
            Cirkle_progress_TIENTRINH.Maximum = 100;

            int SLL_DADONG;
            plc.GetDevice("D20", out SLL_DADONG);
            label_SLL_DADONG.Text = Convert.ToString(SLL_DADONG);
        }

        private void btn_expantion_KLuongtinh_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            if (btn_expantion_KLuongtinh.Value == true)
            {
                panel_KLuongtinh_monitor.Size = new System.Drawing.Size(254, 85);

            }
            else panel_KLuongtinh_monitor.Size = new System.Drawing.Size(254, 30);
        }

        private void btn_Confirm_setting_Click(object sender, EventArgs e)
        {
            float KLuong_1bi = float.Parse(TXTBOX_setting_KLuong1bi.Text);
            plc.WriteDeviceBlock("D0", 2, ref write_float_PLC(KLuong_1bi)[0]);
            show_success_noti("Hoàn tất thiết lập máy !");
        }
        void show_success_noti( string content)
        {
            Success_noti success_Noti = new Success_noti();
            success_Noti.change_label_content(content);
            success_Noti.Show();
        }

        private void btn_modify_Vanxa_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y40", Convert.ToInt16(btn_modify_Vanxa.Value));
        }

        private void btn_modify_HB1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y41", Convert.ToInt16(btn_modify_HB1.Value));

        }
        private void btn_modify_Hutchankhong1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y42", Convert.ToInt16(btn_modify_Hutchankhong1.Value));
        }
        private void btn_modify_Day_HB1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y43", Convert.ToInt16(btn_modify_Day_HB1.Value));

        }

        private void btn_modify_HB2_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y44", Convert.ToInt16(btn_modify_HB2.Value));

        }

        private void btn_modify_Hutchankhong2_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y45", Convert.ToInt16(btn_modify_Hutchankhong2.Value));
        }

        private void btn_modify_DayXa_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y46", Convert.ToInt16(btn_modify_DayXa.Value));

        }

        private void btn_modify_DB1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y47", Convert.ToInt16(btn_modify_DB1.Value));

        }

        private void btn_modify_DB2_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y48", Convert.ToInt16(btn_modify_DB2.Value));

        }

        private void btn_modify_Den_Start_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y4A", Convert.ToInt16(btn_modify_Den_Start.Value));

        }

        private void btn_modify_Den_Stop_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            plc.SetDevice("Y4B", Convert.ToInt16(btn_modify_Den_Stop.Value));

        }
        public void check_Sensor_STATUS(string deviceaddress, PictureBox PictureBOX)
        {
            int stt;
            plc.GetDevice(deviceaddress, out stt);
            string filepatch_VIDEO = "D:\\STUDY\\Nam4-HK2\\DATN\\C#\\SOURCE\\CLIP\\icons8-light.gif";
            string filepatch_STATIC= "D:\\STUDY\\Nam4-HK2\\DATN\\C#\\SOURCE\\CLIP\\icons8-light-64.png";
            
            //Ktra ảnh động hay không !!!
            //Quá hay

            if (stt == 1)
            {
                int frameCount = 1;
                try
                {
                    frameCount = PictureBOX.Image.GetFrameCount(FrameDimension.Time);

                }
                catch (Exception) { }

                if (frameCount < 2 )
                {
                    PictureBOX.Image = Image.FromFile(filepatch_VIDEO);
                }   
            } else PictureBOX.Image = Image.FromFile(filepatch_STATIC);
            
        }

        private void timer_TESTMACHINE_Tick(object sender, EventArgs e)
        {
            check_Sensor_STATUS("X0", pictureBox_modify_START);
            check_Sensor_STATUS("X1", pictureBox_modify_STOP);
            check_Sensor_STATUS("X3", pictureBox_modify_EMG);
            check_Sensor_STATUS("X5", pictureBox_modify_CBTu_Xa);
            check_Sensor_STATUS("X6", pictureBox_modify_HB1);
            check_Sensor_STATUS("X7", pictureBox_modify_Day_HB1);
            check_Sensor_STATUS("X8", pictureBox_modify_HB2);
            check_Sensor_STATUS("X9", pictureBox_modify_DayXa);
            check_Sensor_STATUS("X0A", pictureBox_modify_DongMiengBi);
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void btn_START_Click(object sender, EventArgs e)
        {
            plc.SetDevice("M0", 1);
            plc.SetDevice("M5", 1);
            plc.SetDevice("M6", 0);
            plc.SetDevice("M1", 0);
            plc.SetDevice("M2", 0);
        }
        bool SET_PANEL_MACHINE_STT = false;

        private void btn_CLOSE_PANEL_MACHINR_STT_Click(object sender, EventArgs e)
        {
            SET_PANEL_MACHINE_STT = !SET_PANEL_MACHINE_STT;
            if (SET_PANEL_MACHINE_STT == true)
            {
                bunifuTransition_panel_Close_MACHINE_STT.ShowSync(panel_MACHINE_STT, true, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.HorizSlide);
                panel_MACHINE_STT.Controls.Add(btn_CLOSE_PANEL_MACHINR_STT);
                btn_CLOSE_PANEL_MACHINR_STT.Location = new Point(324, 10);

            }
            else
            {
                panel_MACHINE_STT.Visible = false;
                tab_Monitor.Controls.Add(btn_CLOSE_PANEL_MACHINR_STT);
                btn_CLOSE_PANEL_MACHINR_STT.Location = new Point(38, 468);
            }
        }

        private void btn_STOP_Click(object sender, EventArgs e)
        {
            plc.SetDevice("M0", 0);
            plc.SetDevice("M1", 1);
            plc.SetDevice("M2", 0);
        }

        private void btn_EMG_Click(object sender, EventArgs e)
        {
            plc.SetDevice("M0", 0);
            plc.SetDevice("M1", 0);
            plc.SetDevice("M2", 1);
        }

        private void btn_calib_LOADCELL_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            if(btn_calib_LOADCELL.Value == true)
            {
                plc.SetDevice("M9", 1);
                show_success_noti("Đã hiệu chỉnh LOADCELL xong !");
            }
            else
            {
                plc.SetDevice("M9", 0);

            }
        }
        
        private void btn_CHUYENHE_TEST_CheckedChanged(object sender, BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
                if (btn_CHUYENHE_TEST.Value == true)
                {
                panel_TEST_THEOKHAU.Visible = false;
                panel_TEST_CHITIET.Visible = true;
                panel_TEST_CHITIET.Location = new Point(20, 96);
            }
                else
                {
                panel_TEST_THEOKHAU.Visible = true;
                panel_TEST_CHITIET.Visible = false;
                panel_TEST_THEOKHAU.Location = new Point(20, 96);

            }
            
        }

       
        public void USER()
        {


        }
        private void MAINFORM_Load(object sender, EventArgs e)
        {

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        byte[] imageData = webClient.DownloadData(Login.linkavt);
                        using (var stream = new System.IO.MemoryStream(imageData))
                        {
                            Image image = Image.FromStream(stream);
                            PictureBox_AVT.Image = image;
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            catch(Exception ex) { }
            try
            {
                ADMIN = new FireSharp.FirebaseClient(ifc);
            }

            catch
            {
                MessageBox.Show("No Internet or Connection Problem", "Warning!");
            }


        }

        private void btn_CLOSEFORM_CheckedChanged(object sender, BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            try{
                foreach (Form form in Application.OpenForms)
                {
                    form.Close();
                }
            }
            catch { }

        }

        private void TXTBOX_setting_Saiso_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void BTN_khauxa_CheckedChanged(object sender, BunifuToggleSwitch.CheckedChangedEventArgs e) 
        { 
            plc.SetDevice("M5", Convert.ToInt16(BTN_khauxa.Value));
        }

        private void btn_EMG_Click_1(object sender, EventArgs e)
        {
            plc.SetDevice("M0", 0);
            plc.SetDevice("M1", 0);
            plc.SetDevice("M2", 1);
        }
        bool adminlogin;
        private void btn_SINGIN_ADMIN_Click(object sender, EventArgs e)
        {

            FirebaseResponse res2 = ADMIN.Get(@"Tech/" + textbox_USER_ADMIN.Text);
            MyUser RETechnican = res2.ResultAs<MyUser>();
            MyUser LoginTechnican = new MyUser()
            {
                Username = textbox_USER_ADMIN.Text,
                PassWord = textbox_PASSWORD_ADMIN.Text
            };
            if(MyUser.IsEqual(RETechnican, LoginTechnican) == false)
            {
                MyUser.ShowError();
                adminlogin = false;
            }
            else
            {
                var thread = new System.Threading.Thread(p =>
                {
                    Action action = () =>
                    {
                        show_success_noti("Chào mừng quay trở lại.");
                        page_MAIN.SetPage(tab_page);
                        if(tab_page == 5)
                        {
                            plc.SetCpuStatus(1);
                            timer_TESTMACHINE.Start();
                        }
                        adminlogin = true;

                    };
                    this.Invoke(action);
                    

                });
                thread.Start();

            }
        }
    }
}
