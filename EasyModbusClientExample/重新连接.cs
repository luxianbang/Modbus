using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModbusClient
{
    public partial class 重新连接 : Form
    {
        int 计次 = 0;
        public  重新连接()
        {
            InitializeComponent();
            this.CenterToParent();
            this.TopMost = true;
        }

        ~重新连接()//关闭前会调用
        {
            //timer1.Enabled = false;
            //timer1.Dispose();
        }

        private void 取消_BT_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            计次++;

            label2.Text = "尝试连接设备: " + 计次.ToString()+" 次";//尝试连接: 1 次(共 10 次)

            try
            {
                MainForm.modbusClient.Connect();
                if (MainForm.modbusClient.Connected)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    if (timer1 != null)
                        timer1.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                label3.Text = ex.Message;
                if (timer1 != null)
                    timer1.Enabled = true;
            }
        }

        private void 重新连接_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();
        }

        private void 重新连接_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1_Tick(null, null);
        }
    }
}
