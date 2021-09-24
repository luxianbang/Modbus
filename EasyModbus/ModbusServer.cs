/*
Copyright (c) 2018-2020 Rossmann-Engineering
Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software
and associated documentation files (the "Software"),
to deal in the Software without restriction, 
including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit 
persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission 
notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using System.IO.Ports;

namespace EasyModbus
{
    #region Modbus协议信息-类
    /// <summary> Modbus协议信息
    /// Modbus Protocol informations.
    /// </summary>
    public class ModbusProtocol
    {
    	public enum ProtocolType { ModbusTCP = 0, ModbusUDP = 1, ModbusRTU = 2};
        public DateTime 时间戳;//timeStamp
        public bool     请求;//request
        public bool     响应;//response
        public UInt16   事务标识符;//transactionIdentifier
        public UInt16   协议标识符;//protocolIdentifier
        public UInt16   长度;//length
        public byte     单元标识符;//unitIdentifier
        public byte     功能码;//functionCode
        public UInt16   起始地址;//startingAdress
        public UInt16   读起始地址;//startingAddressRead
        public UInt16   写起始地址;//startingAddressWrite
        public UInt16   数量;//quantity
        public UInt16   读数量;//quantityRead
        public UInt16   写数量;//quantityWrite
        public byte     字节数;//byteCount
        public byte     异常码;//exceptionCode
        public byte     错误代码;// errorCode
        public UInt16[] 接收线圈值;//receiveCoilValues
        public UInt16[] 接收寄存器值;//receiveRegisterValues
        public Int16[]  发送寄存器值;//sendRegisterValues
        public bool[]   发送线圈值;    //sendCoilValues
        public UInt16   crc;//
    }
    #endregion

    #region 结构
    struct NetworkConnectionParameter
    {
        public NetworkStream Net基础流;     //仅适用于TCP连接
        public Byte[] bytes;                            //字节
        public int portIn;                                //仅适用于UDP连接
        public IPAddress ipAddressIn;           //仅适用于UDP连接
    }
    #endregion

    #region TCP处理程序-类
    internal class TCPHandler
    {
        public delegate void DataChanged(object 连接参数);
        public event DataChanged 数据更改事件;// dataChanged

        public delegate void NumberOfClientsChanged();
        public event NumberOfClientsChanged 客户数更改事件; // numberOfClientsChanged

        TcpListener 服务器端 = null;

        private List<Client> 客户端上次请求列表 = new List<Client>();//tcp客户端上次请求列表 tcpClientLastRequestList

        public int 客户端连接数 { get; set; }// NumberOfConnectedClients

        public string ipAddress = null;

        /// <summary>当制作服务器TCP侦听套接字时，将侦听此IP地址 When making a server TCP listen socket, will listen to this IP address. <summary>
        public IPAddress LocalIPAddress { get { return localIPAddress; }  }
        private IPAddress localIPAddress = IPAddress.Any;

        /// <summary> 收听所有网络接口 Listen to all network interfaces </summary>
        /// <param name="port">TCP port to listen</param>
        public TCPHandler(int port)
        {
            服务器端 = new TcpListener(LocalIPAddress, port);
            服务器端.Start();
            服务器端.BeginAcceptTcpClient(接受客户端回调, null);
        }

        /// <summary> 收听特定的网络接口  Listen to a specific network interface </summary>
        /// <param name="localIPAddress">IP address of network interface to listen</param>
        /// <param name="port">TCP port to listen</param>
        public TCPHandler(IPAddress localIPAddress, int port)//TCP处理程序
        {
            this.localIPAddress = localIPAddress;
            服务器端 = new TcpListener(LocalIPAddress, port);
            服务器端.Start();
            服务器端.BeginAcceptTcpClient(接受客户端回调, null);
        }


        private void 接受客户端回调(IAsyncResult asyncResult)//AcceptTcpClientCallback
        {
            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient = 服务器端.EndAcceptTcpClient(asyncResult);
                tcpClient.ReceiveTimeout = 4000;
                if (ipAddress != null)
                {
                    string ipEndpoint = tcpClient.Client.RemoteEndPoint.ToString();
                    ipEndpoint = ipEndpoint.Split(':')[0];
                    if (ipEndpoint != ipAddress)
                    {
                        tcpClient.Client.Disconnect(false);
                        return;
                    }
                }
            }
            catch (Exception) { }

            try
            {
                服务器端.BeginAcceptTcpClient(接受客户端回调, null);
                Client client = new Client(tcpClient);
                NetworkStream networkStream = client.NetworkStream;
                networkStream.ReadTimeout = 4000;
                networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, 读取回调, client);
            }
            catch (Exception) { }
        }
 
        private int 获取并清除已连接客户端数量(Client client)//GetAndCleanNumberOfConnectedClients
        {
            lock (this)
            {
                bool objetExists = false;
                foreach (Client clientLoop in 客户端上次请求列表)
                {
                    if (client.Equals(clientLoop))
                        objetExists = true;
                }

                try
                {
                    客户端上次请求列表.RemoveAll(delegate (Client c)
                    {
                        return ((DateTime.Now.Ticks - c.Ticks) > 40000000);//4S
                    }
                       );
                }
                catch (Exception) { }

                if (!objetExists)
                    客户端上次请求列表.Add(client);

                return 客户端上次请求列表.Count;
            }
        }

        private void 读取回调( IAsyncResult 异步结果)  // ReadCallback
        {
            NetworkConnectionParameter 网络连接参数 = new NetworkConnectionParameter();// networkConnectionParameter
            Client client = 异步结果.AsyncState as Client;
            client.Ticks = DateTime.Now.Ticks;
            客户端连接数 = 获取并清除已连接客户端数量(client);
            if (客户数更改事件 != null)
                客户数更改事件();
            if (client != null)
            {
                int read;
                NetworkStream Net基础流 = null;
                try
                {
                    Net基础流 = client.NetworkStream;
                    read = Net基础流.EndRead(异步结果);
                }
                catch (Exception)  { return; }
                 
                if (read == 0)
                {
                    //OnClientDisconnected(client.TcpClient);
                    //connectedClients.Remove(client);
                    return;
                }
                byte[] data = new byte[read];
                Buffer.BlockCopy(client.Buffer, 0, data, 0, read);
                网络连接参数.bytes = data;
                网络连接参数.Net基础流 = Net基础流;
                if (数据更改事件 != null)
                    数据更改事件(网络连接参数);

                try  {  Net基础流.BeginRead(client.Buffer, 0, client.Buffer.Length, 读取回调, client);  }
                catch (Exception)  {  }
            }
        }

        /// <summary> 关闭连接 </summary>
        public void Disconnect()
        {
            try
            {
                foreach (Client 客户端循环 in 客户端上次请求列表)
                    客户端循环.NetworkStream.Close(00);
            }
            catch (Exception) { }
            服务器端.Stop();  
        }

        /// <summary> 内部的连接类 </summary>
        internal class Client
        {
            private readonly TcpClient tcpClient;
            private readonly byte[] buffer;
            public long Ticks { get; set; }

            public Client(TcpClient tcpClient)
            {
                this.tcpClient = tcpClient;
                int bufferSize = tcpClient.ReceiveBufferSize;
                buffer = new byte[bufferSize];
            }

            public TcpClient TcpClient  { get { return tcpClient; }  }

            public byte[] Buffer  { get { return buffer; }  }

            public NetworkStream NetworkStream   {  get { return tcpClient.GetStream();  }  }
        }
    }
    #endregion

    /// <summary> Modbus TCP Server </summary>
    public class ModbusServer
    {
        private bool debug = false;
        Int32 port = 8000;
        //ModbusProtocol receiveData;
        ModbusProtocol sendData =  new ModbusProtocol();
        Byte[] bytes = new Byte[2100];
        //public Int16[] _holdingRegisters = new Int16[65535];
        public HoldingRegisters 保持寄存器;      
        public InputRegisters 输入寄存器;
        public Coils 线圈;
        public DiscreteInputs 离散输入;
        private int 连接数 = 0;
        private bool UDP标志;
        private bool 串口标志;
        private int baudrate = 9600;
        private System.IO.Ports.Parity parity = Parity.Even;
        private System.IO.Ports.StopBits stopBits = StopBits.One;
        private string serialPort = "COM1";
        private SerialPort 串口;//串口
        private byte 单元标识符 = 1;
        private int portIn;
        private IPAddress ipAddressIn;
        private UdpClient udpClient;
        private IPEndPoint iPEndPoint;
        private TCPHandler tcpHandler;
        Thread  侦听器线程;
        Thread 客户端连接线程 = null;// clientConnectionThread
        private ModbusProtocol[] 日志数据 = new ModbusProtocol[100];//Modbus日志数据

        #region 功能码-开关
        public bool FC01_Enable { get; set; }//读线圈-功能选择 ReadCoil_Enable
        public bool FC02_Enable { get; set; }//
        public bool FC03_Enable { get; set; }
        public bool FC04_Enable { get; set; }
        public bool FC05_Enable { get; set; }
        public bool FC06_Enable { get; set; }
        public bool FC15_Enable { get; set; }
        public bool FC16_Enable { get; set; }
        public bool FC23_Enable { get; set; }
        #endregion

        public bool 端口变更 { get; set; }// PortChanged
        object lockCoils = new object();//锁定线圈
        object lockHoldingRegisters = new object();//锁定保持寄存器
        private volatile bool 运行标志;// shouldStop

        private IPAddress 本机IP = IPAddress.Any;

        /// <summary> 创建TCP或UDP套接字时，要附加到的本地IP地址
        /// When creating a TCP or UDP socket, the local IP address to attach to.
        /// </summary>
        public IPAddress LocalIPAddress
        {
            get { return 本机IP; }
            set { if (侦听器线程 == null) 本机IP = value; }
        }

        public ModbusServer()
        {
            保持寄存器 = new HoldingRegisters(this);
            输入寄存器 = new InputRegisters(this);
            线圈 = new Coils(this);
            离散输入 = new DiscreteInputs(this);
        }

        #region events
        public delegate void CoilsChangedHandler(int coil, int numberOfCoils);
        public event CoilsChangedHandler CoilsChanged;

        public delegate void HoldingRegistersChangedHandler(int register, int numberOfRegisters);
        public event HoldingRegistersChangedHandler HoldingRegistersChanged;

        public delegate void NumberOfConnectedClientsChangedHandler();
        public event NumberOfConnectedClientsChangedHandler NumberOfConnectedClientsChanged;

        public delegate void LogDataChangedHandler();
        public event LogDataChangedHandler LogDataChanged;
        #endregion

        public void 侦听()
        {
            侦听器线程 = new Thread(侦听器线程FC);
            侦听器线程.Start();
        }

        public void 停止侦听()
        {
            //串口
            if (串口标志 & (串口 != null))
        	{
        		if (串口.IsOpen)
        			串口.Close();
                运行标志 = false;
            }
           //TCP
            try
            {
                if (tcpHandler != null) {tcpHandler.Disconnect(); }
                if (侦听器线程 != null) { 侦听器线程.Abort(); }
            } catch (Exception) { }
            if (侦听器线程 != null) {侦听器线程.Join(); }
          
            // 客户端连接线程
            try
            {
                if (客户端连接线程 != null){ 客户端连接线程.Abort(); }     
            } catch (Exception) { }
        }
        
        private void 侦听器线程FC()
        {
            //if (运行标志) return;

            if (!UDP标志 & !串口标志)
            {
                if (udpClient != null)//
                {
                    try  {  udpClient.Close();  }  catch (Exception) { }
                }
                tcpHandler = new TCPHandler(LocalIPAddress, port);
                tcpHandler.数据更改事件 += new TCPHandler.DataChanged(处理接收数据);
                tcpHandler.客户数更改事件 += new TCPHandler.NumberOfClientsChanged(客户数量更改);

                if (debug) StoreLogData.Instance.Store($"Modbus Server listing for incomming data at Port {port}, local IP {LocalIPAddress}", System.DateTime.Now);
            }
            else if (串口标志)
            {
                if (串口 == null)
                {
                    if (debug) StoreLogData.Instance.Store("EasyModbus RTU-Server listing for incomming data at Serial Port " + serialPort, System.DateTime.Now);
                    串口 = new SerialPort();
                    串口.PortName = serialPort;
                    串口.BaudRate = this.baudrate;
                    串口.Parity = this.parity;
                    串口.StopBits = stopBits;
                    串口.WriteTimeout = 1000;
                    串口.ReadTimeout = 1000;
                    串口.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    串口.Open();
                }
            }
            else
                while (运行标志)
                {
                    if (UDP标志)
                    {
                        if (udpClient == null | 端口变更)
                        {
                            端口变更 = false;
                            IPEndPoint localEndoint = new IPEndPoint(LocalIPAddress, port);
                            udpClient = new UdpClient(localEndoint);
                            udpClient.Client.ReceiveTimeout = 1000;
                            iPEndPoint = new IPEndPoint(IPAddress.Any, port);

                            if (debug) StoreLogData.Instance.Store($"EasyModbus Server listing for incomming data at Port {port}, local IP {LocalIPAddress}", System.DateTime.Now);
                        }
                        if (tcpHandler != null)
                            tcpHandler.Disconnect();
                        try
                        {
                            bytes = udpClient.Receive(ref iPEndPoint);
                            portIn = iPEndPoint.Port;
                            NetworkConnectionParameter networkConnectionParameter = new NetworkConnectionParameter();
                            networkConnectionParameter.bytes = bytes;
                            ipAddressIn = iPEndPoint.Address;
                            networkConnectionParameter.portIn = portIn;
                            networkConnectionParameter.ipAddressIn = ipAddressIn;
                            ParameterizedThreadStart pts = new ParameterizedThreadStart(this.处理接收数据);
                            Thread processDataThread = new Thread(pts);
                            processDataThread.Start(networkConnectionParameter);
                        }
                        catch (Exception)  { }
                    }
                }
        }
    
		#region SerialHandler
        private bool dataReceived = false;
        private byte[] readBuffer = new byte[2094];
        private DateTime lastReceive;
        private int nextSign = 0;
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int silence = 4000 / baudrate;
            if ((DateTime.Now.Ticks - lastReceive.Ticks) > TimeSpan.TicksPerMillisecond*silence)
                nextSign = 0;


            SerialPort sp = (SerialPort)sender;

            int numbytes = sp.BytesToRead;
            byte[] rxbytearray = new byte[numbytes];

            sp.Read(rxbytearray, 0, numbytes);
            
            Array.Copy(rxbytearray, 0,  readBuffer, nextSign, rxbytearray.Length);
            lastReceive= DateTime.Now;
            nextSign = numbytes+ nextSign;
            if (ModbusClient.检测有效帧(readBuffer, nextSign))
            {
                
                dataReceived = true;
                nextSign= 0;

                    NetworkConnectionParameter networkConnectionParameter = new NetworkConnectionParameter();
                    networkConnectionParameter.bytes = readBuffer;
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(this.处理接收数据);
                    Thread processDataThread = new Thread(pts);
                    processDataThread.Start(networkConnectionParameter);
                    dataReceived = false;
                
            }
            else
                dataReceived = false;
        }
		#endregion
 
		#region Method numberOfClientsChanged
        private void 客户数量更改()
        {
            连接数 = tcpHandler.客户端连接数;
            if (NumberOfConnectedClientsChanged != null)
                NumberOfConnectedClientsChanged();
        }
        #endregion

        object lockProcessReceivedData = new object();
        #region Method ProcessReceivedData
        private void 处理接收数据(object networkConnectionParameter)
        {
            lock (lockProcessReceivedData)
            {
                Byte[] bytes = new byte[((NetworkConnectionParameter)networkConnectionParameter).bytes.Length];
                if (debug) StoreLogData.Instance.Store("Received Data: " + BitConverter.ToString(bytes), System.DateTime.Now);
                NetworkStream stream = ((NetworkConnectionParameter)networkConnectionParameter).Net基础流;
                int portIn = ((NetworkConnectionParameter)networkConnectionParameter).portIn;
                IPAddress ipAddressIn = ((NetworkConnectionParameter)networkConnectionParameter).ipAddressIn;


                Array.Copy(((NetworkConnectionParameter)networkConnectionParameter).bytes, 0, bytes, 0, ((NetworkConnectionParameter)networkConnectionParameter).bytes.Length);

                ModbusProtocol receiveDataThread = new ModbusProtocol();
                ModbusProtocol sendDataThread = new ModbusProtocol();

                try
                {
                    UInt16[] wordData = new UInt16[1];
                    byte[] byteData = new byte[2];
                    receiveDataThread.时间戳 = DateTime.Now;
                    receiveDataThread.请求 = true;
                    if (!串口标志)
                    {
                        //Lese Transaction identifier
                        byteData[1] = bytes[0];
                        byteData[0] = bytes[1];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.事务标识符 = wordData[0];

                        //Lese Protocol identifier
                        byteData[1] = bytes[2];
                        byteData[0] = bytes[3];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.协议标识符 = wordData[0];

                        //Lese 长度
                        byteData[1] = bytes[4];
                        byteData[0] = bytes[5];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.长度 = wordData[0];
                    }

                    //Lese unit identifier
                    receiveDataThread.单元标识符 = bytes[6 - 6 * Convert.ToInt32(串口标志)];
                    //Check UnitIdentifier
                    if ((receiveDataThread.单元标识符 != this.单元标识符) & (receiveDataThread.单元标识符 != 0))
                        return;

                    // Lese function code
                    receiveDataThread.功能码 = bytes[7 - 6 * Convert.ToInt32(串口标志)];

                    // Lese starting address 
                    byteData[1] = bytes[8 - 6 * Convert.ToInt32(串口标志)];
                    byteData[0] = bytes[9 - 6 * Convert.ToInt32(串口标志)];
                    Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                    receiveDataThread.起始地址 = wordData[0];

                    if (receiveDataThread.功能码 <= 4)
                    {
                        // Lese 数量
                        byteData[1] = bytes[10 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[11 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.数量 = wordData[0];
                    }
                    if (receiveDataThread.功能码 == 5)
                    {
                        receiveDataThread.接收线圈值 = new ushort[1];
                        // Lese Value
                        byteData[1] = bytes[10 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[11 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, receiveDataThread.接收线圈值, 0, 2);
                    }
                    if (receiveDataThread.功能码 == 6)
                    {
                        receiveDataThread.接收寄存器值 = new ushort[1];
                        // Lese Value
                        byteData[1] = bytes[10 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[11 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, receiveDataThread.接收寄存器值, 0, 2);
                    }
                    if (receiveDataThread.功能码 == 15)
                    {
                        // Lese 数量
                        byteData[1] = bytes[10 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[11 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.数量 = wordData[0];

                        receiveDataThread.字节数 = bytes[12 - 6 * Convert.ToInt32(串口标志)];

                        if ((receiveDataThread.字节数 % 2) != 0)
                            receiveDataThread.接收线圈值 = new ushort[receiveDataThread.字节数 / 2 + 1];
                        else
                            receiveDataThread.接收线圈值 = new ushort[receiveDataThread.字节数 / 2];
                        // Lese Value
                        Buffer.BlockCopy(bytes, 13 - 6 * Convert.ToInt32(串口标志), receiveDataThread.接收线圈值, 0, receiveDataThread.字节数);
                    }
                    if (receiveDataThread.功能码 == 16)
                    {
                        // Lese 数量
                        byteData[1] = bytes[10 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[11 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.数量 = wordData[0];

                        receiveDataThread.字节数 = bytes[12 - 6 * Convert.ToInt32(串口标志)];
                        receiveDataThread.接收寄存器值 = new ushort[receiveDataThread.数量];
                        for (int i = 0; i < receiveDataThread.数量; i++)
                        {
                            // Lese Value
                            byteData[1] = bytes[13 + i * 2 - 6 * Convert.ToInt32(串口标志)];
                            byteData[0] = bytes[14 + i * 2 - 6 * Convert.ToInt32(串口标志)];
                            Buffer.BlockCopy(byteData, 0, receiveDataThread.接收寄存器值, i * 2, 2);
                        }

                    }
                    if (receiveDataThread.功能码 == 23)
                    {
                        // Lese starting Address Read
                        byteData[1] = bytes[8 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[9 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.读起始地址 = wordData[0];
                        // Lese 数量 Read
                        byteData[1] = bytes[10 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[11 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.读数量 = wordData[0];
                        // Lese starting Address Write
                        byteData[1] = bytes[12 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[13 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.写起始地址 = wordData[0];
                        // Lese 数量 Write
                        byteData[1] = bytes[14 - 6 * Convert.ToInt32(串口标志)];
                        byteData[0] = bytes[15 - 6 * Convert.ToInt32(串口标志)];
                        Buffer.BlockCopy(byteData, 0, wordData, 0, 2);
                        receiveDataThread.写数量 = wordData[0];

                        receiveDataThread.字节数 = bytes[16 - 6 * Convert.ToInt32(串口标志)];
                        receiveDataThread.接收寄存器值 = new ushort[receiveDataThread.写数量];
                        for (int i = 0; i < receiveDataThread.写数量; i++)
                        {
                            // Lese Value
                            byteData[1] = bytes[17 + i * 2 - 6 * Convert.ToInt32(串口标志)];
                            byteData[0] = bytes[18 + i * 2 - 6 * Convert.ToInt32(串口标志)];
                            Buffer.BlockCopy(byteData, 0, receiveDataThread.接收寄存器值, i * 2, 2);
                        }
                    }
                }
                catch (Exception)
                { }
                this.CreateAnswer(receiveDataThread, sendDataThread, stream, portIn, ipAddressIn);
                //this.sendAnswer();
                this.CreateLogData(receiveDataThread, sendDataThread);

                if (LogDataChanged != null)
                    LogDataChanged();
            }
        }
        #endregion
         
        #region Method CreateAnswer
        private void CreateAnswer(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {

            switch (receiveData.功能码)
            {
                // 读线圈 Read Coils
                case 1:
                    if (!FC01_Enable)
                        this.ReadCoils(receiveData, sendData, stream, portIn, ipAddressIn);
                    else
                    {
                        sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);//错误代码 = 0x81
                        sendData.异常码 = 1;//异常码 = 0x01
                        sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn); //发送错误代码
                    }
                    break;
                // 读离散输入 Read Input Registers
                case 2:
                    if (!FC02_Enable)
                        this.ReadDiscreteInputs(receiveData, sendData, stream, portIn, ipAddressIn);
                    else
                    {
                        sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                        sendData.异常码 = 1;
                        sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                    }
                    
                    break;
                // 读保持寄存器 Read Holding Registers
                case 3:
                    if (!FC03_Enable)
                        this.ReadHoldingRegisters(receiveData, sendData, stream, portIn, ipAddressIn);
                    else
                    {
                        sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                        sendData.异常码 = 1;
                        sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                    }
                    
                    break;
                // 读输入寄存器 Read Input Registers
                case 4:
                    if (!FC04_Enable)
                        this.ReadInputRegisters(receiveData, sendData, stream, portIn, ipAddressIn);
                    else
                    {
                        sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                        sendData.异常码 = 1;
                        sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                    }
                    
                    break;
                // 写单线圈 Write single coil
                case 5:
                    if (!FC05_Enable)
                        this.WriteSingleCoil(receiveData, sendData, stream, portIn, ipAddressIn);
                    else
                    {
                        sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                        sendData.异常码 = 1;
                        sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                    }
                    
                    break;
                // 写单寄存器 Write single register
                case 6:
                    if (!FC06_Enable)
                        this.WriteSingleRegister(receiveData, sendData, stream, portIn, ipAddressIn);
                    else
                    {
                        sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                        sendData.异常码 = 1;
                        sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                    }
                    
                        break;
                // 写多线圈 Write Multiple 线圈
                case 15:
                        if (!FC15_Enable)
                            this.WriteMultipleCoils(receiveData, sendData, stream, portIn, ipAddressIn);
                        else
                        {
                            sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                            sendData.异常码 = 1;
                            sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                        }

                        break;
                // 写多寄存器 Write Multiple registers
                case 16:
                        if (!FC16_Enable)
                            this.WriteMultipleRegisters(receiveData, sendData, stream, portIn, ipAddressIn);
                        else
                        {
                            sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                            sendData.异常码 = 1;
                            sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                        }

                        break;
                // 读写多寄存器 Read Write Multiple registers
                case 23:
                        if (!FC23_Enable)
                            this.ReadWriteMultipleRegisters(receiveData, sendData, stream, portIn, ipAddressIn);
                        else
                        {
                            sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                            sendData.异常码 = 1;
                            sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                        }

                        break;
                // 不支持函数代码 Error: Function Code not supported
                default: sendData.错误代码 = (byte) (receiveData.功能码 + 0x80);
                        sendData.异常码 = 1;
                        sendException(sendData.错误代码, sendData.异常码, receiveData, sendData, stream, portIn, ipAddressIn);
                        break;
            }
            sendData.时间戳 = DateTime.Now;
        }
        #endregion
         
        private void ReadCoils(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            if ((receiveData.数量 < 1) | (receiveData.数量 > 0x07D0))  //Invalid 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if (((receiveData.起始地址 + 1 + receiveData.数量) > 65535) | (receiveData.起始地址 < 0))     //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                if ((receiveData.数量 % 8) == 0)
                    sendData.字节数 = (byte)(receiveData.数量 / 8);
                else
                    sendData.字节数 = (byte)(receiveData.数量 / 8 + 1);

                sendData.发送线圈值 = new bool[receiveData.数量];
                lock (lockCoils)
                    Array.Copy(线圈.localArray, receiveData.起始地址 + 1, sendData.发送线圈值, 0, receiveData.数量);
            }
            if (true)
            {
                Byte[] data;

                if (sendData.异常码 > 0)
                	data = new byte[9 + 2*Convert.ToInt32(串口标志)];
                else
                   	data = new byte[9 + sendData.字节数+ 2*Convert.ToInt32(串口标志)];
              
                Byte[] byteData = new byte[2];

                sendData.长度 = (byte)(data.Length - 6);

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];
                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;

                //ByteCount
                data[8] = sendData.字节数;

                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送线圈值 = null;
                }

                if (sendData.发送线圈值 != null)
                    for (int i = 0; i < (sendData.字节数); i++)
                    {
                        byteData = new byte[2];
                        for (int j = 0; j < 8; j++)
                        {

                            byte boolValue;
                            if (sendData.发送线圈值[i * 8 + j] == true)
                                boolValue = 1;
                            else
                                boolValue = 0;
                            byteData[1] = (byte)((byteData[1]) | (boolValue << j));
                            if ((i * 8 + j + 1) >= sendData.发送线圈值.Length)
                                break;
                        }
                        data[9 + i] = byteData[1];
                    }
                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }
                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
            }  
        }

        private void ReadDiscreteInputs(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            if ((receiveData.数量 < 1) | (receiveData.数量 > 0x07D0))  //Invalid 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if (((receiveData.起始地址 + 1 + receiveData.数量) > 65535) | (receiveData.起始地址 < 0))   //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                if ((receiveData.数量 % 8) == 0)
                    sendData.字节数 = (byte)(receiveData.数量 / 8);
                else
                    sendData.字节数 = (byte)(receiveData.数量 / 8 + 1);

                sendData.发送线圈值 = new bool[receiveData.数量];
                Array.Copy(离散输入.localArray, receiveData.起始地址 + 1, sendData.发送线圈值, 0, receiveData.数量);
            }
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[9 + sendData.字节数 + 2 * Convert.ToInt32(串口标志)];
                Byte[] byteData = new byte[2];
                sendData.长度 = (byte)(data.Length - 6);

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;

                //ByteCount
                data[8] = sendData.字节数;


                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送线圈值 = null;
                }

                if (sendData.发送线圈值 != null)
                    for (int i = 0; i < (sendData.字节数); i++)
                    {
                        byteData = new byte[2];
                        for (int j = 0; j < 8; j++)
                        {

                            byte boolValue;
                            if (sendData.发送线圈值[i * 8 + j] == true)
                                boolValue = 1;
                            else
                                boolValue = 0;
                            byteData[1] = (byte)((byteData[1]) | (boolValue << j));
                            if ((i * 8 + j + 1) >= sendData.发送线圈值.Length)
                                break;
                        }
                        data[9 + i] = byteData[1];
                    }

                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }
                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if(debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
            }
        }

        private void ReadHoldingRegisters(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            if ((receiveData.数量 < 1) | (receiveData.数量 > 0x007D))  //Invalid 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if (((receiveData.起始地址 + 1 + receiveData.数量) > 65535)  | (receiveData.起始地址 < 0))   //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                sendData.字节数 = (byte)(2 * receiveData.数量);
                sendData.发送寄存器值 = new Int16[receiveData.数量];
                lock (lockHoldingRegisters)
                    Buffer.BlockCopy(保持寄存器.localArray, receiveData.起始地址 * 2 + 2, sendData.发送寄存器值, 0, receiveData.数量 * 2);
            }
                if (sendData.异常码 > 0)
                    sendData.长度 = 0x03;
                else
                    sendData.长度 = (ushort)(0x03 + sendData.字节数);
            
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[9 + sendData.字节数 + 2 * Convert.ToInt32(串口标志)];
                Byte[] byteData = new byte[2];
                sendData.长度 = (byte)(data.Length - 6);

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;

                //ByteCount
                data[8] = sendData.字节数;

                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送寄存器值 = null;
                }
   

                if (sendData.发送寄存器值 != null)
                    for (int i = 0; i < (sendData.字节数 / 2); i++)
                    {
                        byteData = BitConverter.GetBytes((Int16)sendData.发送寄存器值[i]);
                        data[9 + i * 2] = byteData[1];
                        data[10 + i * 2] = byteData[0];
                    }
                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }
                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
            }       
        }

        private void ReadInputRegisters(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            if ((receiveData.数量 < 1) | (receiveData.数量 > 0x007D))  //Invalid 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if (((receiveData.起始地址 + 1 + receiveData.数量) > 65535)  | (receiveData.起始地址 < 0))   //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                sendData.字节数 = (byte)(2 * receiveData.数量);
                sendData.发送寄存器值 = new Int16[receiveData.数量];
                Buffer.BlockCopy(输入寄存器.localArray, receiveData.起始地址 * 2 + 2, sendData.发送寄存器值, 0, receiveData.数量 * 2);
            }
                if (sendData.异常码 > 0)
                    sendData.长度 = 0x03;
                else
                    sendData.长度 = (ushort)(0x03 + sendData.字节数);
            
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[9 + sendData.字节数 + 2 * Convert.ToInt32(串口标志)];
                Byte[] byteData = new byte[2];
                sendData.长度 = (byte)(data.Length - 6);

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;

                //ByteCount
                data[8] = sendData.字节数;

                
                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送寄存器值 = null;
                }


                if (sendData.发送寄存器值 != null)
                    for (int i = 0; i < (sendData.字节数 / 2); i++)
                    {
                        byteData = BitConverter.GetBytes((Int16)sendData.发送寄存器值[i]);
                        data[9 + i * 2] = byteData[1];
                        data[10 + i * 2] = byteData[0];
                    }
                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }

                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
            }
        }

        private void WriteSingleCoil(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            sendData.起始地址 = receiveData.起始地址;
            sendData.接收线圈值 = receiveData.接收线圈值;
            if ((receiveData.接收线圈值[0] != 0x0000) & (receiveData.接收线圈值[0] != 0xFF00))  //Invalid Value
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if (((receiveData.起始地址 + 1) > 65535)  | (receiveData.起始地址 < 0))    //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                if (receiveData.接收线圈值[0] == 0xFF00)
                {
                    lock (lockCoils)
                        线圈[receiveData.起始地址 + 1] = true;
                }
                if (receiveData.接收线圈值[0] == 0x0000)
                {
                    lock (lockCoils)
                        线圈[receiveData.起始地址 + 1] = false;
                }
            }
                if (sendData.异常码 > 0)
                    sendData.长度 = 0x03;
                else
                    sendData.长度 = 0x06;
            
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[12 + 2 * Convert.ToInt32(串口标志)];

                Byte[] byteData = new byte[2];
                sendData.长度 = (byte)(data.Length - 6);

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;



                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送寄存器值 = null;
                }
                else
                {
                    byteData = BitConverter.GetBytes((int)receiveData.起始地址);
                    data[8] = byteData[1];
                    data[9] = byteData[0];
                    byteData = BitConverter.GetBytes((int)receiveData.接收线圈值[0]);
                    data[10] = byteData[1];
                    data[11] = byteData[0];
                }


                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }

                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
                if (CoilsChanged != null)
                    CoilsChanged(receiveData.起始地址+1, 1);
            }
        }

        private void WriteSingleRegister(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            sendData.起始地址 = receiveData.起始地址;
            sendData.接收寄存器值 = receiveData.接收寄存器值;
           
            if ((receiveData.接收寄存器值[0] < 0x0000) | (receiveData.接收寄存器值[0] > 0xFFFF))  //Invalid Value
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if (((receiveData.起始地址 + 1) > 65535)  | (receiveData.起始地址 < 0))    //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                lock (lockHoldingRegisters)
                    保持寄存器[receiveData.起始地址 + 1] = unchecked((short)receiveData.接收寄存器值[0]);
            }
                if (sendData.异常码 > 0)
                    sendData.长度 = 0x03;
                else
                    sendData.长度 = 0x06;
            
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[12 + 2 * Convert.ToInt32(串口标志)];

                Byte[] byteData = new byte[2];
                sendData.长度 = (byte)(data.Length - 6);


                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;



                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送寄存器值 = null;
                }
                else
                {
                    byteData = BitConverter.GetBytes((int)receiveData.起始地址);
                    data[8] = byteData[1];
                    data[9] = byteData[0];
                    byteData = BitConverter.GetBytes((int)receiveData.接收寄存器值[0]);
                    data[10] = byteData[1];
                    data[11] = byteData[0];
                }


                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }

                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
                if (HoldingRegistersChanged != null)
                    HoldingRegistersChanged(receiveData.起始地址+1, 1);
            }
        }

        private void WriteMultipleCoils(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            sendData.起始地址 = receiveData.起始地址;
            sendData.数量 = receiveData.数量;
            
            if ((receiveData.数量 == 0x0000) | (receiveData.数量 > 0x07B0))  //Invalid Quantity
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if ((((int)receiveData.起始地址 + 1 + (int)receiveData.数量) > 65535)  | (receiveData.起始地址 < 0))    //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                lock (lockCoils)
                    for (int i = 0; i < receiveData.数量; i++)
                    {
                        int shift = i % 16;
                    /*                if ((i == receiveData.数量 - 1) & (receiveData.数量 % 2 != 0))
                                    {
                                        if (shift < 8)
                                            shift = shift + 8;
                                        else
                                            shift = shift - 8;
                                    }*/
                        int mask = 0x1;
                        mask = mask << (shift);
                        if ((receiveData.接收线圈值[i / 16] & (ushort)mask) == 0)
                        
                            线圈[receiveData.起始地址 + i + 1] = false;
                        else
                        
                            线圈[receiveData.起始地址 + i + 1] = true;

                    }
            }
            if (sendData.异常码 > 0)
                sendData.长度 = 0x03;
            else
                sendData.长度 = 0x06;
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[12 + 2 * Convert.ToInt32(串口标志)];

                Byte[] byteData = new byte[2];
                sendData.长度 = (byte)(data.Length - 6);

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;



                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送寄存器值 = null;
                }
                else
                {
                    byteData = BitConverter.GetBytes((int)receiveData.起始地址);
                    data[8] = byteData[1];
                    data[9] = byteData[0];
                    byteData = BitConverter.GetBytes((int)receiveData.数量);
                    data[10] = byteData[1];
                    data[11] = byteData[0];
                }


                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }

                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
                if (CoilsChanged != null)
                    CoilsChanged(receiveData.起始地址+1, receiveData.数量);
            }
        }

        private void WriteMultipleRegisters(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;
            sendData.起始地址 = receiveData.起始地址;
            sendData.数量 = receiveData.数量;

            if ((receiveData.数量 == 0x0000) | (receiveData.数量 > 0x07B0))  //Invalid Quantity
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if ((((int)receiveData.起始地址 + 1 + (int)receiveData.数量) > 65535)  | (receiveData.起始地址 < 0))   //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                lock (lockHoldingRegisters)
                    for (int i = 0; i < receiveData.数量; i++)
                    {
                        保持寄存器[receiveData.起始地址 + i + 1] = unchecked((short)receiveData.接收寄存器值[i]);
                    }
            }
            if (sendData.异常码 > 0)
                sendData.长度 = 0x03;
            else
                sendData.长度 = 0x06;
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[12 + 2 * Convert.ToInt32(串口标志)];

                Byte[] byteData = new byte[2];
                sendData.长度 = (byte)(data.Length - 6);

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;



                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送寄存器值 = null;
                }
                else
                {
                    byteData = BitConverter.GetBytes((int)receiveData.起始地址);
                    data[8] = byteData[1];
                    data[9] = byteData[0];
                    byteData = BitConverter.GetBytes((int)receiveData.数量);
                    data[10] = byteData[1];
                    data[11] = byteData[0];
                }


                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }

                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                    }
                catch (Exception) { }
                if (HoldingRegistersChanged != null)
                    HoldingRegistersChanged(receiveData.起始地址+1, receiveData.数量);
            }
        }

        private void ReadWriteMultipleRegisters(ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = this.单元标识符;
            sendData.功能码 = receiveData.功能码;


            if ((receiveData.读数量 < 0x0001) | (receiveData.读数量 > 0x007D) | (receiveData.写数量 < 0x0001) | (receiveData.写数量 > 0x0079) | (receiveData.字节数 != (receiveData.写数量 * 2)))  //Invalid Quantity
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 3;
            }
            if ((((int)receiveData.读起始地址 + 1 + (int)receiveData.读数量) > 65535) | (((int)receiveData.写起始地址 + 1 + (int)receiveData.写数量) > 65535) | (receiveData.写数量 < 0) | (receiveData.读数量 < 0))    //Invalid Starting adress or Starting address + 数量
            {
                sendData.错误代码 = (byte)(receiveData.功能码 + 0x80);
                sendData.异常码 = 2;
            }
            if (sendData.异常码 == 0)
            {
                sendData.发送寄存器值 = new Int16[receiveData.读数量];
                lock (lockHoldingRegisters)
                    Buffer.BlockCopy(保持寄存器.localArray, receiveData.读起始地址 * 2 + 2, sendData.发送寄存器值, 0, receiveData.读数量 * 2);

                lock (保持寄存器)
                    for (int i = 0; i < receiveData.写数量; i++)
                    {
                        保持寄存器[receiveData.写起始地址 + i + 1] = unchecked((short)receiveData.接收寄存器值[i]);
                    }
                sendData.字节数 = (byte)(2 * receiveData.读数量);
            }
            if (sendData.异常码 > 0)
                sendData.长度 = 0x03;
            else
                sendData.长度 = Convert.ToUInt16(3 + 2 * receiveData.读数量);
            if (true)
            {
                Byte[] data;
                if (sendData.异常码 > 0)
                    data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                else
                    data = new byte[9 + sendData.字节数 + 2 * Convert.ToInt32(串口标志)];

                Byte[] byteData = new byte[2];

                //Send Transaction identifier
                byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                data[0] = byteData[1];
                data[1] = byteData[0];

                //Send Protocol identifier
                byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                data[2] = byteData[1];
                data[3] = byteData[0];

                //Send 长度
                byteData = BitConverter.GetBytes((int)sendData.长度);
                data[4] = byteData[1];
                data[5] = byteData[0];

                //Unit Identifier
                data[6] = sendData.单元标识符;

                //Function Code
                data[7] = sendData.功能码;

                //ByteCount
                data[8] = sendData.字节数;


                if (sendData.异常码 > 0)
                {
                    data[7] = sendData.错误代码;
                    data[8] = sendData.异常码;
                    sendData.发送寄存器值 = null;
                }
                else
                {
                    if (sendData.发送寄存器值 != null)
                        for (int i = 0; i < (sendData.字节数 / 2); i++)
                        {
                            byteData = BitConverter.GetBytes((Int16)sendData.发送寄存器值[i]);
                            data[9 + i * 2] = byteData[1];
                            data[10 + i * 2] = byteData[0];
                        }

                }


                try
                {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }

                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                }
                catch (Exception) { }
                if (HoldingRegistersChanged != null)
                    HoldingRegistersChanged(receiveData.写起始地址+1, receiveData.写数量);
            }
        }

        private void sendException(int errorCode, int 异常码, ModbusProtocol receiveData, ModbusProtocol sendData, NetworkStream stream, int portIn, IPAddress ipAddressIn)
        {
            sendData.响应 = true;

            sendData.事务标识符 = receiveData.事务标识符;
            sendData.协议标识符 = receiveData.协议标识符;

            sendData.单元标识符 = receiveData.单元标识符;
            sendData.错误代码 = (byte)errorCode;
            sendData.异常码 = (byte)异常码;

             if (sendData.异常码 > 0)
                sendData.长度 = 0x03;
            else
                sendData.长度 = (ushort)(0x03 + sendData.字节数);

             if (true)
             {
                 Byte[] data;
                 if (sendData.异常码 > 0)
                     data = new byte[9 + 2 * Convert.ToInt32(串口标志)];
                 else
                     data = new byte[9 + sendData.字节数 + 2 * Convert.ToInt32(串口标志)];
                 Byte[] byteData = new byte[2];
                 sendData.长度 = (byte)(data.Length - 6);

                 //Send Transaction identifier
                 byteData = BitConverter.GetBytes((int)sendData.事务标识符);
                 data[0] = byteData[1];
                 data[1] = byteData[0];

                 //Send Protocol identifier
                 byteData = BitConverter.GetBytes((int)sendData.协议标识符);
                 data[2] = byteData[1];
                 data[3] = byteData[0];

                 //Send 长度
                 byteData = BitConverter.GetBytes((int)sendData.长度);
                 data[4] = byteData[1];
                 data[5] = byteData[0];

                 //Unit Identifier
                 data[6] = sendData.单元标识符;


                 data[7] = sendData.错误代码;
                 data[8] = sendData.异常码;


                 try
                 {
                    if (串口标志)
                    {
                        if (!串口.IsOpen)
                            throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                        //Create CRC
                        sendData.crc = ModbusClient.CRC(data, Convert.ToUInt16(data.Length - 8), 6);
                        byteData = BitConverter.GetBytes((int)sendData.crc);
                        data[data.Length - 2] = byteData[0];
                        data[data.Length - 1] = byteData[1];
                        串口.Write(data, 6, data.Length - 6);
                        if (debug)
                        {
                            byte[] debugData = new byte[data.Length - 6];
                            Array.Copy(data, 6, debugData, 0, data.Length - 6);
                            if (debug) StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        }
                    }
                    else if (UDP标志)
                    {
                        //UdpClient udpClient = new UdpClient();
                        IPEndPoint endPoint = new IPEndPoint(ipAddressIn, portIn);
                        udpClient.Send(data, data.Length, endPoint);

                    }
                    else
                    {
                        stream.Write(data, 0, data.Length);
                        if (debug) StoreLogData.Instance.Store("Send Data: " + BitConverter.ToString(data), System.DateTime.Now);
                    }
                 }
                 catch (Exception) { }
             }
        }

        private void CreateLogData(ModbusProtocol receiveData, ModbusProtocol sendData)
        {
            for (int i = 0; i < 98; i++)
            {
                日志数据[99 - i] = 日志数据[99 - i - 2];

            }
            日志数据[0] = receiveData;
            日志数据[1] = sendData;

        }



        public int NumberOfConnections
        {
            get{return 连接数; }
        }

        public ModbusProtocol[] ModbusLogData
        {
            get { return 日志数据;  }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public bool UDPFlag
        {
            get { return UDP标志; }
            set { UDP标志 = value; }
        }
        
 		public bool SerialFlag
 		{
 			get{return 串口标志;}
 			set{串口标志 = value;}
 		}

        public int Baudrate
        {
            get
            {
                return baudrate;
            }
            set
            {
                baudrate = value;
            }
        }

        public System.IO.Ports.Parity Parity
        {
            get
            {
                return parity;
            }
            set
            {
                parity = value;
            }
        }

        public System.IO.Ports.StopBits StopBits
        {
            get
            {
                return stopBits;
            }
            set
            {
                stopBits = value;
            }
        }

        public string SerialPort
        {
            get
            {
                return serialPort;
            }
            set
            {
                serialPort = value;
                if (serialPort != null)
                    串口标志 = true;
                else
                    串口标志 = false;
            }
        }

        public byte UnitIdentifier
        {
            get
            {
                return 单元标识符;
            }
            set
            {
                单元标识符 = value;
            }
        }

    public class HoldingRegisters
    {
        public Int16[] localArray = new Int16[65535];
        ModbusServer modbusServer;
     
        public HoldingRegisters(EasyModbus.ModbusServer modbusServer)
        {
            this.modbusServer = modbusServer;
        }

        public Int16 this[int x]
        {
            get { return this.localArray[x]; }
            set  {  this.localArray[x] = value;  }
        }
     }

    public class InputRegisters
    {
        public Int16[] localArray = new Int16[65535];
        ModbusServer modbusServer;

        public InputRegisters(EasyModbus.ModbusServer modbusServer)
        {
            this.modbusServer = modbusServer;
        }

        public Int16 this[int x]
        {
            get { return this.localArray[x]; }
            set
            {
                this.localArray[x] = value;

            }
        }
    }

    public class Coils
    {
        public bool[] localArray = new bool[65535];
        ModbusServer modbusServer;

        public Coils(EasyModbus.ModbusServer modbusServer)
        {
            this.modbusServer = modbusServer;
        }

        public bool this[int x]
        {
            get { return this.localArray[x]; }
            set
            {
                this.localArray[x] = value;
            
            }
        }
    }

    public class DiscreteInputs
    {
        public bool[] localArray = new bool[65535];
        ModbusServer modbusServer;

        public DiscreteInputs(EasyModbus.ModbusServer modbusServer)
        {
            this.modbusServer = modbusServer;
        }

        public bool this[int x]
        {
            get { return this.localArray[x]; }
            set
            {
                this.localArray[x] = value;
              
            }
        }

      
        }
    }
}
   