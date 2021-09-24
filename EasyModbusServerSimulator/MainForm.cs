using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EasyModbus;
using System.Reflection;

namespace EasyModbusServerSimulator
{
    public partial class MainForm : Form
    {
        连接设置类 settings = new 连接设置类();
        ModbusServer ModbusServer_TCP;
        private UInt16  离散输入起始地址 = 1;// startingAddressDiscreteInputs
        private UInt16  线圈起始地址 = 1;//startingAddressCoils
        private UInt16  保持寄存器起始地址 = 1; //startingAddressHoldingRegisters
        private UInt16  输入寄存器起始地址 = 1;//startingAddressInputRegisters
        private bool    显示协议信息 = true; //QshowProtocolInformations
        //private bool preventInvokeDiscreteInputs = false;
        private bool 禁止调用线圈 = false; //
        //private bool preventInvokeInputRegisters = false;
        private bool 禁止调用保持寄存器 = false;

        public MainForm()
        {
            InitializeComponent();

            string 版本号 = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            lblVersion.Text = 版本号;
            ModbusServer_TCP = new ModbusServer();

            ModbusServer_TCP.侦听();


            ModbusServer_TCP.CoilsChanged += new ModbusServer.CoilsChangedHandler(CoilsChanged);
            ModbusServer_TCP.HoldingRegistersChanged += new ModbusServer.HoldingRegistersChangedHandler(HoldingRegistersChanged);
            ModbusServer_TCP.NumberOfConnectedClientsChanged += new ModbusServer.NumberOfConnectedClientsChangedHandler(更改的连接数);
            ModbusServer_TCP.LogDataChanged += new ModbusServer.LogDataChangedHandler(日志数据变更);    
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                numericUpDown1.Value = 离散输入起始地址;
                离散输入_数据网格视图.Rows.Clear();
                for (int i = 离散输入起始地址; i < 20 + 离散输入起始地址; i++)
                {
                    离散输入_数据网格视图.Rows.Add(i, ModbusServer_TCP.离散输入[i]);
                    
                    if (ModbusServer_TCP.离散输入[i])
                        离散输入_数据网格视图[1, i - 离散输入起始地址].Style.BackColor = Color.Green;
                    else
                        离散输入_数据网格视图[1, i - 离散输入起始地址].Style.BackColor = Color.Red;
                }
            }
            if (tabControl1.SelectedIndex == 1)
            {
                
                numericUpDown1.Value = 线圈起始地址;
                线圈_数据网格视图.Rows.Clear();
                for (int i = 线圈起始地址; i < 20 + 线圈起始地址; i++)
                {
                    线圈_数据网格视图.Rows.Add(i, ModbusServer_TCP.线圈[i]);
                    if (ModbusServer_TCP.线圈[i])
                        线圈_数据网格视图[1, i - 线圈起始地址].Style.BackColor = Color.Green;
                    else
                        线圈_数据网格视图[1, i - 线圈起始地址].Style.BackColor = Color.Red;
                }
            }
            if (tabControl1.SelectedIndex == 2)
            {
                numericUpDown1.Value = 输入寄存器起始地址;
                输入寄存器_数据网格视图.Rows.Clear();
                for (int i = 输入寄存器起始地址; i < 20 + 输入寄存器起始地址; i++)
                    输入寄存器_数据网格视图.Rows.Add(i, ModbusServer_TCP.输入寄存器[i]);
            }
            if (tabControl1.SelectedIndex == 3)
            {
                numericUpDown1.Value = 保持寄存器起始地址;
                保持寄存器_数据网格视图.Rows.Clear();
                for (int i = 保持寄存器起始地址; i < 20 + 保持寄存器起始地址; i++)
                    保持寄存器_数据网格视图.Rows.Add(i, ModbusServer_TCP.保持寄存器[i]);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1_SelectedIndexChanged(null, null);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                离散输入起始地址 = (UInt16)numericUpDown1.Value;
            if (tabControl1.SelectedIndex == 1)
                线圈起始地址 = (UInt16)numericUpDown1.Value;
            if (tabControl1.SelectedIndex == 2)
                输入寄存器起始地址 = (UInt16)numericUpDown1.Value;
            if (tabControl1.SelectedIndex == 3)
                保持寄存器起始地址 = (UInt16)numericUpDown1.Value;
            tabControl1_SelectedIndexChanged(null, null);

        }

       

        delegate void coilsChangedCallback(int coil, int numberOfCoil);
        private void CoilsChanged(int coil, int numberOfCoil)
        {
            if (禁止调用线圈)
                return;
            if (this.tabControl1.InvokeRequired)
            {

                {
                    coilsChangedCallback d = new coilsChangedCallback(CoilsChanged);
                    this.Invoke(d, coil, numberOfCoil);
                }
            }
            else
            {
                if (tabControl1.SelectedIndex == 1)
                    tabControl1_SelectedIndexChanged(null, null);
            }
        }

        delegate void registersChangedCallback(int register, int numberOfRegisters);
        bool registersChanegesLocked;
        private void HoldingRegistersChanged(int register, int numberOfRegisters)
        {
            if (禁止调用保持寄存器)
                return;

                try
                {
                    if (this.tabControl1.InvokeRequired)
                    {
                        {
                            if (!registersChanegesLocked)
                                lock (this)
                                {
                                    registersChanegesLocked = true;

                                    registersChangedCallback d = new registersChangedCallback(HoldingRegistersChanged);
                                    this.Invoke(d, register, numberOfRegisters);
                                }
                        }
                    }
                    else
                    {
                        if (tabControl1.SelectedIndex == 3)
                            tabControl1_SelectedIndexChanged(null, null);
                    }
                }
                catch (Exception) { }
                registersChanegesLocked = false;
            }
        

        bool 锁定已更改的连接数 = false;//LockNumberOfConnectionsChanged
        delegate void numberOfConnectionsCallback();
        private void 更改的连接数()
        {
            if (this.客户端数.InvokeRequired & !锁定已更改的连接数)
            {
                {
                    lock (this)
                    {
                        锁定已更改的连接数 = true;
                        numberOfConnectionsCallback d = new numberOfConnectionsCallback(更改的连接数);
                        try  { this.Invoke(d); }  catch (Exception) { }
                        
                        finally { 锁定已更改的连接数 = false; }
                    }
                }
            }
            else
            {
                try  { 客户端数.Text = ModbusServer_TCP.NumberOfConnections.ToString();  }  catch (Exception) { }
            }
        }


        delegate void 日志数据变更回调();
        bool locked;
        private void 日志数据变更()
        {

            if (显示协议信息 == true)
            { 
                if (this.协议信息_列表框.InvokeRequired)
                {
                    if (!locked)
                    {
                        lock (this)
                        {
                            locked = true;
                            try
                            {
                                日志数据变更回调 d = new 日志数据变更回调(日志数据变更);
                                this.Invoke(d);
                            }
                            catch (Exception) { } 
                        } 
                    }
                }
                else
                { 
                    try
                    {
                        协议信息_列表框.Items.Clear();
                        string listBoxData;
                        for (int i = 0; i < ModbusServer_TCP.ModbusLogData.Length; i++)
                        {
                            if (ModbusServer_TCP.ModbusLogData[i] == null)
                                break;
                            if (ModbusServer_TCP.ModbusLogData[i].请求)
                            {
                                listBoxData = ModbusServer_TCP.ModbusLogData[i].时间戳.ToString("H:mm:ss.ff") + " Request from Client - Functioncode: " + ModbusServer_TCP.ModbusLogData[i].功能码.ToString();
                                if (ModbusServer_TCP.ModbusLogData[i].功能码 <= 4)
                                {
                                    listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " Quantity: " + ModbusServer_TCP.ModbusLogData[i].数量.ToString();
                                }
                                if (ModbusServer_TCP.ModbusLogData[i].功能码 == 5)
                                {
                                    listBoxData = listBoxData + " ; Output Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " Output Value: ";
                                    if (ModbusServer_TCP.ModbusLogData[i].接收线圈值[0] == 0)
                                        listBoxData = listBoxData + "False";
                                    if (ModbusServer_TCP.ModbusLogData[i].接收线圈值[0] == 0xFF00)
                                        listBoxData = listBoxData + "True";
                                }
                                if (ModbusServer_TCP.ModbusLogData[i].功能码 == 6)
                                {
                                    listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " Register Value: " + ModbusServer_TCP.ModbusLogData[i].接收寄存器值[0].ToString();
                                }
                                if (ModbusServer_TCP.ModbusLogData[i].功能码 == 15)
                                {
                                    listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " Quantity: " + ModbusServer_TCP.ModbusLogData[i].数量.ToString() + " Byte Count: " + ModbusServer_TCP.ModbusLogData[i].字节数.ToString() + " Values Received: ";
                                    for (int j = 0; j < ModbusServer_TCP.ModbusLogData[i].数量; j++)
                                    {
                                        int shift = j % 16;
                                        if ((i == ModbusServer_TCP.ModbusLogData[i].数量 - 1) & (ModbusServer_TCP.ModbusLogData[i].数量 % 2 != 0))
                                        {
                                            if (shift < 8)
                                                shift = shift + 8;
                                            else
                                                shift = shift - 8;
                                        }
                                        int mask = 0x1;
                                        mask = mask << (shift);
                                        if ((ModbusServer_TCP.ModbusLogData[i].接收线圈值[j / 16] & (ushort)mask) == 0)
                                            listBoxData = listBoxData + " False";
                                        else
                                            listBoxData = listBoxData + " True";
                                    }
                                }
                                if (ModbusServer_TCP.ModbusLogData[i].功能码 == 16)
                                {
                                    listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " Quantity: " + ModbusServer_TCP.ModbusLogData[i].数量.ToString() + " Byte Count: " + ModbusServer_TCP.ModbusLogData[i].字节数.ToString() + " Values Received: ";
                                    for (int j = 0; j < ModbusServer_TCP.ModbusLogData[i].数量; j++)
                                    {
                                        listBoxData = listBoxData + " " + ModbusServer_TCP.ModbusLogData[i].接收寄存器值[j];
                                    }
                                }
                                if (ModbusServer_TCP.ModbusLogData[i].功能码 == 23)
                                {
                                    listBoxData = listBoxData + " ; Starting Address Read: " + ModbusServer_TCP.ModbusLogData[i].读起始地址.ToString() + " ; Quantity Read: " + ModbusServer_TCP.ModbusLogData[i].读数量.ToString() + " ; Starting Address Write: " + ModbusServer_TCP.ModbusLogData[i].写起始地址.ToString() + " ; Quantity Write: " + ModbusServer_TCP.ModbusLogData[i].写数量.ToString() + " ; Byte Count: " + ModbusServer_TCP.ModbusLogData[i].字节数.ToString() + " ; Values Received: ";
                                    for (int j = 0; j < ModbusServer_TCP.ModbusLogData[i].写数量; j++)
                                    {
                                        listBoxData = listBoxData + " " + ModbusServer_TCP.ModbusLogData[i].接收寄存器值[j];
                                    }
                                }

                                协议信息_列表框.Items.Add(listBoxData);
                            }
                            if (ModbusServer_TCP.ModbusLogData[i].响应)
                            {
                                if (ModbusServer_TCP.ModbusLogData[i].异常码 > 0)
                                {
                                    listBoxData = ModbusServer_TCP.ModbusLogData[i].时间戳.ToString("H:mm:ss.ff");
                                    listBoxData = listBoxData + (" Response To Client - Error code: " + Convert.ToString(ModbusServer_TCP.ModbusLogData[i].错误代码, 16));
                                    listBoxData = listBoxData + " Exception Code: " + ModbusServer_TCP.ModbusLogData[i].异常码.ToString();
                                    协议信息_列表框.Items.Add(listBoxData);
                                }
                                else
                                {
                                    listBoxData = (ModbusServer_TCP.ModbusLogData[i].时间戳.ToString("H:mm:ss.ff") + " Response To Client - Functioncode: " + ModbusServer_TCP.ModbusLogData[i].功能码.ToString());

                                    if (ModbusServer_TCP.ModbusLogData[i].功能码 <= 4)
                                    {
                                        listBoxData = listBoxData + " ; Bytecount: " + ModbusServer_TCP.ModbusLogData[i].字节数.ToString() + " ; Send values: ";
                                    }
                                    if (ModbusServer_TCP.ModbusLogData[i].功能码 == 5)
                                    {
                                        listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " ; Output Value: ";
                                        if (ModbusServer_TCP.ModbusLogData[i].接收线圈值[0] == 0)
                                            listBoxData = listBoxData + "False";
                                        if (ModbusServer_TCP.ModbusLogData[i].接收线圈值[0] == 0xFF00)
                                            listBoxData = listBoxData + "True";
                                    }
                                    if (ModbusServer_TCP.ModbusLogData[i].功能码 == 6)
                                    {
                                        listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " ; Register Value: " + ModbusServer_TCP.ModbusLogData[i].接收寄存器值[0].ToString();
                                    }
                                    if (ModbusServer_TCP.ModbusLogData[i].功能码 == 15)
                                    {
                                        listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " ; Quantity: " + ModbusServer_TCP.ModbusLogData[i].数量.ToString();
                                    }
                                    if (ModbusServer_TCP.ModbusLogData[i].功能码 == 16)
                                    {
                                        listBoxData = listBoxData + " ; Starting Address: " + ModbusServer_TCP.ModbusLogData[i].起始地址.ToString() + " ; Quantity: " + ModbusServer_TCP.ModbusLogData[i].数量.ToString();
                                    }
                                    if (ModbusServer_TCP.ModbusLogData[i].功能码 == 23)
                                    {
                                        listBoxData = listBoxData + " ; ByteCount: " + ModbusServer_TCP.ModbusLogData[i].字节数.ToString() + " ; Send Register Values: ";
                                    }
                                    if (ModbusServer_TCP.ModbusLogData[i].发送线圈值 != null)
                                    {
                                        for (int j = 0; j < ModbusServer_TCP.ModbusLogData[i].发送线圈值.Length; j++)
                                        {
                                            listBoxData = listBoxData + ModbusServer_TCP.ModbusLogData[i].发送线圈值[j].ToString() + " ";
                                        }
                                    }
                                    if (ModbusServer_TCP.ModbusLogData[i].发送寄存器值 != null)
                                    {
                                        for (int j = 0; j < ModbusServer_TCP.ModbusLogData[i].发送寄存器值.Length; j++)
                                        {
                                            listBoxData = listBoxData + ModbusServer_TCP.ModbusLogData[i].发送寄存器值[j].ToString() + " ";
                                        }
                                    }
                                    协议信息_列表框.Items.Add(listBoxData);
                                }
                            }
                        }
                    }
                    catch (Exception) { }

                    locked = false;
                }
            }
        }
        private void 显示协议信息_选择框_更改(object sender, EventArgs e)
        {
            if (显示协议信息_选择框.Checked)
                显示协议信息 = true;
            else
                显示协议信息 = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)      
        {
            ModbusServer_TCP.停止侦听();
            Environment.Exit(0);
        }

        #region 数据网格视图-事件
        private void 离散输入_数据网格视图_单元格双击(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex = 离散输入_数据网格视图.SelectedCells[0].RowIndex;
            if (ModbusServer_TCP.离散输入[rowindex + 离散输入起始地址] == false)
                ModbusServer_TCP.离散输入[rowindex + 离散输入起始地址] = true;
            else
                ModbusServer_TCP.离散输入[rowindex + 离散输入起始地址] = false;
            tabControl1_SelectedIndexChanged(null, null);

        }
        private void 线圈_数据网格视图_单元格双击(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex = 线圈_数据网格视图.SelectedCells[0].RowIndex;
            if (ModbusServer_TCP.线圈[rowindex + 线圈起始地址] == false)
                ModbusServer_TCP.线圈[rowindex + 线圈起始地址] = true;
            else
                ModbusServer_TCP.线圈[rowindex + 线圈起始地址] = false;
            tabControl1_SelectedIndexChanged(null, null);

        }
        private void 输入寄存器_数据网格视图_单元格值变更(object sender, DataGridViewCellEventArgs e)
        {
            if (输入寄存器_数据网格视图.SelectedCells.Count > 0)
            {
                int rowindex = 输入寄存器_数据网格视图.SelectedCells[0].RowIndex;
                try
                {
                    ModbusServer_TCP.输入寄存器[rowindex + 输入寄存器起始地址] = Int16.Parse(输入寄存器_数据网格视图.SelectedCells[0].Value.ToString());
                }
                catch (Exception) { }
                tabControl1_SelectedIndexChanged(null, null);
            }
        }
        private void 保持寄存器_数据网格视图_单元格值变更(object sender, DataGridViewCellEventArgs e)
        {
            if (保持寄存器_数据网格视图.SelectedCells.Count > 0)
            {
                int rowindex = 保持寄存器_数据网格视图.SelectedCells[0].RowIndex;
                try
                {
                    ModbusServer_TCP.保持寄存器[rowindex + 保持寄存器起始地址] = Int16.Parse(保持寄存器_数据网格视图.SelectedCells[0].Value.ToString());
                }
                catch (Exception) { }
                tabControl1_SelectedIndexChanged(null, null);
            }
        }
        private void 线圈_数据网格视图_鼠标进入(object sender, EventArgs e) { 禁止调用线圈 = true; }
        private void 线圈_数据网格视图_鼠标离开(object sender, EventArgs e) { 禁止调用线圈 = false; }
        private void 保持寄存器_数据网格视图_鼠标进入(object sender, EventArgs e) { 禁止调用保持寄存器 = true; }
        private void 保持寄存器_数据网格视图_鼠标离开(object sender, EventArgs e) { 禁止调用保持寄存器 = false; }

        private void 离散输入_滚动条_值改变(object sender, EventArgs e)
        {
            离散输入起始地址 = (ushort)离散输入_滚动条.Value;
            tabControl1_SelectedIndexChanged(null, null);
        }
        private void 线圈_滚动条_值改变(object sender, EventArgs e)
        {
            线圈起始地址 = (ushort)线圈_滚动条.Value;
            tabControl1_SelectedIndexChanged(null, null);
        }
        private void 输入寄存器_滚动条_值改变(object sender, EventArgs e)
        {
            输入寄存器起始地址 = (ushort)输入寄存器_滚动条.Value;
            tabControl1_SelectedIndexChanged(null, null);
        }
        private void 保持寄存器_滚动条_值改变(object sender, EventArgs e)
        {
            保持寄存器起始地址 = (ushort)保持寄存器_滚动条.Value;
            tabControl1_SelectedIndexChanged(null, null);
        }
        #endregion

        #region 功能码选择

        private void 读线圈选择框_更改(object sender, EventArgs e)
        {
            ModbusServer_TCP.FC01_Enable = !读线圈选择框.Checked;
        }
        private void 读离散输入选择框_更改(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            ModbusServer_TCP.FC02_Enable = !读离散输入选择框.Checked;
        }
        private void 读保持寄存器选择框_更改(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            ModbusServer_TCP.FC03_Enable = !读保持寄存器选择框.Checked;
        }
        private void 读输入寄存器选择框_更改(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            ModbusServer_TCP.FC04_Enable = !读输入寄存器选择框.Checked;
        }
        private void 写单线圈选择框_更改(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            ModbusServer_TCP.FC05_Enable = !写单线圈选择框.Checked;
        }
        private void 写单寄存器选择框_更改(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            ModbusServer_TCP.FC06_Enable = !写单寄存器选择框.Checked;
        }
        private void 写多线圈选择框_更改(object sender, EventArgs e)
        {
            ModbusServer_TCP.FC15_Enable = !写多线圈选择框.Checked;
        }
        private void 写多寄存器选择框_更改(object sender, EventArgs e)
        {
            ModbusServer_TCP.FC16_Enable = !写多寄存器选择框.Checked;
        }
        private void 读写多寄存器选择框_更改(object sender, EventArgs e)
        {
            ModbusServer_TCP.FC23_Enable = !读写多寄存器选择框.Checked;
        }
        #endregion

        #region 显示协议信息
        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            if (显示协议信息_选择框.Checked)
                显示协议信息 = true;
            else
                显示协议信息 = false;
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
          显示协议信息 = false;
        }

        private int X_最后位置;
        private int Y_最后位置;
        private void 协议信息_列表框_鼠标移动(object sender, MouseEventArgs e)//鼠标移动显示提示
        {
            if ((Math.Abs(e.Location.X - X_最后位置) < 50) & (Math.Abs(e.Location.Y - Y_最后位置) < 50))
                return;
            X_最后位置 = e.Location.X;
            Y_最后位置 = e.Location.Y;

            显示协议信息 = false;
            string strToolTip = "";

            //Get the item
            int nIdx = 协议信息_列表框.IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < 协议信息_列表框.Items.Count))
                strToolTip = 协议信息_列表框.Items[nIdx].ToString();

            toolTip1.SetToolTip(协议信息_列表框, strToolTip);
        }
        #endregion

        #region 关于
        private void 打开链接(object sender, LinkLabelLinkClickedEventArgs e) //打开链接
        {
            System.Diagnostics.Process.Start("http://www.EasyModbusTCP.net");
        }
        private void 打开链接(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.EasyModbusTCP.net");
        }

        private void 关于_Click(object sender, EventArgs e)//关于
        {
            Info infoWindow = new Info();
            infoWindow.Show();
        }
        #endregion

        private void 连接设置属性_Click(object sender, EventArgs e)
        {
            settings.ComPort = ModbusServer_TCP.SerialPort;
            settings.SlaveAddress = ModbusServer_TCP.UnitIdentifier;
            属性设置窗口 propertryForm = new 属性设置窗口(settings);
            propertryForm.SettingsChangedEvent += new 属性设置窗口.settingsChangedEvent(设置更改);
            propertryForm.Show();
        }

        private void 设置更改()
        {
            ModbusServer_TCP.停止侦听();
            ModbusServer_TCP.Port = settings.Port;
            ModbusServer_TCP.SerialPort = settings.ComPort;
            ModbusServer_TCP.UnitIdentifier = settings.SlaveAddress;
            if (settings.ModbusTypeSelection == 连接设置类.ModbusType.ModbusUDP)
            {
                ModbusServer_TCP.UDPFlag = true;
                ModbusServer_TCP.SerialFlag = false;
                label4.Text = "...Modbus-UDP Server Listening (Port " + settings.Port + ")...";
            }
            else if (settings.ModbusTypeSelection == 连接设置类.ModbusType.ModbusTCP)
            {
                ModbusServer_TCP.UDPFlag = false;
                ModbusServer_TCP.SerialFlag = false;
                label4.Text = "...Modbus-TCP Server Listening (Port " + settings.Port + ")...";
            }
            else if (settings.ModbusTypeSelection == 连接设置类.ModbusType.ModbusRTU)
            {
                ModbusServer_TCP.UDPFlag = false;
                ModbusServer_TCP.SerialFlag = true;
                label4.Text = "...Modbus-RTU Client Listening (Com-Port: " + settings.ComPort + ")...";
            }
            ModbusServer_TCP.端口变更 = true;

            ModbusServer_TCP.侦听();
        }

        private void 启动_BT_Click(object sender, EventArgs e)
        {
            ModbusServer_TCP.侦听();
        }

        private void 停止_BT_Click(object sender, EventArgs e)
        {
            label4.ForeColor = Color.Red;
            ModbusServer_TCP.停止侦听();
        }

    }
}
