/*
Copyright (c) 2018-2020 Rossmann-Engineering
Permission is hereby granted, free of charge, 

*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ModbusClient
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
        public static EasyModbus.ModbusClient  modbusClient = new EasyModbus.ModbusClient();

        delegate void UpdateReceiveDataCallback(); //更新接收数据回调

        //public delegate void 连接出错回调处理程序(object sender);//接收数据已更改处理程序
        //public event 连接出错回调处理程序  连接出错回调事件;// 
        //连接出错回调事件 += new 连接出错回调处理程序(TEST);
        //    if (连接出错回调事件 != null)
        //        连接出错回调事件(this);



        bool 准备线圈列表框   = false; // listBoxPrepareCoils
        bool 准备寄存器列表框 = false; //listBoxPrepareRegisters 

        string  接收数据 = null;
        string  发送数据 = null;

        public MainForm()
		{
            InitializeComponent();

            string 版本号 = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            this.Text = $"Modbus客户端: {版本号}";

			

            modbusClient.接收数据已更改事件 += new EasyModbus.ModbusClient.ReceiveDataChangedHandler(更新接收的数据);
            modbusClient.发送数据已更改事件 += new EasyModbus.ModbusClient.SendDataChangedHandler(更新发送的数据);
            modbusClient.连接已更改事件 += new EasyModbus.ModbusClient.ConnectedChangedHandler(更新已更改的连接);

            
                                                                                          //modbusClient.Baudrate = 9600;
                                                                                          //modbusClient.UnitIdentifier = 2;
            
        }

        private void TEST(object sender)
        {
            MessageBox.Show("连接出错回调事件");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //modbusClient.PingIP(1000);//
        }

        void 更新接收的数据(object sender)
		{
            接收数据 = "Rx: " + BitConverter.ToString(modbusClient.接收数据).Replace("-", " ") + System.Environment.NewLine;
            Thread thread = new Thread(更新接收文本框线程);
            thread.Start();
        }
       
        void 更新接收文本框线程()// updateReceiveTextBox
        {
            if (textBox1.InvokeRequired)
            {
                UpdateReceiveDataCallback d = new UpdateReceiveDataCallback(更新接收文本框线程);
                this.Invoke(d, new object[] {  });
            }
            else
            {
                textBox1.AppendText(接收数据);
                //textBox1.Text = 接收数据+"\n"+ textBox1.Text;
            }
        }

        void 更新发送的数据(object sender)
		{
            发送数据 = BitConverter.ToString(modbusClient.发送数据);
            发送数据 = "Tx: " + BitConverter.ToString(modbusClient.发送数据).Replace("-", " ") + System.Environment.NewLine;
            Thread thread = new Thread(更新发送文本框线程);
            thread.Start();

        }

        void 更新发送文本框线程()//updateSendTextBox
        {
            if (textBox1.InvokeRequired)
            {
                UpdateReceiveDataCallback d = new UpdateReceiveDataCallback(更新发送文本框线程);
                this.Invoke(d, new object[] { });
            }
            else
            {
                textBox1.AppendText(发送数据);
                //textBox1.Text = 发送数据 + "\n" + textBox1.Text;
            }
        }
		
		void BtnConnectClick(object sender, EventArgs e)
		{
			modbusClient.IPAddress = txtIpAddressInput.Text;
			modbusClient.Port = int.Parse(txtPortInput.Text);
			modbusClient.Connect();
		}	
        

        void 读线圈_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }
                bool[] serverResponse = modbusClient.读线圈(int.Parse(读开始地址.Text) - 1, int.Parse(读长度.Text));
                lsbAnswerFromServer.Items.Clear();
                for (int i = 0; i < serverResponse.Length; i++)
                {
                    lsbAnswerFromServer.Items.Add(serverResponse[i]);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 读取离散输入_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }
                bool[] serverResponse = modbusClient.读取离散输入(int.Parse(读开始地址.Text) - 1, int.Parse(读长度.Text));
                lsbAnswerFromServer.Items.Clear(); //清除列表
                for (int i = 0; i < serverResponse.Length; i++)
                {
                    lsbAnswerFromServer.Items.Add(serverResponse[i]);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 读保持寄存器_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }

                if (modbusClient.Connected)
                {
                    int[] serverResponse = modbusClient.读保持寄存器(int.Parse(读开始地址.Text) - 1, int.Parse(读长度.Text));

                    lsbAnswerFromServer.Items.Clear();
                    for (int i = 0; i < serverResponse.Length; i++)
                    {
                        lsbAnswerFromServer.Items.Add(serverResponse[i]);

                    }

                }   
            }
            catch (Exception exc)
            {
                modbusClient.Disconnect();
                timer1.Enabled = false;
                //断开_BT_Click(null, null);
                MessageBox.Show(exc.Message, "从服务器读取值时发生异常", MessageBoxButtons.OK, MessageBoxIcon.Error);//
                //MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);//“从服务器读取值时发生异常
                timer1.Enabled = true;

            }
        }
        private void 读输入寄存器_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }

                int[] serverResponse = modbusClient.读输入寄存器(int.Parse(读开始地址.Text) - 1, int.Parse(读长度.Text));

                lsbAnswerFromServer.Items.Clear();
                for (int i = 0; i < serverResponse.Length; i++)
                {
                    lsbAnswerFromServer.Items.Add(serverResponse[i]);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 写单线圈_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }

                bool coilsToSend = false;

                coilsToSend = bool.Parse(lsbWriteToServer.Items[0].ToString());


                modbusClient.写单线圈(int.Parse(txtStartingAddressOutput.Text) - 1, coilsToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 写多个线圈_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }

                bool[] coilsToSend = new bool[lsbWriteToServer.Items.Count];

                for (int i = 0; i < lsbWriteToServer.Items.Count; i++)
                {

                    coilsToSend[i] = bool.Parse(lsbWriteToServer.Items[i].ToString());
                }


                modbusClient.写多线圈(int.Parse(txtStartingAddressOutput.Text) - 1, coilsToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 写单个寄存器_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }

                int registerToSend = 0;

                registerToSend = int.Parse(lsbWriteToServer.Items[0].ToString());


                modbusClient.写单寄存器(int.Parse(txtStartingAddressOutput.Text) - 1, registerToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void 写多个寄存器_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    联机_BT_Click(null, null);
                }

                int[] registersToSend = new int[lsbWriteToServer.Items.Count];

                for (int i = 0; i < lsbWriteToServer.Items.Count; i++)
                {

                    registersToSend[i] = int.Parse(lsbWriteToServer.Items[i].ToString());
                }


                modbusClient.写多寄存器(int.Parse(txtStartingAddressOutput.Text) - 1, registersToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 选择Modbus方式_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modbusClient.Connected)
                modbusClient.Disconnect();

            if (选择Modbus方式.SelectedIndex == 0)
            {

                IP_COM.Text = "Server IP-Address";
                Port_ID.Text = "Server Port";

                txtIpAddressInput.Visible = true;
                txtPortInput.Visible = true;


                串口选择框.Visible = false;
                txtSlaveAddressInput.Visible = false;
                lblBaudrate.Visible = false;
                lblParity.Visible = false;
                lblStopbits.Visible = false;
                txtBaudrate.Visible = false;
                cbbParity.Visible = false;
                cbbStopbits.Visible = false;
            }
            if (选择Modbus方式.SelectedIndex == 1)
            {
                IP_COM.Text = "COM-Port";
                Port_ID.Text = "Slave ID";

                串口选择框.SelectedIndex = 0;
                cbbParity.SelectedIndex = 0;
                cbbStopbits.SelectedIndex = 0;
                if (串口选择框.SelectedText == "")
                    串口选择框.SelectedItem.ToString();

                //txtIpAddress.Visible = false;
                txtIpAddressInput.Visible = false;
                txtPortInput.Visible = false;
                串口选择框.Visible = true;
                txtSlaveAddressInput.Visible = true;
                lblBaudrate.Visible = true;
                lblParity.Visible = true;
                lblStopbits.Visible = true;
                txtBaudrate.Visible = true;
                cbbParity.Visible = true;
                cbbStopbits.Visible = true;


            }
        }

		void TxtSlaveAddressInputTextChanged(object sender, EventArgs e)
		{
            try
            {
                modbusClient.UnitIdentifier = byte.Parse(txtSlaveAddressInput.Text);
            }
            catch (FormatException){ }	
		}

        private void 清除1条_BT_Click(object sender, EventArgs e)
        {
            int rowindex = lsbWriteToServer.SelectedIndex;
            if (rowindex >= 0)
                lsbWriteToServer.Items.RemoveAt(rowindex);
        }
        private void 清除全部_BT_Click(object sender, EventArgs e)
        {
            lsbWriteToServer.Items.Clear();
        }
        private void 添加线圈_BT_Click(object sender, EventArgs e)
        {
            if (!准备线圈列表框)
            {
                lsbAnswerFromServer.Items.Clear();
            }
            准备线圈列表框 = true;
            准备寄存器列表框 = false;
            lsbWriteToServer.Items.Add(更改布尔值.Text);
        }   
        private void 添加寄存器_BT_Click(object sender, EventArgs e) //注册寄存器
        {
            if (!准备寄存器列表框)
            {
                lsbAnswerFromServer.Items.Clear();
            }
            准备寄存器列表框 = true;
            准备线圈列表框 = false;
            lsbWriteToServer.Items.Add(int.Parse(txtRegisterValue.Text));
        }


        private void lsbAnswerFromServer_DoubleClick(object sender, EventArgs e)
        {
            int rowindex = lsbAnswerFromServer.SelectedIndex;
        }

        private void 更改线圈值_DoubleClick(object sender, EventArgs e)//更改线圈值
        {
            if (更改布尔值.Text.Equals("FALSE"))
                更改布尔值.Text = "TRUE";
            else
                更改布尔值.Text = "FALSE";
        }

        private void 联机_BT_Click(object sender, EventArgs e)
        {
            try
            {
                if (modbusClient.Connected)
                    modbusClient.Disconnect();
                if (选择Modbus方式.SelectedIndex == 0)
                {


                    modbusClient.IPAddress = txtIpAddressInput.Text;
                    modbusClient.Port = int.Parse(txtPortInput.Text);
                    modbusClient.SerialPort = null;
                    //modbusClient.receiveDataChanged += new EasyModbus.ModbusClient.ReceiveDataChanged(UpdateReceiveData);
                    //modbusClient.sendDataChanged += new EasyModbus.ModbusClient.SendDataChanged(UpdateSendData);
                    //modbusClient.connectedChanged += new EasyModbus.ModbusClient.ConnectedChanged(UpdateConnectedChanged);

                    modbusClient.Connect();
                    timer1.Enabled = true;
                }
                if (选择Modbus方式.SelectedIndex == 1)//串口
                {
                    modbusClient.SerialPort = 串口选择框.SelectedItem.ToString();

                    modbusClient.UnitIdentifier = byte.Parse(txtSlaveAddressInput.Text);
                    modbusClient.Baudrate = int.Parse(txtBaudrate.Text);
                    if (cbbParity.SelectedIndex == 0)
                        modbusClient.Parity = System.IO.Ports.Parity.Even;
                    if (cbbParity.SelectedIndex == 1)
                        modbusClient.Parity = System.IO.Ports.Parity.Odd;
                    if (cbbParity.SelectedIndex == 2)
                        modbusClient.Parity = System.IO.Ports.Parity.None;

                    if (cbbStopbits.SelectedIndex == 0)
                        modbusClient.StopBits = System.IO.Ports.StopBits.One;
                    if (cbbStopbits.SelectedIndex == 1)
                        modbusClient.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    if (cbbStopbits.SelectedIndex == 2)
                        modbusClient.StopBits = System.IO.Ports.StopBits.Two;

                    modbusClient.Connect();
                }
            }
            catch (Exception exc)
            {
                timer1.Enabled = false;
                if(MessageBox.Show(exc.Message + "\n是否重新连接？", "Unable to connect to Server", MessageBoxButtons.YesNo, MessageBoxIcon.Error)== DialogResult.Yes)
                    timer1.Enabled = true;
            }

        }
        private void 断开_BT_Click(object sender, EventArgs e)
        {
            modbusClient.Disconnect();
            timer1.Enabled = false;
        }

        private void 更新已更改的连接(object sender)// UpdateConnectedChanged
        {
            if (modbusClient.Connected)
            {
                txtConnectedStatus.Text = "已连接到设备";
                txtConnectedStatus.BackColor = Color.Green;
            }
            else
            {
                txtConnectedStatus.Text = "没见有已连接设备";
                txtConnectedStatus.BackColor = Color.Red;
            }
        }

        #region 串口相关

        private void 串口选择框_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modbusClient.Connected)
                modbusClient.Disconnect();
            modbusClient.SerialPort = 串口选择框.SelectedItem.ToString();

            modbusClient.UnitIdentifier = byte.Parse(txtSlaveAddressInput.Text);

        }


        private void txtBaudrate_TextChanged(object sender, EventArgs e)//波特率文本更改事件
        {
            if (modbusClient.Connected)
                modbusClient.Disconnect();
            modbusClient.Baudrate = int.Parse(txtBaudrate.Text);
        }


        #endregion


        private void link_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.EasyModbusTCP.net");
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            读保持寄存器_BT_Click(null,null);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                重新连接 f = new 重新连接();
                f.ShowDialog();
            }
            catch (Exception)
            {
            }
            

        }

        private void Debug_Log_checkBox_Click(object sender, EventArgs e)
        {
            if ( Debug_Log_checkBox.Checked )
                modbusClient.DebugLogFileName = Thread.GetDomain().BaseDirectory + "Debug.log";//保存调试的日志文件名
            else
                modbusClient.DebugLogFileName = null;
        }

        private void Error_Log_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Error_Log_checkBox.Checked)
                modbusClient.ErrorLogFileName = Thread.GetDomain().BaseDirectory + "Error.log";//保存调试的日志文件名
            else
                modbusClient.ErrorLogFileName = null;
        }
    }
}
