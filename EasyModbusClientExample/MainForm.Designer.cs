/*
 * Created by SharpDevelop.
 * User: srossmann
 * Date: 13.02.2015
 * Time: 11:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ModbusClient
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TextBox txtIpAddressInput;
		private System.Windows.Forms.Label IP_COM;
		private System.Windows.Forms.Label Port_ID;
        private System.Windows.Forms.TextBox txtPortInput;
		private System.Windows.Forms.Button 读线圈_BT;
		private System.Windows.Forms.Button 读取离散输入_BT;
		private System.Windows.Forms.Button 读保持寄存器_BT;
		private System.Windows.Forms.Button 读输入寄存器_BT;
		private System.Windows.Forms.TextBox 读开始地址;
		private System.Windows.Forms.Label txtStartingAddress;
		private System.Windows.Forms.Label txtNumberOfValues;
		private System.Windows.Forms.TextBox 读长度;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtIpAddressInput = new System.Windows.Forms.TextBox();
            this.IP_COM = new System.Windows.Forms.Label();
            this.Port_ID = new System.Windows.Forms.Label();
            this.txtPortInput = new System.Windows.Forms.TextBox();
            this.读线圈_BT = new System.Windows.Forms.Button();
            this.读取离散输入_BT = new System.Windows.Forms.Button();
            this.读保持寄存器_BT = new System.Windows.Forms.Button();
            this.读输入寄存器_BT = new System.Windows.Forms.Button();
            this.读开始地址 = new System.Windows.Forms.TextBox();
            this.txtStartingAddress = new System.Windows.Forms.Label();
            this.txtNumberOfValues = new System.Windows.Forms.Label();
            this.读长度 = new System.Windows.Forms.TextBox();
            this.lsbAnswerFromServer = new System.Windows.Forms.ListBox();
            this.link_Label = new System.Windows.Forms.LinkLabel();
            this.选择Modbus方式 = new System.Windows.Forms.ComboBox();
            this.串口选择框 = new System.Windows.Forms.ComboBox();
            this.txtSlaveAddressInput = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.写多个寄存器_BT = new System.Windows.Forms.Button();
            this.写多个线圈_BT = new System.Windows.Forms.Button();
            this.写单个寄存器_BT = new System.Windows.Forms.Button();
            this.写单线圈_BT = new System.Windows.Forms.Button();
            this.更改布尔值 = new System.Windows.Forms.TextBox();
            this.txtRegisterValue = new System.Windows.Forms.TextBox();
            this.lblReadOperations = new System.Windows.Forms.Label();
            this.联机_BT = new System.Windows.Forms.Button();
            this.断开_BT = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtStartingAddressOutput = new System.Windows.Forms.TextBox();
            this.lsbWriteToServer = new System.Windows.Forms.ListBox();
            this.lblParity = new System.Windows.Forms.Label();
            this.lblStopbits = new System.Windows.Forms.Label();
            this.cbbParity = new System.Windows.Forms.ComboBox();
            this.cbbStopbits = new System.Windows.Forms.ComboBox();
            this.txtBaudrate = new System.Windows.Forms.TextBox();
            this.lblBaudrate = new System.Windows.Forms.Label();
            this.txtConnectedStatus = new System.Windows.Forms.TextBox();
            this.清除1条_BT = new System.Windows.Forms.Button();
            this.清除全部_BT = new System.Windows.Forms.Button();
            this.注册寄存器_BT = new System.Windows.Forms.Button();
            this.btnPrepareCoils = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.Debug_Log_checkBox = new System.Windows.Forms.CheckBox();
            this.Error_Log_checkBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtIpAddressInput
            // 
            this.txtIpAddressInput.Location = new System.Drawing.Point(34, 51);
            this.txtIpAddressInput.Name = "txtIpAddressInput";
            this.txtIpAddressInput.Size = new System.Drawing.Size(118, 21);
            this.txtIpAddressInput.TabIndex = 0;
            this.txtIpAddressInput.Text = "192.168.14.127";
            // 
            // IP_COM
            // 
            this.IP_COM.Location = new System.Drawing.Point(34, 32);
            this.IP_COM.Name = "IP_COM";
            this.IP_COM.Size = new System.Drawing.Size(100, 13);
            this.IP_COM.TabIndex = 1;
            this.IP_COM.Text = "Server IP-Address";
            // 
            // Port_ID
            // 
            this.Port_ID.Location = new System.Drawing.Point(158, 32);
            this.Port_ID.Name = "Port_ID";
            this.Port_ID.Size = new System.Drawing.Size(73, 16);
            this.Port_ID.TabIndex = 3;
            this.Port_ID.Text = "Server Port";
            // 
            // txtPortInput
            // 
            this.txtPortInput.Location = new System.Drawing.Point(171, 51);
            this.txtPortInput.Name = "txtPortInput";
            this.txtPortInput.Size = new System.Drawing.Size(56, 21);
            this.txtPortInput.TabIndex = 2;
            this.txtPortInput.Text = "502";
            // 
            // 读线圈_BT
            // 
            this.读线圈_BT.Location = new System.Drawing.Point(12, 162);
            this.读线圈_BT.Name = "读线圈_BT";
            this.读线圈_BT.Size = new System.Drawing.Size(161, 21);
            this.读线圈_BT.TabIndex = 5;
            this.读线圈_BT.Text = "读线圈 - FC1";
            this.读线圈_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.读线圈_BT.UseVisualStyleBackColor = true;
            this.读线圈_BT.Click += new System.EventHandler(this.读线圈_BT_Click);
            // 
            // 读取离散输入_BT
            // 
            this.读取离散输入_BT.Location = new System.Drawing.Point(12, 189);
            this.读取离散输入_BT.Name = "读取离散输入_BT";
            this.读取离散输入_BT.Size = new System.Drawing.Size(161, 21);
            this.读取离散输入_BT.TabIndex = 6;
            this.读取离散输入_BT.Text = "读取离散输入 - FC2";
            this.读取离散输入_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.读取离散输入_BT.UseVisualStyleBackColor = true;
            this.读取离散输入_BT.Click += new System.EventHandler(this.读取离散输入_BT_Click);
            // 
            // 读保持寄存器_BT
            // 
            this.读保持寄存器_BT.Location = new System.Drawing.Point(12, 216);
            this.读保持寄存器_BT.Name = "读保持寄存器_BT";
            this.读保持寄存器_BT.Size = new System.Drawing.Size(161, 21);
            this.读保持寄存器_BT.TabIndex = 7;
            this.读保持寄存器_BT.Text = "读保持寄存器 - FC3";
            this.读保持寄存器_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.读保持寄存器_BT.UseVisualStyleBackColor = true;
            this.读保持寄存器_BT.Click += new System.EventHandler(this.读保持寄存器_BT_Click);
            // 
            // 读输入寄存器_BT
            // 
            this.读输入寄存器_BT.Location = new System.Drawing.Point(12, 243);
            this.读输入寄存器_BT.Name = "读输入寄存器_BT";
            this.读输入寄存器_BT.Size = new System.Drawing.Size(161, 21);
            this.读输入寄存器_BT.TabIndex = 8;
            this.读输入寄存器_BT.Text = "读输入寄存器 - FC4";
            this.读输入寄存器_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.读输入寄存器_BT.UseVisualStyleBackColor = true;
            this.读输入寄存器_BT.Click += new System.EventHandler(this.读输入寄存器_BT_Click);
            // 
            // 读开始地址
            // 
            this.读开始地址.Location = new System.Drawing.Point(259, 162);
            this.读开始地址.Name = "读开始地址";
            this.读开始地址.Size = new System.Drawing.Size(39, 21);
            this.读开始地址.TabIndex = 9;
            this.读开始地址.Text = "22";
            this.读开始地址.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtStartingAddress
            // 
            this.txtStartingAddress.Location = new System.Drawing.Point(188, 162);
            this.txtStartingAddress.Name = "txtStartingAddress";
            this.txtStartingAddress.Size = new System.Drawing.Size(64, 21);
            this.txtStartingAddress.TabIndex = 10;
            this.txtStartingAddress.Text = "开始地址:";
            this.txtStartingAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNumberOfValues
            // 
            this.txtNumberOfValues.Location = new System.Drawing.Point(196, 193);
            this.txtNumberOfValues.Name = "txtNumberOfValues";
            this.txtNumberOfValues.Size = new System.Drawing.Size(55, 21);
            this.txtNumberOfValues.TabIndex = 12;
            this.txtNumberOfValues.Text = "长度:";
            this.txtNumberOfValues.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // 读长度
            // 
            this.读长度.Location = new System.Drawing.Point(259, 193);
            this.读长度.Name = "读长度";
            this.读长度.Size = new System.Drawing.Size(39, 21);
            this.读长度.TabIndex = 11;
            this.读长度.Text = "10";
            this.读长度.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lsbAnswerFromServer
            // 
            this.lsbAnswerFromServer.FormattingEnabled = true;
            this.lsbAnswerFromServer.ItemHeight = 12;
            this.lsbAnswerFromServer.Location = new System.Drawing.Point(310, 136);
            this.lsbAnswerFromServer.Name = "lsbAnswerFromServer";
            this.lsbAnswerFromServer.Size = new System.Drawing.Size(188, 148);
            this.lsbAnswerFromServer.TabIndex = 13;
            this.lsbAnswerFromServer.DoubleClick += new System.EventHandler(this.lsbAnswerFromServer_DoubleClick);
            // 
            // link_Label
            // 
            this.link_Label.AutoSize = true;
            this.link_Label.Location = new System.Drawing.Point(260, 9);
            this.link_Label.Name = "link_Label";
            this.link_Label.Size = new System.Drawing.Size(173, 12);
            this.link_Label.TabIndex = 16;
            this.link_Label.TabStop = true;
            this.link_Label.Text = "http://www.EasyModbusTCP.net";
            this.link_Label.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_Label_LinkClicked);
            // 
            // 选择Modbus方式
            // 
            this.选择Modbus方式.FormattingEnabled = true;
            this.选择Modbus方式.Items.AddRange(new object[] {
            "ModbusTCP (Ethernet)",
            "ModbusRTU (Serial)"});
            this.选择Modbus方式.Location = new System.Drawing.Point(34, 6);
            this.选择Modbus方式.Name = "选择Modbus方式";
            this.选择Modbus方式.Size = new System.Drawing.Size(180, 20);
            this.选择Modbus方式.TabIndex = 17;
            this.选择Modbus方式.Text = "ModbusTCP (Ethernet)";
            this.选择Modbus方式.SelectedIndexChanged += new System.EventHandler(this.选择Modbus方式_SelectedIndexChanged);
            // 
            // 串口选择框
            // 
            this.串口选择框.FormattingEnabled = true;
            this.串口选择框.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8"});
            this.串口选择框.Location = new System.Drawing.Point(26, 51);
            this.串口选择框.Name = "串口选择框";
            this.串口选择框.Size = new System.Drawing.Size(121, 20);
            this.串口选择框.TabIndex = 19;
            this.串口选择框.Visible = false;
            this.串口选择框.SelectedIndexChanged += new System.EventHandler(this.串口选择框_SelectedIndexChanged);
            // 
            // txtSlaveAddressInput
            // 
            this.txtSlaveAddressInput.Location = new System.Drawing.Point(164, 51);
            this.txtSlaveAddressInput.Name = "txtSlaveAddressInput";
            this.txtSlaveAddressInput.Size = new System.Drawing.Size(56, 21);
            this.txtSlaveAddressInput.TabIndex = 21;
            this.txtSlaveAddressInput.Text = "1";
            this.txtSlaveAddressInput.Visible = false;
            this.txtSlaveAddressInput.TextChanged += new System.EventHandler(this.TxtSlaveAddressInputTextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 467);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(641, 145);
            this.textBox1.TabIndex = 22;
            // 
            // 写多个寄存器_BT
            // 
            this.写多个寄存器_BT.Location = new System.Drawing.Point(12, 418);
            this.写多个寄存器_BT.Name = "写多个寄存器_BT";
            this.写多个寄存器_BT.Size = new System.Drawing.Size(161, 21);
            this.写多个寄存器_BT.TabIndex = 30;
            this.写多个寄存器_BT.Text = "写多个寄存器 - FC16";
            this.写多个寄存器_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.写多个寄存器_BT.UseVisualStyleBackColor = true;
            this.写多个寄存器_BT.Click += new System.EventHandler(this.写多个寄存器_BT_Click);
            // 
            // 写多个线圈_BT
            // 
            this.写多个线圈_BT.Location = new System.Drawing.Point(12, 391);
            this.写多个线圈_BT.Name = "写多个线圈_BT";
            this.写多个线圈_BT.Size = new System.Drawing.Size(161, 21);
            this.写多个线圈_BT.TabIndex = 29;
            this.写多个线圈_BT.Text = "写多个线圈 - FC15";
            this.写多个线圈_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.写多个线圈_BT.UseVisualStyleBackColor = true;
            this.写多个线圈_BT.Click += new System.EventHandler(this.写多个线圈_BT_Click);
            // 
            // 写单个寄存器_BT
            // 
            this.写单个寄存器_BT.Location = new System.Drawing.Point(12, 365);
            this.写单个寄存器_BT.Name = "写单个寄存器_BT";
            this.写单个寄存器_BT.Size = new System.Drawing.Size(161, 21);
            this.写单个寄存器_BT.TabIndex = 28;
            this.写单个寄存器_BT.Text = "写单个寄存器 - FC6";
            this.写单个寄存器_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.写单个寄存器_BT.UseVisualStyleBackColor = true;
            this.写单个寄存器_BT.Click += new System.EventHandler(this.写单个寄存器_BT_Click);
            // 
            // 写单线圈_BT
            // 
            this.写单线圈_BT.Location = new System.Drawing.Point(12, 338);
            this.写单线圈_BT.Name = "写单线圈_BT";
            this.写单线圈_BT.Size = new System.Drawing.Size(161, 21);
            this.写单线圈_BT.TabIndex = 27;
            this.写单线圈_BT.Text = "写单线圈 - FC5";
            this.写单线圈_BT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.写单线圈_BT.UseVisualStyleBackColor = true;
            this.写单线圈_BT.Click += new System.EventHandler(this.写单线圈_BT_Click);
            // 
            // 更改布尔值
            // 
            this.更改布尔值.BackColor = System.Drawing.SystemColors.Info;
            this.更改布尔值.Location = new System.Drawing.Point(563, 381);
            this.更改布尔值.Name = "更改布尔值";
            this.更改布尔值.ReadOnly = true;
            this.更改布尔值.Size = new System.Drawing.Size(81, 21);
            this.更改布尔值.TabIndex = 31;
            this.更改布尔值.Text = "FALSE";
            this.更改布尔值.DoubleClick += new System.EventHandler(this.更改线圈值_DoubleClick);
            // 
            // txtRegisterValue
            // 
            this.txtRegisterValue.BackColor = System.Drawing.SystemColors.Info;
            this.txtRegisterValue.Location = new System.Drawing.Point(563, 426);
            this.txtRegisterValue.Name = "txtRegisterValue";
            this.txtRegisterValue.Size = new System.Drawing.Size(81, 21);
            this.txtRegisterValue.TabIndex = 32;
            this.txtRegisterValue.Text = "0";
            // 
            // lblReadOperations
            // 
            this.lblReadOperations.AutoSize = true;
            this.lblReadOperations.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReadOperations.Location = new System.Drawing.Point(16, 133);
            this.lblReadOperations.Name = "lblReadOperations";
            this.lblReadOperations.Size = new System.Drawing.Size(128, 20);
            this.lblReadOperations.TabIndex = 37;
            this.lblReadOperations.Text = "从服务器读取值";
            // 
            // 联机_BT
            // 
            this.联机_BT.BackColor = System.Drawing.Color.Lime;
            this.联机_BT.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.联机_BT.Location = new System.Drawing.Point(310, 87);
            this.联机_BT.Name = "联机_BT";
            this.联机_BT.Size = new System.Drawing.Size(89, 43);
            this.联机_BT.TabIndex = 38;
            this.联机_BT.Text = "联 机";
            this.联机_BT.UseVisualStyleBackColor = false;
            this.联机_BT.Click += new System.EventHandler(this.联机_BT_Click);
            // 
            // 断开_BT
            // 
            this.断开_BT.BackColor = System.Drawing.Color.Red;
            this.断开_BT.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.断开_BT.Location = new System.Drawing.Point(409, 87);
            this.断开_BT.Name = "断开_BT";
            this.断开_BT.Size = new System.Drawing.Size(89, 43);
            this.断开_BT.TabIndex = 39;
            this.断开_BT.Text = "断 开";
            this.断开_BT.UseVisualStyleBackColor = false;
            this.断开_BT.Click += new System.EventHandler(this.断开_BT_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 313);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 20);
            this.label1.TabIndex = 40;
            this.label1.Text = "将值写入服务器";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(188, 365);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 42;
            this.label4.Text = "开始地址:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStartingAddressOutput
            // 
            this.txtStartingAddressOutput.Location = new System.Drawing.Point(259, 362);
            this.txtStartingAddressOutput.Name = "txtStartingAddressOutput";
            this.txtStartingAddressOutput.Size = new System.Drawing.Size(39, 21);
            this.txtStartingAddressOutput.TabIndex = 41;
            this.txtStartingAddressOutput.Text = "1";
            this.txtStartingAddressOutput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lsbWriteToServer
            // 
            this.lsbWriteToServer.FormattingEnabled = true;
            this.lsbWriteToServer.ItemHeight = 12;
            this.lsbWriteToServer.Location = new System.Drawing.Point(310, 314);
            this.lsbWriteToServer.Name = "lsbWriteToServer";
            this.lsbWriteToServer.Size = new System.Drawing.Size(188, 148);
            this.lsbWriteToServer.TabIndex = 43;
            // 
            // lblParity
            // 
            this.lblParity.Location = new System.Drawing.Point(96, 76);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(56, 18);
            this.lblParity.TabIndex = 46;
            this.lblParity.Text = "Parity";
            this.lblParity.Visible = false;
            // 
            // lblStopbits
            // 
            this.lblStopbits.Location = new System.Drawing.Point(158, 76);
            this.lblStopbits.Name = "lblStopbits";
            this.lblStopbits.Size = new System.Drawing.Size(56, 18);
            this.lblStopbits.TabIndex = 48;
            this.lblStopbits.Text = "Stopbits";
            this.lblStopbits.Visible = false;
            // 
            // cbbParity
            // 
            this.cbbParity.FormattingEnabled = true;
            this.cbbParity.Items.AddRange(new object[] {
            "Even",
            "Odd",
            "None"});
            this.cbbParity.Location = new System.Drawing.Point(97, 93);
            this.cbbParity.Name = "cbbParity";
            this.cbbParity.Size = new System.Drawing.Size(55, 20);
            this.cbbParity.TabIndex = 50;
            this.cbbParity.Visible = false;
            // 
            // cbbStopbits
            // 
            this.cbbStopbits.FormattingEnabled = true;
            this.cbbStopbits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.cbbStopbits.Location = new System.Drawing.Point(158, 93);
            this.cbbStopbits.Name = "cbbStopbits";
            this.cbbStopbits.Size = new System.Drawing.Size(55, 20);
            this.cbbStopbits.TabIndex = 51;
            this.cbbStopbits.Visible = false;
            // 
            // txtBaudrate
            // 
            this.txtBaudrate.Location = new System.Drawing.Point(35, 94);
            this.txtBaudrate.Name = "txtBaudrate";
            this.txtBaudrate.Size = new System.Drawing.Size(56, 21);
            this.txtBaudrate.TabIndex = 53;
            this.txtBaudrate.Text = "9600";
            this.txtBaudrate.Visible = false;
            this.txtBaudrate.TextChanged += new System.EventHandler(this.txtBaudrate_TextChanged);
            // 
            // lblBaudrate
            // 
            this.lblBaudrate.Location = new System.Drawing.Point(35, 76);
            this.lblBaudrate.Name = "lblBaudrate";
            this.lblBaudrate.Size = new System.Drawing.Size(56, 18);
            this.lblBaudrate.TabIndex = 52;
            this.lblBaudrate.Text = "Baudrate";
            this.lblBaudrate.Visible = false;
            // 
            // txtConnectedStatus
            // 
            this.txtConnectedStatus.BackColor = System.Drawing.Color.Red;
            this.txtConnectedStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.txtConnectedStatus.Location = new System.Drawing.Point(3, 618);
            this.txtConnectedStatus.Name = "txtConnectedStatus";
            this.txtConnectedStatus.Size = new System.Drawing.Size(665, 32);
            this.txtConnectedStatus.TabIndex = 54;
            this.txtConnectedStatus.Text = "Not connected to Server";
            // 
            // 清除1条_BT
            // 
            this.清除1条_BT.Cursor = System.Windows.Forms.Cursors.Default;
            this.清除1条_BT.Image = global::ModbusClient.Properties.Resources.circle_minus;
            this.清除1条_BT.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.清除1条_BT.Location = new System.Drawing.Point(518, 314);
            this.清除1条_BT.Name = "清除1条_BT";
            this.清除1条_BT.Size = new System.Drawing.Size(64, 48);
            this.清除1条_BT.TabIndex = 34;
            this.清除1条_BT.Text = "清除选中";
            this.清除1条_BT.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.清除1条_BT.UseVisualStyleBackColor = true;
            this.清除1条_BT.Click += new System.EventHandler(this.清除1条_BT_Click);
            // 
            // 清除全部_BT
            // 
            this.清除全部_BT.Cursor = System.Windows.Forms.Cursors.Default;
            this.清除全部_BT.Image = global::ModbusClient.Properties.Resources.circle_delete1;
            this.清除全部_BT.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.清除全部_BT.Location = new System.Drawing.Point(585, 314);
            this.清除全部_BT.Name = "清除全部_BT";
            this.清除全部_BT.Size = new System.Drawing.Size(64, 48);
            this.清除全部_BT.TabIndex = 33;
            this.清除全部_BT.Text = "清除全部";
            this.清除全部_BT.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.清除全部_BT.UseVisualStyleBackColor = true;
            this.清除全部_BT.Click += new System.EventHandler(this.清除全部_BT_Click);
            // 
            // 注册寄存器_BT
            // 
            this.注册寄存器_BT.Image = global::ModbusClient.Properties.Resources.arrow_left;
            this.注册寄存器_BT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.注册寄存器_BT.Location = new System.Drawing.Point(507, 422);
            this.注册寄存器_BT.Name = "注册寄存器_BT";
            this.注册寄存器_BT.Size = new System.Drawing.Size(146, 40);
            this.注册寄存器_BT.TabIndex = 26;
            this.注册寄存器_BT.Text = "    添加寄存器";
            this.注册寄存器_BT.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.注册寄存器_BT.UseVisualStyleBackColor = true;
            this.注册寄存器_BT.Click += new System.EventHandler(this.添加寄存器_BT_Click);
            // 
            // btnPrepareCoils
            // 
            this.btnPrepareCoils.Image = global::ModbusClient.Properties.Resources.arrow_left;
            this.btnPrepareCoils.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrepareCoils.Location = new System.Drawing.Point(507, 377);
            this.btnPrepareCoils.Name = "btnPrepareCoils";
            this.btnPrepareCoils.Size = new System.Drawing.Size(146, 40);
            this.btnPrepareCoils.TabIndex = 25;
            this.btnPrepareCoils.Text = "    添加线圈";
            this.btnPrepareCoils.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnPrepareCoils.UseVisualStyleBackColor = true;
            this.btnPrepareCoils.Click += new System.EventHandler(this.添加线圈_BT_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ModbusClient.Properties.Resources.small;
            this.pictureBox1.Location = new System.Drawing.Point(507, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(158, 77);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(531, 162);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 35);
            this.button1.TabIndex = 55;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Debug_Log_checkBox
            // 
            this.Debug_Log_checkBox.AutoSize = true;
            this.Debug_Log_checkBox.Location = new System.Drawing.Point(259, 42);
            this.Debug_Log_checkBox.Name = "Debug_Log_checkBox";
            this.Debug_Log_checkBox.Size = new System.Drawing.Size(78, 16);
            this.Debug_Log_checkBox.TabIndex = 56;
            this.Debug_Log_checkBox.Text = "Debug_Log";
            this.Debug_Log_checkBox.UseVisualStyleBackColor = true;
            this.Debug_Log_checkBox.Click += new System.EventHandler(this.Debug_Log_checkBox_Click);
            // 
            // Error_Log_checkBox
            // 
            this.Error_Log_checkBox.AutoSize = true;
            this.Error_Log_checkBox.Location = new System.Drawing.Point(343, 42);
            this.Error_Log_checkBox.Name = "Error_Log_checkBox";
            this.Error_Log_checkBox.Size = new System.Drawing.Size(78, 16);
            this.Error_Log_checkBox.TabIndex = 57;
            this.Error_Log_checkBox.Text = "Error_Log";
            this.Error_Log_checkBox.UseVisualStyleBackColor = true;
            this.Error_Log_checkBox.CheckedChanged += new System.EventHandler(this.Error_Log_checkBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 648);
            this.Controls.Add(this.Error_Log_checkBox);
            this.Controls.Add(this.Debug_Log_checkBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtConnectedStatus);
            this.Controls.Add(this.txtBaudrate);
            this.Controls.Add(this.lblBaudrate);
            this.Controls.Add(this.cbbStopbits);
            this.Controls.Add(this.cbbParity);
            this.Controls.Add(this.lblStopbits);
            this.Controls.Add(this.lblParity);
            this.Controls.Add(this.lsbWriteToServer);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtStartingAddressOutput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.断开_BT);
            this.Controls.Add(this.联机_BT);
            this.Controls.Add(this.lblReadOperations);
            this.Controls.Add(this.清除1条_BT);
            this.Controls.Add(this.清除全部_BT);
            this.Controls.Add(this.txtRegisterValue);
            this.Controls.Add(this.更改布尔值);
            this.Controls.Add(this.写多个寄存器_BT);
            this.Controls.Add(this.写多个线圈_BT);
            this.Controls.Add(this.写单个寄存器_BT);
            this.Controls.Add(this.写单线圈_BT);
            this.Controls.Add(this.注册寄存器_BT);
            this.Controls.Add(this.btnPrepareCoils);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txtSlaveAddressInput);
            this.Controls.Add(this.串口选择框);
            this.Controls.Add(this.选择Modbus方式);
            this.Controls.Add(this.link_Label);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lsbAnswerFromServer);
            this.Controls.Add(this.txtNumberOfValues);
            this.Controls.Add(this.读长度);
            this.Controls.Add(this.txtStartingAddress);
            this.Controls.Add(this.读开始地址);
            this.Controls.Add(this.读输入寄存器_BT);
            this.Controls.Add(this.读保持寄存器_BT);
            this.Controls.Add(this.读取离散输入_BT);
            this.Controls.Add(this.读线圈_BT);
            this.Controls.Add(this.Port_ID);
            this.Controls.Add(this.txtPortInput);
            this.Controls.Add(this.IP_COM);
            this.Controls.Add(this.txtIpAddressInput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Modbus客户端";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private System.Windows.Forms.ListBox lsbAnswerFromServer;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel link_Label;
        private System.Windows.Forms.ComboBox 选择Modbus方式;
        private System.Windows.Forms.ComboBox 串口选择框;
        private System.Windows.Forms.TextBox txtSlaveAddressInput;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnPrepareCoils;
        private System.Windows.Forms.Button 注册寄存器_BT;
        private System.Windows.Forms.Button 写多个寄存器_BT;
        private System.Windows.Forms.Button 写多个线圈_BT;
        private System.Windows.Forms.Button 写单个寄存器_BT;
        private System.Windows.Forms.Button 写单线圈_BT;
        private System.Windows.Forms.TextBox 更改布尔值;
        private System.Windows.Forms.TextBox txtRegisterValue;
        private System.Windows.Forms.Button 清除全部_BT;
        private System.Windows.Forms.Button 清除1条_BT;
        private System.Windows.Forms.Label lblReadOperations;
        private System.Windows.Forms.Button 联机_BT;
        private System.Windows.Forms.Button 断开_BT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtStartingAddressOutput;
        private System.Windows.Forms.ListBox lsbWriteToServer;
        private System.Windows.Forms.Label lblParity;
        private System.Windows.Forms.Label lblStopbits;
        private System.Windows.Forms.ComboBox cbbParity;
        private System.Windows.Forms.ComboBox cbbStopbits;
        private System.Windows.Forms.TextBox txtBaudrate;
        private System.Windows.Forms.Label lblBaudrate;
        private System.Windows.Forms.TextBox txtConnectedStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox Debug_Log_checkBox;
        private System.Windows.Forms.CheckBox Error_Log_checkBox;
    }
}
