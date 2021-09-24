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
using System.Net.Sockets;
using System.Net;
using System.IO.Ports;
using System.Reflection;

namespace EasyModbus
{
    /// <summary> 实现 Modbus Client。
    /// Implements a ModbusClient.
    /// </summary>
    public partial class ModbusClient
	{
        #region 公有变量
        public enum 寄存器顺序 { LowHigh = 0, HighLow = 1 };// RegisterOrder 
        public byte[] 接收数据;// receiveData
        public byte[] 发送数据; // sendData
        public int 重试次数 { get; set; } = 3; //NumberOfRetries

        public delegate void ReceiveDataChangedHandler(object sender);//接收数据已更改处理程序
        public event ReceiveDataChangedHandler 接收数据已更改事件;// ReceiveDataChanged

        public delegate void SendDataChangedHandler(object sender);
        public event SendDataChangedHandler 发送数据已更改事件;// SendDataChanged

        public delegate void ConnectedChangedHandler(object sender);
        public event ConnectedChangedHandler 连接已更改事件;// ConnectedChanged
        #endregion

        #region 私有变量
        private bool debug = false;//调式

        private TcpClient tcpClient;//TCP连接
        private string ipAddress = "127.0.0.1";//IP
        private int port = 8000;//端口
        private uint 内部事务标识码 = 0;// transactionIdentifierInternal
        private byte[] 事务标识符 = new byte[2];//transactionIdentifier
        private byte[] 协议标识符 = new byte[2];//protocolIdentifier
        private byte[] crc = new byte[2];
        private byte[] 长度 = new byte[2];//length
        private byte 单元标识符 = 0x01;//unitIdentifier
        private byte 功能码;//functionCode
        private byte[] 起始地址 = new byte[2];//startingAddress
        private byte[] 数量 = new byte[2];//quantity
        private bool UDP标志 = false;//udpFlag
        private int portOut;
        private int 波特率 = 9600;//baudRate
        private int 读超时 = 1000;// connectTimeout
        private SerialPort 串口;//serialport
        private Parity 奇偶校验 = Parity.Even;//parity
        private StopBits 停止位 = StopBits.One;//stopBits
        private int 写超时 = 10000;//WriteTimeout
        private bool 连接OK = false;//connected
        private int 重试计数 = 0;//countRetries

        private bool 是否收到数据 = false;//dataReceived
        private bool 接收中 = false;//receiveActive
        private byte[] 读字节缓冲 = new byte[256];// readBuffer
        private int 读字节数 = 0;// bytesToRead
        //private int 要读取的实际位置 = 0;// akjjjctualPositionToRead
        //DateTime 上次读取日期时间;// dateTimeLastRead
        #endregion

        NetworkStream Net基础流;//

        #region Modbus连接
        /// <summary> 确定主机ip地址和主机端口的构造函数
        /// Constructor which determines the Master ip-address and the Master Port.
        /// </summary>
        /// <param name="ipAddress">IP-Address of the Master device</param>
        /// <param name="port">Listening port of the Master device (should be 502)</param>
        public ModbusClient(string ipAddress, int port)//TCP UDP
		{
            //if (debug) StoreLogData.Instance.Store("EasyModbus library initialized for Modbus-TCP, IPAddress: " + ipAddress + ", Port: "+port ,System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("初始化Modbus TCP库，IP地址：" + ipAddress + ", Port: " + port, System.DateTime.Now);
#if (!COMMERCIAL)//商业的
            Console.WriteLine("EasyModbus Client Library Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Copyright (c) Stefan Rossmann Engineering Solutions");
            Console.WriteLine();
#endif
            this.ipAddress = ipAddress;
			this.port = port;
		}

        /// <summary> 用于确定串行端口的构造函数
        /// Constructor which determines the Serial-Port
        /// </summary>
        /// <param name="serialPort">Serial-Port Name e.G. "COM1"</param>
        public ModbusClient(string serialPort) //串口
        {
            //if (debug) StoreLogData.Instance.Store("EasyModbus library initialized for Modbus-RTU, COM-Port: " + serialPort, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("初始化Modbus RTU库，COM端口：" + serialPort, System.DateTime.Now);
#if (!COMMERCIAL)//商业的
            //Console.WriteLine("EasyModbus Client Library Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Modbus客户端库版本：" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Copyright (c) Stefan Rossmann Engineering Solutions");
            Console.WriteLine();
#endif
            this.串口 = new SerialPort();
            串口.PortName = serialPort;
            串口.BaudRate = 波特率;
            串口.Parity = 奇偶校验;
            串口.StopBits = 停止位;
            串口.WriteTimeout = 写超时;
            串口.ReadTimeout = 读超时;

            串口.DataReceived += new SerialDataReceivedEventHandler(串口接收数据事件);
        }

        /// <summary> 无参数构造函数
        /// Parameterless constructor
        /// </summary>
        public ModbusClient()
        {
            //if (debug) StoreLogData.Instance.Store("EasyModbus library initialized for Modbus-TCP" ,System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("初始化Modbus TCP库", System.DateTime.Now);
#if (!COMMERCIAL)
            Console.WriteLine("EasyModbus Client Library Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Copyright (c) Stefan Rossmann Engineering Solutions");
            Console.WriteLine();
#endif
        }

        /// <summary> 析构函数-与主设备断开连接前调用
        /// Destructor - Close connection to Master Device.
        /// </summary>
        ~ModbusClient()
        {
            //if (debug) StoreLogData.Instance.Store("Destructor called - automatically disconnect", System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("自动断开前调用的函数", System.DateTime.Now);
            if (串口 != null)//关闭串口
            {
                if (串口.IsOpen) { 串口.Close(); }
                return;
            }
            if (tcpClient != null & !UDP标志)//关闭TCP连接
            {
                if (Net基础流 != null) { Net基础流.Close(); }
                tcpClient.Close();
            }
        }

        /// <summary> 在Modbus TCP的情况下建立与主设备的连接。在Modbus RTU的情况下打开COM端口
        /// Establish connection to Master device in case of Modbus TCP. Opens COM-Port in case of Modbus RTU
        /// </summary>
        public void Connect()
		{
            if (串口 != null)
            {
                if (!串口.IsOpen)// 串口是否打开?
                {
                	if (debug) StoreLogData.Instance.Store("Open Serial port " + 串口.PortName,System.DateTime.Now);
                    串口.BaudRate = 波特率;
                    串口.Parity = 奇偶校验;
                    串口.StopBits = 停止位;
                    串口.WriteTimeout = 写超时;
                    串口.ReadTimeout = 读超时;
                    串口.Open();
                    连接OK = true;
                }
                if (连接已更改事件 != null)
                {
                    try
                    {
                        连接已更改事件(this);
                    }
                    catch { }
                }                   
                return;
            }
            if (!UDP标志)
            {
                if (debug) StoreLogData.Instance.Store("打开TCP套接字,IP: " + ipAddress + ", Port: " + port, System.DateTime.Now);
                tcpClient = new TcpClient();
                var result = tcpClient.BeginConnect(ipAddress, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(读超时);
                if (!success)
                {
                    throw new EasyModbus.Exceptions.ConnectionException("连接超时");
                    //throw new EasyModbus.Exceptions.ConnectionException("connection timed out");
                }
                tcpClient.EndConnect(result);

                //tcpClient = new TcpClient(ipAddress, port);
                Net基础流 = tcpClient.GetStream();
                Net基础流.ReadTimeout = 读超时;
                连接OK = true;
            }
            else
            {
                tcpClient = new TcpClient();
                连接OK = true;
            }
            if (连接已更改事件 != null)
            {
                try
                {
                    连接已更改事件(this);
                }
                catch { }
            }  
        }

        /// <summary> 在Modbus TCP的情况下建立与主设备的连接
        /// Establish connection to Master device in case of Modbus TCP.
        /// </summary>
        public void Connect(string ipAddress, int port)
		{
            if (!UDP标志)
            {
                if (debug) StoreLogData.Instance.Store("Open TCP-Socket, IP-Address: " + ipAddress + ", Port: " + port, System.DateTime.Now);
                tcpClient = new TcpClient();
                var result = tcpClient.BeginConnect(ipAddress, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(读超时);
                if (!success)
                {
                    throw new EasyModbus.Exceptions.ConnectionException("connection timed out");
                }
                tcpClient.EndConnect(result);

                Net基础流 = tcpClient.GetStream();
                Net基础流.ReadTimeout = 读超时;
                连接OK = true;
            }
            else //UDP
            {
                tcpClient = new TcpClient();
                连接OK = true;
            }

            if (连接已更改事件 != null) { 连接已更改事件(this); }
        }
        /// <summary> 关闭与主设备的连接。
        /// Close connection to Master Device.
        /// </summary>
        public void Disconnect()
        {
            if (debug) StoreLogData.Instance.Store("Disconnect", System.DateTime.Now);
            if (串口 != null)
            {
                if (串口.IsOpen & !this.接收中)
                    串口.Close();
                if (连接已更改事件 != null)
                    连接已更改事件(this);
                return;
            }
            if (Net基础流 != null)
                Net基础流.Close();
            if (tcpClient != null)
                tcpClient.Close();
            连接OK = false;
            if (连接已更改事件 != null)
                连接已更改事件(this);

        }
        #endregion

        #region 数据转换
        /// <summary>
        /// Converts two ModbusRegisters to Float - Example: EasyModbus.ModbusClient.ConvertRegistersToFloat(modbusClient.ReadHoldingRegisters(19,2))
        /// </summary>
        /// <param name="registers">Two Register values received from Modbus</param>
        /// <returns>Connected float value</returns>
        public static float 转为浮点数(int[] registers)//32Bit ConvertRegistersToFloat
        {
            if (registers.Length != 2)
                //throw new ArgumentException("Input Array length invalid - Array langth must be '2'");
                throw new ArgumentException("输入数组长度无效-数组长度必须为“2”");
            int highRegister = registers[1];
            int lowRegister = registers[0];
            byte[] highRegisterBytes = BitConverter.GetBytes(highRegister);
            byte[] lowRegisterBytes = BitConverter.GetBytes(lowRegister);
            byte[] floatBytes = {
                                    lowRegisterBytes[0],
                                    lowRegisterBytes[1],
                                    highRegisterBytes[0],
                                    highRegisterBytes[1]
                                };
            return BitConverter.ToSingle(floatBytes, 0);
        }

        /// <summary>
        /// Converts two ModbusRegisters to Float, Registers can by swapped
        /// </summary>
        /// <param name="registers">Two Register values received from Modbus</param>
        /// <param name="registerOrder">Desired Word Order (Low Register first or High Register first</param>
        /// <returns>Connected float value</returns>
        public static float 转为浮点数(int[] registers, 寄存器顺序 registerOrder)//32Bit ConvertRegistersToFloat
        {
            int [] swappedRegisters = {registers[0],registers[1]};
            if (registerOrder == 寄存器顺序.HighLow) 
                swappedRegisters = new int[] {registers[1],registers[0]};
            return 转为浮点数(swappedRegisters);
        }

        /// <summary>
        /// Converts two ModbusRegisters to 32 Bit Integer value
        /// </summary>
        /// <param name="registers">Two Register values received from Modbus</param>
        /// <returns>Connected 32 Bit Integer value</returns>
        public static Int32 转为INT(int[] registers)//32Bit ConvertRegistersToInt
        {
            if (registers.Length != 2)
                throw new ArgumentException("输入数组长度无效-数组长度必须为“2”");
            int highRegister = registers[1];
            int lowRegister = registers[0];
            byte[] highRegisterBytes = BitConverter.GetBytes(highRegister);
            byte[] lowRegisterBytes = BitConverter.GetBytes(lowRegister);
            byte[] doubleBytes = {
                                    lowRegisterBytes[0],
                                    lowRegisterBytes[1],
                                    highRegisterBytes[0],
                                    highRegisterBytes[1]
                                };
            return BitConverter.ToInt32(doubleBytes, 0);
        }

        /// <summary>
        /// Converts two ModbusRegisters to 32 Bit Integer Value - Registers can be swapped
        /// </summary>
        /// <param name="registers">Two Register values received from Modbus</param>
        /// <param name="registerOrder">Desired Word Order (Low Register first or High Register first</param>
        /// <returns>Connecteds 32 Bit Integer value</returns>
        public static Int32 转为INT(int[] registers, 寄存器顺序 registerOrder)//32Bit ConvertRegistersToInt
        {
            int[] swappedRegisters = { registers[0], registers[1] };
            if (registerOrder == 寄存器顺序.HighLow)
                swappedRegisters = new int[] { registers[1], registers[0] };
            return 转为INT(swappedRegisters);
        }

        /// <summary>
        /// Convert four 16 Bit Registers to 64 Bit Integer value Register Order "LowHigh": Reg0: Low Word.....Reg3: High Word, "HighLow": Reg0: High Word.....Reg3: Low Word
        /// </summary>
        /// <param name="registers">four Register values received from Modbus</param>
        /// <returns>64 bit value</returns>
        public static Int64 换为长整数(int[] registers)// 64Bit ConvertRegistersToLong
        {
            if (registers.Length != 4)
                throw new ArgumentException("输入数组长度无效-数组长度必须为“4”");
            int highRegister = registers[3];
            int highLowRegister = registers[2];
            int lowHighRegister = registers[1];
            int lowRegister = registers[0];
            byte[] highRegisterBytes = BitConverter.GetBytes(highRegister);
            byte[] highLowRegisterBytes = BitConverter.GetBytes(highLowRegister);
            byte[] lowHighRegisterBytes = BitConverter.GetBytes(lowHighRegister);
            byte[] lowRegisterBytes = BitConverter.GetBytes(lowRegister);
            byte[] longBytes = {
                                    lowRegisterBytes[0],
                                    lowRegisterBytes[1],
                                    lowHighRegisterBytes[0],
                                    lowHighRegisterBytes[1],
                                    highLowRegisterBytes[0],
                                    highLowRegisterBytes[1],
                                    highRegisterBytes[0],
                                    highRegisterBytes[1]
                                };
            return BitConverter.ToInt64(longBytes, 0);
        }

        /// <summary>
        /// Convert four 16 Bit Registers to 64 Bit Integer value - Registers can be swapped
        /// </summary>
        /// <param name="registers">four Register values received from Modbus</param>
        /// <param name="registerOrder">Desired Word Order (Low Register first or High Register first</param>
        /// <returns>Connected 64 Bit Integer value</returns>
        public static Int64 换为长整数(int[] registers, 寄存器顺序 registerOrder)// 64Bit ConvertRegistersToLong
        {
            if (registers.Length != 4)
                throw new ArgumentException("输入数组长度无效-数组长度必须为“4”");
            int[] swappedRegisters = { registers[0], registers[1], registers[2], registers[3] };
            if (registerOrder == 寄存器顺序.HighLow)
                swappedRegisters = new int[] { registers[3], registers[2], registers[1], registers[0] };
            return 换为长整数(swappedRegisters);
        }

        /// <summary>
        /// Convert four 16 Bit Registers to 64 Bit double prec. value Register Order "LowHigh": Reg0: Low Word.....Reg3: High Word, "HighLow": Reg0: High Word.....Reg3: Low Word
        /// </summary>
        /// <param name="registers">four Register values received from Modbus</param>
        /// <returns>64 bit value</returns>
        public static double 换为Double(int[] registers)//64Bit ConvertRegistersToDouble
        {
            if (registers.Length != 4)
                throw new ArgumentException("输入数组长度无效-数组长度必须为“4”");
            int highRegister = registers[3];
            int highLowRegister = registers[2];
            int lowHighRegister = registers[1];
            int lowRegister = registers[0];
            byte[] highRegisterBytes = BitConverter.GetBytes(highRegister);
            byte[] highLowRegisterBytes = BitConverter.GetBytes(highLowRegister);
            byte[] lowHighRegisterBytes = BitConverter.GetBytes(lowHighRegister);
            byte[] lowRegisterBytes = BitConverter.GetBytes(lowRegister);
            byte[] longBytes = {
                                    lowRegisterBytes[0],
                                    lowRegisterBytes[1],
                                    lowHighRegisterBytes[0],
                                    lowHighRegisterBytes[1],
                                    highLowRegisterBytes[0],
                                    highLowRegisterBytes[1],
                                    highRegisterBytes[0],
                                    highRegisterBytes[1]
                                };
            return BitConverter.ToDouble(longBytes, 0);
        }

        /// <summary>
        /// Convert four 16 Bit Registers to 64 Bit double prec. value - Registers can be swapped
        /// </summary>
        /// <param name="registers">four Register values received from Modbus</param>
        /// <param name="registerOrder">Desired Word Order (Low Register first or High Register first</param>
        /// <returns>Connected double prec. float value</returns>
        public static double 换为Double(int[] registers, 寄存器顺序 registerOrder)//64Bit ConvertRegistersToDouble
        {
            if (registers.Length != 4)
                throw new ArgumentException("输入数组长度无效-数组长度必须为“4”");
            int[] swappedRegisters = { registers[0], registers[1], registers[2], registers[3] };
            if (registerOrder == 寄存器顺序.HighLow)
                swappedRegisters = new int[] { registers[3], registers[2], registers[1], registers[0] };
            return 换为Double(swappedRegisters);
        }

        /// <summary>
        /// Converts float to two ModbusRegisters - Example:  modbusClient.WriteMultipleRegisters(24, EasyModbus.ModbusClient.ConvertFloatToTwoRegisters((float)1.22));
        /// </summary>
        /// <param name="floatValue">Float value which has to be converted into two registers</param>
        /// <returns>Register values</returns>
        public static int[] 浮点数换为Int32(float floatValue)//32Bit ConvertFloatToRegisters 
        {
            byte[] floatBytes = BitConverter.GetBytes(floatValue);
            byte[] highRegisterBytes = 
            {
                floatBytes[2],
                floatBytes[3],
                0,
                0
            };
            byte[] lowRegisterBytes = 
            {
                
                floatBytes[0],
                floatBytes[1],
                0,
                0
            };
            int[] returnValue =
            {
                BitConverter.ToInt32(lowRegisterBytes,0),
                BitConverter.ToInt32(highRegisterBytes,0)
            };
            return returnValue;
        }

        /// <summary>
        /// Converts float to two ModbusRegisters Registers - Registers can be swapped
        /// </summary>
        /// <param name="floatValue">Float value which has to be converted into two registers</param>
        /// <param name="registerOrder">Desired Word Order (Low Register first or High Register first</param>
        /// <returns>Register values</returns>
        public static int[] 浮点数换为Int32(float floatValue, 寄存器顺序 registerOrder)//32Bit ConvertFloatToRegisters
        {
            int[] registerValues = 浮点数换为Int32(floatValue);
            int[] returnValue = registerValues;
            if (registerOrder == 寄存器顺序.HighLow)
                returnValue = new Int32[] { registerValues[1], registerValues[0] };
            return returnValue;
        }

        /// <summary>
        /// Converts 32 Bit Value to two ModbusRegisters
        /// </summary>
        /// <param name="intValue">Int value which has to be converted into two registers</param>
        /// <returns>Register values</returns>
        public static int[] Int32存到寄存器(Int32 intValue)//32Bit ConvertIntToRegisters
        {
            byte[] doubleBytes = BitConverter.GetBytes(intValue);
            byte[] highRegisterBytes = 
            {
                doubleBytes[2],
                doubleBytes[3],
                0,
                0
            };
            byte[] lowRegisterBytes = 
            {
                
                doubleBytes[0],
                doubleBytes[1],
                0,
                0
            };
            int[] returnValue =
            {
                BitConverter.ToInt32(lowRegisterBytes,0),
                BitConverter.ToInt32(highRegisterBytes,0)
            };
            return returnValue;
        }

        /// <summary>
        /// Converts 32 Bit Value to two ModbusRegisters Registers - Registers can be swapped
        /// </summary>
        /// <param name="intValue">Double value which has to be converted into two registers</param>
        /// <param name="registerOrder">Desired Word Order (Low Register first or High Register first</param>
        /// <returns>Register values</returns>
        public static int[] Int32存到寄存器(Int32 intValue, 寄存器顺序 registerOrder)//32Bit ConvertIntToRegisters
        {
            int[] registerValues = Int32存到寄存器(intValue);
            int[] returnValue = registerValues;
            if (registerOrder == 寄存器顺序.HighLow)
                returnValue = new Int32[] { registerValues[1], registerValues[0] };
            return returnValue;
        }

        /// <summary>
        /// Converts 64 Bit Value to four ModbusRegisters
        /// </summary>
        /// <param name="longValue">long value which has to be converted into four registers</param>
        /// <returns>Register values</returns>
        public static int[] ConvertLongToRegisters(Int64 longValue)//将长寄存器转换为寄存器
        {
            byte[] longBytes = BitConverter.GetBytes(longValue);
            byte[] highRegisterBytes =
            {
                longBytes[6],
                longBytes[7],
                0,
                0
            };
            byte[] highLowRegisterBytes =
            {
                longBytes[4],
                longBytes[5],
                0,
                0
            };
            byte[] lowHighRegisterBytes =
            {
                longBytes[2],
                longBytes[3],
                0,
                0
            };
            byte[] lowRegisterBytes =
            {

                longBytes[0],
                longBytes[1],
                0,
                0
            };
            int[] returnValue =
            {
                BitConverter.ToInt32(lowRegisterBytes,0),
                BitConverter.ToInt32(lowHighRegisterBytes,0),
                BitConverter.ToInt32(highLowRegisterBytes,0),
                BitConverter.ToInt32(highRegisterBytes,0)
            };
            return returnValue;
        }

        /// <summary>
        /// Converts 64 Bit Value to four ModbusRegisters - Registers can be swapped
        /// </summary>
        /// <param name="longValue">long value which has to be converted into four registers</param>
        /// <param name="registerOrder">Desired Word Order (Low Register first or High Register first</param>
        /// <returns>Register values</returns>
        public static int[] ConvertLongToRegisters(Int64 longValue, 寄存器顺序 registerOrder)//将长寄存器转换为寄存器
        {
            int[] registerValues = ConvertLongToRegisters(longValue);
            int[] returnValue = registerValues;
            if (registerOrder == 寄存器顺序.HighLow)
                returnValue = new int[] { registerValues[3], registerValues[2], registerValues[1], registerValues[0] };
            return returnValue;
        }

        /// <summary>
        /// Converts 64 Bit double prec Value to four ModbusRegisters
        /// </summary>
        /// <param name="doubleValue">double value which has to be converted into four registers</param>
        /// <returns>Register values</returns>
        public static int[] ConvertDoubleToRegisters(double doubleValue)//转换双精小数寄存器
        {
            byte[] doubleBytes = BitConverter.GetBytes(doubleValue);
            byte[] highRegisterBytes =
            {
                doubleBytes[6],
                doubleBytes[7],
                0,
                0
            };
            byte[] highLowRegisterBytes =
            {
                doubleBytes[4],
                doubleBytes[5],
                0,
                0
            };
            byte[] lowHighRegisterBytes =
            {
                doubleBytes[2],
                doubleBytes[3],
                0,
                0
            };
            byte[] lowRegisterBytes =
            {

                doubleBytes[0],
                doubleBytes[1],
                0,
                0
            };
            int[] returnValue =
            {
                BitConverter.ToInt32(lowRegisterBytes,0),
                BitConverter.ToInt32(lowHighRegisterBytes,0),
                BitConverter.ToInt32(highLowRegisterBytes,0),
                BitConverter.ToInt32(highRegisterBytes,0)
            };
            return returnValue;
        }

        /// <summary>
        /// 转换64位双精度预处理器。四个ModBus寄存器的值-可以交换寄存器
        /// Converts 64 Bit double prec. Value to four ModbusRegisters - Registers can be swapped
        /// </summary>
        /// <param name="doubleValue">必须转换为四个寄存器的double double value which has to be converted into four registers</param>
        /// <param name="registerOrder">所需的字顺序（低寄存器优先或高寄存器优先 Desired Word Order (Low Register first or High Register first</param>
        /// <returns>寄存器值 Register values</returns>
        public static int[] ConvertDoubleToRegisters(double doubleValue, 寄存器顺序 registerOrder)//转换双精小数寄存器
        {
            int[] registerValues = ConvertDoubleToRegisters(doubleValue);
            int[] returnValue = registerValues;
            if (registerOrder == 寄存器顺序.HighLow)
                returnValue = new int[] { registerValues[3], registerValues[2], registerValues[1], registerValues[0] };
            return returnValue;
        }

        /// <summary>
        /// 将16位寄存器值转换为字符串
        /// </summary>
        /// <param name="registers">通过Modbus接收的寄存器数组 Register array received via Modbus</param>
        /// <param name="offset">包含要转换的字符串的第一个寄存器 First Register containing the String to convert</param>
        /// <param name="stringLength">字符串中的字符数（必须为偶数）number of characters in String (must be even)</param>
        /// <returns>Converted String</returns>
        public static string ConvertRegistersToString(int[] registers, int offset, int stringLength)//将寄存器转换为字符串
        { 
        byte[] result = new byte[stringLength];
        byte[] registerResult = new byte[2];
        
            for (int i = 0; i < stringLength/2; i++)
            {
                registerResult = BitConverter.GetBytes(registers[offset + i]);
                result[i * 2] = registerResult[0];
                result[i * 2 + 1] = registerResult[1];
            }
            return System.Text.Encoding.Default.GetString(result);
        }

        /// <summary>
        /// 将字符串转换为16位寄存器
        /// </summary>
        /// <param name="registers">通过Modbus接收的寄存器数组</param>
        /// <returns>转换的字符串</returns>
        public static int[] ConvertStringToRegisters(string stringToConvert)//将字符串转换为寄存器
        {
            byte[] array = System.Text.Encoding.ASCII.GetBytes(stringToConvert);
            int[] returnarray = new int[stringToConvert.Length / 2 + stringToConvert.Length % 2];
            for (int i = 0; i < returnarray.Length; i++)
            {
                returnarray[i] = array[i * 2];
                if (i*2 +1< array.Length)
                {
                    returnarray[i] = returnarray[i] | ((int)array[i * 2 + 1] << 8);
                }
            }
            return returnarray;
        }
        #endregion
        
        /*
                private void DataReceivedHandler(object sender,
                                SerialDataReceivedEventArgs e)
                {
                    long ticksWait = TimeSpan.TicksPerMillisecond * 2000;
                    SerialPort sp = (SerialPort)sender;

                    if (bytesToRead == 0 || sp.BytesToRead == 0)
                    {
                        actualPositionToRead = 0;
                        sp.DiscardInBuffer();
                        dataReceived = false;
                        receiveActive = false;
                        return;
                    }

                    if (actualPositionToRead == 0 && !dataReceived)
                        readBuffer = new byte[256];

                    //if ((DateTime.Now.Ticks - dateTimeLastRead.Ticks) > ticksWait)
                    //{
                    //    readBuffer = new byte[256];
                    //    actualPositionToRead = 0;
                    //}
                    int numberOfBytesInBuffer = sp.BytesToRead;
                    sp.Read(readBuffer, actualPositionToRead, ((numberOfBytesInBuffer + actualPositionToRead) > readBuffer.Length) ? 0 : numberOfBytesInBuffer);
                    actualPositionToRead = actualPositionToRead + numberOfBytesInBuffer;
                    //sp.DiscardInBuffer();
                    //if (DetectValidModbusFrame(readBuffer, (actualPositionToRead < readBuffer.Length) ? actualPositionToRead : readBuffer.Length) | bytesToRead <= actualPositionToRead)
                    if (actualPositionToRead >= bytesToRead)
                    {

                            dataReceived = true;
                            bytesToRead = 0;
                            actualPositionToRead = 0;
                            if (debug) StoreLogData.Instance.Store("Received Serial-Data: " + BitConverter.ToString(readBuffer), System.DateTime.Now);

                    }


                    //dateTimeLastRead = DateTime.Now;
                }
         */

        private void 串口接收数据事件(object sender, SerialDataReceivedEventArgs e)// DataReceivedHandler
        {
            串口.DataReceived -= 串口接收数据事件;

            //while (receiveActive | dataReceived)
            //	System.Threading.Thread.Sleep(10);
            接收中 = true;

            const long 等待时间 = TimeSpan.TicksPerMillisecond * 2000;//((40*10000000) / this.baudRate);


            SerialPort sp = (SerialPort)sender;
            if (读字节数 == 0)
            {
                sp.DiscardInBuffer();
                接收中 = false;
                串口.DataReceived += new SerialDataReceivedEventHandler(串口接收数据事件);
                return;
            }
            读字节缓冲 = new byte[256];
            int 字节数 = 0;
            int 实际读取位置 = 0;
            DateTime 上次读取时间 = DateTime.Now;
            do
            {
                try
                {
                    上次读取时间 = DateTime.Now;
                    while ((sp.BytesToRead) == 0)
                    {
                        System.Threading.Thread.Sleep(10);
                        if ((DateTime.Now.Ticks - 上次读取时间.Ticks) > 等待时间)
                            break;
                    }
                    字节数 = sp.BytesToRead;

                    byte[] 接收字节 = new byte[字节数];
                    sp.Read(接收字节, 0, 字节数);
                    Array.Copy(接收字节, 0, 读字节缓冲, 实际读取位置, (实际读取位置 + 接收字节.Length) <= 读字节数 ? 接收字节.Length : 读字节数 - 实际读取位置);

                    实际读取位置 = 实际读取位置 + 接收字节.Length;

                }
                catch (Exception) { }

                if (读字节数 <= 实际读取位置)
                    break;

                if (检测有效帧(读字节缓冲, (实际读取位置 < 读字节缓冲.Length) ? 实际读取位置 : 读字节缓冲.Length) | 读字节数 <= 实际读取位置)
                    break;
            }
            while ((DateTime.Now.Ticks - 上次读取时间.Ticks) < 等待时间);

            //10.000 Ticks in 1 ms

            接收数据 = new byte[实际读取位置];
            Array.Copy(读字节缓冲, 0, 接收数据, 0, (实际读取位置 < 读字节缓冲.Length) ? 实际读取位置 : 读字节缓冲.Length);
            if (debug) StoreLogData.Instance.Store("Received Serial-Data: " + BitConverter.ToString(读字节缓冲), System.DateTime.Now);
            读字节数 = 0;

            是否收到数据 = true;
            接收中 = false;
            串口.DataReceived += new SerialDataReceivedEventHandler(串口接收数据事件);
            if (接收数据已更改事件 != null)
                接收数据已更改事件(this);

            //sp.DiscardInBuffer();
        }

        public static bool 检测有效帧(byte[] readBuffer, int length)//检测有效的Modbus帧 DetectValidModbusFrame
        {
            // 最小长度为6字节 minimum length 6 bytes
            if (length < 6)
                return false;
            //检查从机ID SlaveID correct
            if ((readBuffer[0] < 1) | (readBuffer[0] > 247))
                return false;
            //检查CRC
            byte[] crc = new byte[2];
            crc = BitConverter.GetBytes(CRC(readBuffer, (ushort)(length - 2), 0));
            if (crc[0] != readBuffer[length - 2] | crc[1] != readBuffer[length - 1])
                return false;
            return true;
        }


        // ** --- ** -- ** --- ** -- ** --- ** -- ** --- ** -- ** --- ** --　** --- ** --　** --- ** --　** --- ** -- ** --- ** -- ** --- ** -- ** --- ** -- **
        #region 功能码函数
        /// <summary>　从主机读取线圈（FC1）Read Coils from Server device (FC1) </summary>
        /// <param name="startingAddress">要读取的第一个线圈</param>
        /// <param name="quantity">要读取的线圈数</param>
        /// <returns>包含线圈的布尔数组 Boolean Array which contains the coils</returns>
        public bool[] 读线圈(int startingAddress, int quantity)// ReadCoils
        {
            //if (debug) StoreLogData.Instance.Store("FC1 (Read Coils from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC1 （从主设备读取线圈），起始地址：" + startingAddress + ", 数量: " + quantity, System.DateTime.Now);
            内部事务标识码++;
            if (串口 != null)
                if (!串口.IsOpen)
                {
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开，异常抛出", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("抛出连接异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
            if (startingAddress > 65535 | quantity > 2000)
            {
                //if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
                //throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
                if (debug) StoreLogData.Instance.Store("参数异常抛出", System.DateTime.Now);
                throw new ArgumentException("起始地址必须为0-65535；数量必须为0-2000");
            }
            bool[] response;
            this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
            this.协议标识符 = BitConverter.GetBytes((int)0x0000);
            this.长度 = BitConverter.GetBytes((int)0x0006);
            this.功能码 = 0x01;
            this.起始地址 = BitConverter.GetBytes(startingAddress);
            this.数量 = BitConverter.GetBytes(quantity);
            Byte[] data = new byte[] {
                            this.事务标识符[1],this.事务标识符[0],this.协议标识符[1],this.协议标识符[0], this.长度[1],this.长度[0],
                            this.单元标识符, this.功能码,this.起始地址[1],this.起始地址[0], this.数量[1],this.数量[0], this.crc[0],this.crc[1]
            };

            crc = BitConverter.GetBytes(CRC(data, 6, 6));
            data[12] = crc[0];
            data[13] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                if (quantity % 8 == 0)
                    读字节数 = 5 + quantity / 8;
                else
                    读字节数 = 6 + quantity / 8;
                //serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, 8);
                if (debug)
                {
                    byte[] debugData = new byte[8];
                    Array.Copy(data, 6, debugData, 0, 8);
                    StoreLogData.Instance.Store("发送串行数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
                if (发送数据已更改事件 != null)
                {
                    发送数据 = new byte[8];
                    Array.Copy(data, 6, 发送数据, 0, 8);
                    发送数据已更改事件(this);
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;
                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                    while (!是否收到数据 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                        System.Threading.Thread.Sleep(1);
                    data = new byte[2100];

                    Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                    receivedUnitIdentifier = data[6];
                }
                if (receivedUnitIdentifier != this.单元标识符)
                    data = new byte[2100];
                else
                    重试计数 = 0;
            }
            else if (tcpClient.Client.Connected | UDP标志)
            {
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length - 2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length - 2);
                    if (debug)
                    {
                        byte[] debugData = new byte[data.Length - 2];
                        Array.Copy(data, 0, debugData, 0, data.Length - 2);
                        //StoreLogData.Instance.Store("Send MocbusTCP-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        StoreLogData.Instance.Store("MocbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                    if (发送数据已更改事件 != null)
                    {
                        发送数据 = new byte[data.Length - 2];
                        Array.Copy(data, 0, 发送数据, 0, data.Length - 2);
                        发送数据已更改事件(this);
                    }
                    data = new Byte[2100];
                    int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
                    {
                        接收数据 = new byte[NumberOfBytes];
                        Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
                    }
                }
            }
            if (data[7] == 0x81 & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("函数不支持功能代码");
            }
            if (data[7] == 0x81 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x81 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x81 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus 抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取错误");
            }

            if (串口 != null)
            {
                crc = BitConverter.GetBytes(CRC(data, (ushort)(data[8] + 3), 6));
                if ((crc[0] != data[data[8] + 9] | crc[1] != data[data[8] + 10]) & 是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("CRC检查失败，引发异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("回复CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        return 读线圈(startingAddress, quantity);
                    }
                }
                else if (!是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");
                    }
                    else
                    {
                        重试计数++;
                        return 读线圈(startingAddress, quantity);
                    }
                }
            }
            response = new bool[quantity];
            for (int i = 0; i < quantity; i++)
            {
                int intData = data[9 + i / 8];
                int mask = Convert.ToInt32(Math.Pow(2, (i % 8)));
                response[i] = Convert.ToBoolean((intData & mask) / mask);
            }
            return (response);
        }

        /// <summary> 从主机读取离散输入（FC2） Read Discrete Inputs from Server device (FC2) </summary>
        /// <param name="startingAddress">First discrete input to read</param>
        /// <param name="quantity">Number of discrete Inputs to read</param>
        /// <returns>Boolean Array which contains the discrete Inputs</returns>
        public bool[] 读取离散输入(int startingAddress, int quantity)//ReadDiscreteInputs
        {
            //if (debug) StoreLogData.Instance.Store("FC2 (Read Discrete Inputs from Master device), StartingAddress: "+ startingAddress+", Quantity: " +quantity, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC2 （从主设备读取离散输入），起始地址：" + startingAddress + ", 数量: " + quantity, System.DateTime.Now);
            内部事务标识码++;
            if (串口 != null)
                if (!串口.IsOpen)
                {
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //      throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开，抛出异常", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //            throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("抛出连接异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("connection error");
            }
            if (startingAddress > 65535 | quantity > 2000)
            {
                //if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
                //throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
                if (debug) StoreLogData.Instance.Store("参数抛出异常", System.DateTime.Now);
                throw new ArgumentException("起始地址必须为0-65535；数量必须为0-2000");
            }
            bool[] response;
            this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
            this.协议标识符 = BitConverter.GetBytes((int)0x0000);
            this.长度 = BitConverter.GetBytes((int)0x0006);
            this.功能码 = 0x02;
            this.起始地址 = BitConverter.GetBytes(startingAddress);
            this.数量 = BitConverter.GetBytes(quantity);
            Byte[] data = new byte[]  {
                            this.事务标识符[1], this.事务标识符[0], this.协议标识符[1], this.协议标识符[0], this.长度[1], this.长度[0],
                            this.单元标识符, this.功能码, this.起始地址[1], this.起始地址[0], this.数量[1], this.数量[0], this.crc[0], this.crc[1]
             };
            crc = BitConverter.GetBytes(CRC(data, 6, 6));
            data[12] = crc[0];
            data[13] = crc[1];

            if (串口 != null)
            {
                是否收到数据 = false;
                if (quantity % 8 == 0)
                    读字节数 = 5 + quantity / 8;
                else
                    读字节数 = 6 + quantity / 8;
                //               serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, 8);
                if (debug)
                {
                    byte[] debugData = new byte[8];
                    Array.Copy(data, 6, debugData, 0, 8);
                    //if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("发送串行数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
                if (发送数据已更改事件 != null)
                {
                    发送数据 = new byte[8];
                    Array.Copy(data, 6, 发送数据, 0, 8);
                    发送数据已更改事件(this);
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;

                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                    while (是否收到数据 == false & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                        System.Threading.Thread.Sleep(1);
                    data = new byte[2100];
                    Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                    receivedUnitIdentifier = data[6];
                }
                if (receivedUnitIdentifier != this.单元标识符)
                    data = new byte[2100];
                else
                    重试计数 = 0;
            }
            else if (tcpClient.Client.Connected | UDP标志)
            {
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length - 2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 2000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length - 2);
                    if (debug)
                    {
                        byte[] debugData = new byte[data.Length - 2];
                        Array.Copy(data, 0, debugData, 0, data.Length - 2);
                        //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                    if (发送数据已更改事件 != null)
                    {
                        发送数据 = new byte[data.Length - 2];
                        Array.Copy(data, 0, 发送数据, 0, data.Length - 2);
                        发送数据已更改事件(this);
                    }
                    data = new Byte[2100];
                    int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
                    {
                        接收数据 = new byte[NumberOfBytes];
                        Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
                    }
                }
            }
            if (data[7] == 0x82 & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("函数不支持功能代码");
            }
            if (data[7] == 0x82 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x82 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x82 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus异常抛出", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取错误");
            }
            if (串口 != null)
            {
                crc = BitConverter.GetBytes(CRC(data, (ushort)(data[8] + 3), 6));
                if ((crc[0] != data[data[8] + 9] | crc[1] != data[data[8] + 10]) & 是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("CRC检查失败，引发异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("回复CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        return 读取离散输入(startingAddress, quantity);
                    }
                }
                else if (!是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");
                    }
                    else
                    {
                        重试计数++;
                        return 读取离散输入(startingAddress, quantity);
                    }
                }
            }
            response = new bool[quantity];
            for (int i = 0; i < quantity; i++)
            {
                int intData = data[9 + i / 8];
                int mask = Convert.ToInt32(Math.Pow(2, (i % 8)));
                response[i] = Convert.ToBoolean((intData & mask) / mask);
            }
            return response;
        }

        /// <summary> 从主设备读取保持寄存器（FC3 Read Holding Registers from Master device (FC3) </summary>
        /// <param name="起始地址">First holding register to be read</param>
        /// <param name="数量">Number of holding registers to be read</param>
        /// <returns>Int Array which contains the holding registers</returns>
        public int[] 读保持寄存器(int 起始地址, int 数量)//ReadHoldingRegisters
        {
            //if (debug) StoreLogData.Instance.Store("FC3 (Read Holding Registers from Master device), StartingAddress: "+ startingAddress+", Quantity: " +quantity, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC3 （从主设备读取保持寄存器），起始地址：" + 起始地址 + ", 数量: " + 数量, System.DateTime.Now);
            内部事务标识码++;
            if (串口 != null)
                if (!串口.IsOpen)
            	{
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开，异常抛出", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("连接抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
			if (起始地址 > 65535 | 数量 > 125)
			{
                //if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
                //throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 125");
                if (debug) StoreLogData.Instance.Store("参数抛出异常", System.DateTime.Now);
                throw new ArgumentException("起始地址必须为0-65535；数量必须为0-125");
            }

			int[] 响应数据;
			this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
			this.协议标识符 = BitConverter.GetBytes((int) 0x0000);
			this.长度 = BitConverter.GetBytes((int)0x0006);
			this.功能码 = 0x03;
			this.起始地址 = BitConverter.GetBytes(起始地址);
			this.数量 = BitConverter.GetBytes(数量);
			Byte[] data = new byte[]{	this.事务标识符[1], this.事务标识符[0], this.协议标识符[1], this.协议标识符[0], this.长度[1], this.长度[0],
							this.单元标识符, this.功能码, this.起始地址[1], this.起始地址[0], this.数量[1], this.数量[0], this.crc[0], this.crc[1]
            };

            crc = BitConverter.GetBytes(CRC(data, 6, 6));
            data[12] = crc[0];
            data[13] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                读字节数 = 5 + 2 * 数量;
                //serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, 8);
                if (debug)
                {
                	byte [] 调试数据 = new byte[8];
            		Array.Copy(data, 6, 调试数据, 0, 8);
                    //if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("发送串行数据：" + BitConverter.ToString(调试数据), System.DateTime.Now);
                }
               if (发送数据已更改事件 != null)
            	{
                    发送数据 = new byte[8];
            		Array.Copy(data, 6, 发送数据, 0, 8);
                    发送数据已更改事件(this);
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                
                DateTime 发送时间 = DateTime.Now;
                byte 接收单元标识符 = 0xFF;
                while (接收单元标识符 != this.单元标识符 & !((DateTime.Now.Ticks - 发送时间.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                	while (!是否收到数据 & !((DateTime.Now.Ticks - 发送时间.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                    	System.Threading.Thread.Sleep(1);//延时1MS

                    data = new byte[2100];
                	Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);

                    接收单元标识符 = data[6];
                }
                if (接收单元标识符 != this.单元标识符)
                	data = new byte[2100];
                else
                    重试计数 = 0;
            }
			else if (tcpClient.Client.Connected | UDP标志)
			{
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length-2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length - 2);//发送
                    if (debug)
                    {
                        byte[] 调试数据 = new byte[data.Length - 2];
                        Array.Copy(data, 0, 调试数据, 0, data.Length - 2);
                        //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(调试数据), System.DateTime.Now);
                    }
                    if (发送数据已更改事件 != null)//添加发送数据事件
                    {
                        发送数据 = new byte[data.Length - 2];
                        Array.Copy(data, 0, 发送数据, 0, data.Length - 2);
                        发送数据已更改事件(this);//添加发送数据事件

                    }
                    data = new Byte[256];
                    int 字节数 = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)//添加接收数据事件
                    {
                        接收数据 = new byte[字节数];
                        Array.Copy(data, 0, 接收数据, 0, 字节数);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);//添加接收数据事件
                    }
                }
            }
            if (data[7] == 0x83 & data[8] == 0x01)
            {
            	if (debug) StoreLogData.Instance.Store("不支持函数代码,抛出异常抛出", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("函数不支持功能代码");
                
            }
            if (data[7] == 0x83 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，异常抛出", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x83 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x83 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus异常抛出", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取时出错");
            }
            if (串口 != null)
            {
                crc = BitConverter.GetBytes(CRC(data, (ushort)(data[8] + 3), 6));
                if ((crc[0] != data[data[8]+9] | crc[1] != data[data[8]+10])& 是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("CRC检查失败，引发异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("响应的CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        return 读保持寄存器(起始地址, 数量);
                    }
                }
                else if (!是否收到数据)//没收到的数据
                {
                	//if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);

                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;//重试次数
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");
                    }
                    else
                    {
                        重试计数++;//重试次数
                        return 读保持寄存器(起始地址, 数量);//重试
                    }
                }
            }

            if (data.Length < 15)
            {
                throw new ArgumentException("没接收到数据");
            }

            响应数据 = new int[数量];
			for (int i = 0; i < 数量; i++)
			{
				byte lowByte;
				byte highByte;
				highByte = data[9+i*2];
				lowByte = data[9+i*2+1];
				
				data[9+i*2] = lowByte;
				data[9+i*2+1] = highByte;

                响应数据[i] = BitConverter.ToInt16(data,(9+i*2));
			}			
    		return (响应数据);			
		}

        /// <summary> 从主设备读取输入寄存器 FC4）Read Input Registers from Master device (FC4) </summary>
        /// <param name="startingAddress">First input register to be read</param>
        /// <param name="quantity">Number of input registers to be read</param>
        /// <returns>Int Array which contains the input registers</returns>
        public int[] 读输入寄存器(int startingAddress, int quantity)//ReadInputRegisters
        {
            //if (debug) StoreLogData.Instance.Store("FC4 (Read Input Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC4 (从主设备读取输入寄存器), 起始地址: " + startingAddress + ", 数量: " + quantity, System.DateTime.Now);
            内部事务标识码++;

            if (串口 != null)
                if (!串口.IsOpen)
            	{
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开，异常抛出", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //            throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("连接抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
			if (startingAddress > 65535 | quantity >125)
			{
                //if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
                //throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 125");
                if (debug) StoreLogData.Instance.Store("参数异常抛出", System.DateTime.Now);
                throw new ArgumentException("起始地址必须为0-65535；数量必须为0-125");
            }
			int[] response;
			this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
			this.协议标识符 = BitConverter.GetBytes((int) 0x0000);
			this.长度 = BitConverter.GetBytes((int)0x0006);
			this.功能码 = 0x04;
			this.起始地址 = BitConverter.GetBytes(startingAddress);
			this.数量 = BitConverter.GetBytes(quantity);
            Byte[] data = new byte[]{
                this.事务标识符[1], this.事务标识符[0], this.协议标识符[1], this.协议标识符[0], this.长度[1], this.长度[0],
                this.单元标识符,  this.功能码, this.起始地址[1], this.起始地址[0], this.数量[1], this.数量[0], this.crc[0], this.crc[1]
            };
            crc = BitConverter.GetBytes(CRC(data, 6, 6));
            data[12] = crc[0];
            data[13] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                读字节数 = 5 + 2 * quantity;

                //               serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, 8);
                if (debug)
                {
                	byte [] debugData = new byte[8];
            		Array.Copy(data, 6, debugData, 0, 8);
                    //if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);          		
                    if (debug) StoreLogData.Instance.Store("发送串行数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
               if (发送数据已更改事件 != null)
            	{
                    发送数据 = new byte[8];
            		Array.Copy(data, 6, 发送数据, 0, 8);
                    发送数据已更改事件(this);
                    
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;
                
                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                	while (!是否收到数据 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                    	System.Threading.Thread.Sleep(1);  
                	data = new byte[2100];
                	Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                	receivedUnitIdentifier = data[6];
                }
               
                if (receivedUnitIdentifier != this.单元标识符)
              	 	data = new byte[2100];   
                else
                    重试计数 = 0;
            }
			else if (tcpClient.Client.Connected | UDP标志)
			{
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length-2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length-2);
                    if (debug)
                    {
                	    byte [] debugData = new byte[data.Length-2];
            		    Array.Copy(data, 0, debugData, 0, data.Length-2);
                        //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);          		
                        if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                     if (发送数据已更改事件 != null)
            		{
                        发送数据 = new byte[data.Length-2];
            			Array.Copy(data, 0, 发送数据, 0, data.Length-2);
                        发送数据已更改事件(this);
            		}
                    data = new Byte[2100];
                    int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
            		{
                        接收数据 = new byte[NumberOfBytes];
            			Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
            		}

                }
			}
            if (data[7] == 0x84 & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Master不支持功能代码");
            }
            if (data[7] == 0x84 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效异常抛出", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x84 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x84 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("异常抛出", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取时错误");
            }
            if (串口 != null)
            {
            crc = BitConverter.GetBytes(CRC(data, (ushort)(data[8]+3), 6));
                if ((crc[0] != data[data[8]+9] | crc[1] != data[data[8]+10]) & 是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("CRC检查失败，引发异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("回复CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        return 读输入寄存器(startingAddress, quantity);
                    }
                }
                else if (!是否收到数据)
                {
                	//if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");
                    }
                    else
                    {
                        重试计数++;
                        return 读输入寄存器(startingAddress, quantity);
                    }
                }
            }
			response = new int[quantity];
			for (int i = 0; i < quantity; i++)
			{
				byte lowByte;
				byte highByte;
				highByte = data[9+i*2];
				lowByte = data[9+i*2+1];
				
				data[9+i*2] = lowByte;
				data[9+i*2+1] = highByte;
				
				response[i] = BitConverter.ToInt16(data,(9+i*2));
			}
    		return (response);
		 }

        /// <summary> 将单线圈写入主设备（FC5）Write single Coil to Master device (FC5) </summary>
        /// <param name="startingAddress">Coil to be written</param>
        /// <param name="value">Coil Value to be written</param>
        public void 写单线圈(int startingAddress, bool value) //WriteSingleCoil
        {
            //if (debug) StoreLogData.Instance.Store("FC5 (Write single coil to Master device), StartingAddress: "+ startingAddress+", Value: " + value, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC5 （将单线圈写入主设备），起始地址：" + startingAddress + ", Value: " + value, System.DateTime.Now);
            内部事务标识码++;
            if (串口 != null)
                if (!串口.IsOpen)
                {
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开，异常抛出", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("连接抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
            byte[] coilValue = new byte[2];
            this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
            this.协议标识符 = BitConverter.GetBytes((int)0x0000);
            this.长度 = BitConverter.GetBytes((int)0x0006);
            this.功能码 = 0x05;
            this.起始地址 = BitConverter.GetBytes(startingAddress);
            if (value == true)
                coilValue = BitConverter.GetBytes((int)0xFF00);
            else
                coilValue = BitConverter.GetBytes((int)0x0000);

            Byte[] data = new byte[]{   this.事务标识符[1], this.事务标识符[0], this.协议标识符[1], this.协议标识符[0], this.长度[1], this.长度[0],
                            this.单元标识符, this.功能码, this.起始地址[1], this.起始地址[0], coilValue[1], coilValue[0], this.crc[0], this.crc[1]
             };
            crc = BitConverter.GetBytes(CRC(data, 6, 6));
            data[12] = crc[0];
            data[13] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                读字节数 = 8;
                //serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, 8);
                if (debug)
                {
                    byte[] debugData = new byte[8];
                    Array.Copy(data, 6, debugData, 0, 8);
                    //if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("发送串行数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
                if (发送数据已更改事件 != null)
                {
                    发送数据 = new byte[8];
                    Array.Copy(data, 6, 发送数据, 0, 8);
                    发送数据已更改事件(this);
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;
                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                    while (!是否收到数据 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                        System.Threading.Thread.Sleep(1);
                    data = new byte[2100];
                    Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                    receivedUnitIdentifier = data[6];
                }

                if (receivedUnitIdentifier != this.单元标识符)
                    data = new byte[2100];
                else
                    重试计数 = 0;
            }
            else if (tcpClient.Client.Connected | UDP标志)
            {
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length - 2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length - 2);
                    if (debug)
                    {
                        byte[] debugData = new byte[data.Length - 2];
                        Array.Copy(data, 0, debugData, 0, data.Length - 2);
                        //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                    if (发送数据已更改事件 != null)
                    {
                        发送数据 = new byte[data.Length - 2];
                        Array.Copy(data, 0, 发送数据, 0, data.Length - 2);
                        发送数据已更改事件(this);
                    }
                    data = new Byte[2100];
                    int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
                    {
                        接收数据 = new byte[NumberOfBytes];
                        Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
                    }
                }
            }
            if (data[7] == 0x85 & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("函数不支持功能代码");
            }
            if (data[7] == 0x85 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x85 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("的数量无效,抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x85 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取错误");
            }
            if (串口 != null)
            {
                crc = BitConverter.GetBytes(CRC(data, 6, 6));
                if ((crc[0] != data[12] | crc[1] != data[13]) & 是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("CRC检查失败，引发异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("响应CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        写单线圈(startingAddress, value);
                    }
                }
                else if (!是否收到数据)
                {
                    //if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");
                    }
                    else
                    {
                        重试计数++;
                        写单线圈(startingAddress, value);
                    }
                }
            }
        }

        /// <summary> 将单个寄存器写入主设备（FC6）Write single Register to Master device (FC6) </summary>
        /// <param name="startingAddress">Register to be written</param>
        /// <param name="value">Register Value to be written</param>
        public void 写单寄存器(int startingAddress, int value)// WriteSingleRegister
        {
            //if (debug) StoreLogData.Instance.Store("FC6 (Write single register to Master device), StartingAddress: "+ startingAddress+", Value: " + value, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC6 (将单个寄存器写入主设备),起始地址：" + startingAddress + ", Value: " + value, System.DateTime.Now);
            内部事务标识码++;
            if (串口 != null)
                if (!串口.IsOpen)
            	{
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //      throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开,抛出异常", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("连接抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
            byte[] registerValue = new byte[2];
            this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
            this.协议标识符 = BitConverter.GetBytes((int)0x0000);
            this.长度 = BitConverter.GetBytes((int)0x0006);
            this.功能码 = 0x06;
            this.起始地址 = BitConverter.GetBytes(startingAddress);
            registerValue = BitConverter.GetBytes((int)value);

            Byte[] data = new byte[]{   this.事务标识符[1], this.事务标识符[0], this.协议标识符[1], this.协议标识符[0], this.长度[1], this.长度[0],
                            this.单元标识符, this.功能码, this.起始地址[1], this.起始地址[0], registerValue[1], registerValue[0], this.crc[0], this.crc[1]
                            };

            crc = BitConverter.GetBytes(CRC(data, 6, 6));
            data[12] = crc[0];
            data[13] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                读字节数 = 8;
                //serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, 8);
                if (debug)
                {
                	byte [] debugData = new byte[8];
            		Array.Copy(data, 6, debugData, 0, 8);
                    //if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("发送串行数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
               if (发送数据已更改事件 != null)
            	{
                    发送数据 = new byte[8];
            		Array.Copy(data, 6, 发送数据, 0, 8);
                    发送数据已更改事件(this);
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;
                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                    while (!是否收到数据 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                        System.Threading.Thread.Sleep(1);
                    data = new byte[2100];
                    Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                    receivedUnitIdentifier = data[6];
                }
                if (receivedUnitIdentifier != this.单元标识符)
                    data = new byte[2100];
                else
                    重试计数 = 0;
            }
            else if (tcpClient.Client.Connected | UDP标志)
            {
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length - 2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length - 2);
                    if (debug)
                    {
                        byte[] debugData = new byte[data.Length - 2];
                        Array.Copy(data, 0, debugData, 0, data.Length - 2);
                        //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                    if (发送数据已更改事件 != null)
                    {
                        发送数据 = new byte[data.Length - 2];
                        Array.Copy(data, 0, 发送数据, 0, data.Length - 2);
                        发送数据已更改事件(this);
                    }
                    data = new Byte[2100];
                    int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
            		{
                        接收数据 = new byte[NumberOfBytes];
            			Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
            		}
                }
            }
            if (data[7] == 0x86 & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("函数不支持功能代码");
            }
            if (data[7] == 0x86 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x86 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x86 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取错误");
            }
            if (串口 != null)
            {
             crc = BitConverter.GetBytes(CRC(data, 6, 6));           
             if ((crc[0] != data[12] | crc[1] != data[13]) & 是否收到数据)
             {
                    //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("CRC检查失败，引发异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("响应CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        写单寄存器(startingAddress, value);
                    }
                }
             else if (!是否收到数据)
             {
                	//if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");

                    }
                    else
                    {
                        重试计数++;
                        写单寄存器(startingAddress, value);
                    }
                }
            }
        }

        /// <summary> 写多个线圈（FC15）Write multiple coils to Master device (FC15) </summary>
        /// <param name="startingAddress">First coil to be written</param>
        /// <param name="values">Coil Values to be written</param>
        public void 写多线圈(int startingAddress, bool[] values) //WriteMultipleCoils
        {
            string debugString = "";
        	for (int i = 0; i < values.Length;i++)
        		debugString = debugString + values[i] + " ";
            //if (debug) StoreLogData.Instance.Store("FC15 (Write multiple coils to Master device), StartingAddress: "+ startingAddress+", Values: " + debugString, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC15 (将多个线圈写入主设备), 起始地址：" + startingAddress + ", Values: " + debugString, System.DateTime.Now);
            内部事务标识码++;
            byte byteCount = (byte)((values.Length % 8 != 0 ? values.Length / 8 + 1: (values.Length / 8)));
            byte[] quantityOfOutputs = BitConverter.GetBytes((int)values.Length);
            byte singleCoilValue = 0;
            if (串口 != null)
                if (!串口.IsOpen)
            	{
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开，异常抛出", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("抛出连接异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
            this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
            this.协议标识符 = BitConverter.GetBytes((int)0x0000);
            this.长度 = BitConverter.GetBytes((int)(7+(byteCount)));
            this.功能码 = 0x0F;
            this.起始地址 = BitConverter.GetBytes(startingAddress);

            Byte[] data = new byte[14 +2 + (values.Length % 8 != 0 ? values.Length/8 : (values.Length / 8)-1)];
            data[0] = this.事务标识符[1];
            data[1] = this.事务标识符[0];
            data[2] = this.协议标识符[1];
	        data[3] = this.协议标识符[0];
			data[4] = this.长度[1];
			data[5] = this.长度[0];
			data[6] = this.单元标识符;
			data[7] = this.功能码;
			data[8] = this.起始地址[1];
			data[9] = this.起始地址[0];
            data[10] = quantityOfOutputs[1];
            data[11] = quantityOfOutputs[0];
            data[12] = byteCount;
            for (int i = 0; i < values.Length; i++)
            {
                if ((i % 8) == 0)
                    singleCoilValue = 0;
                byte CoilValue;
                if (values[i] == true)
                    CoilValue = 1;
                else
                    CoilValue = 0;

                singleCoilValue = (byte)((int)CoilValue<<(i%8) | (int)singleCoilValue);

                data[13 + (i / 8)] = singleCoilValue;            
            }
            crc = BitConverter.GetBytes(CRC(data, (ushort)(data.Length - 8), 6));
            data[data.Length - 2] = crc[0];
            data[data.Length - 1] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                读字节数 = 8;
                //               serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, data.Length - 6);
                if (debug)
                {
                	byte [] debugData = new byte[data.Length - 6];
            		Array.Copy(data, 6, debugData, 0, data.Length - 6);
            		//if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("串行发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
               if (发送数据已更改事件 != null)
            	{
                    发送数据 = new byte[data.Length - 6];
            		Array.Copy(data, 6, 发送数据, 0, data.Length - 6);
                    发送数据已更改事件(this);
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;
                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                	while (!是否收到数据 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                    	System.Threading.Thread.Sleep(1);  
                	data = new byte[2100];
                	Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                	receivedUnitIdentifier = data[6];
                }
                if (receivedUnitIdentifier != this.单元标识符)
                	data = new byte[2100];     
                else
                    重试计数 = 0;
            }
            else if (tcpClient.Client.Connected | UDP标志)
            {
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length-2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);                   
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length-2);
                    if (debug)
                    {
                        byte[] debugData = new byte[data.Length - 2];
                        Array.Copy(data, 0, debugData, 0, data.Length - 2);
                        //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                    if (发送数据已更改事件 != null)
            		{
                        发送数据 = new byte[data.Length-2];
            			Array.Copy(data, 0, 发送数据, 0, data.Length-2);
                        发送数据已更改事件(this);
                    }                    
                    data = new Byte[2100];
                    int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
            		{
                        接收数据 = new byte[NumberOfBytes];
            			Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
            		}
                }
            }
            if (data[7] == 0x8F & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("函数不支持功能代码");
            }
            if (data[7] == 0x8F & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x8F & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x8F & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取错误");
            }
            if (串口 != null)
            {
             crc = BitConverter.GetBytes(CRC(data, 6, 6));           
             if ((crc[0] != data[12] | crc[1] != data[13]) & 是否收到数据)
             {
                //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                  if (debug) StoreLogData.Instance.Store("CRC检查失败，抛出异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("回答CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        写多线圈(startingAddress, values);
                    }
                }
             else if (!是否收到数据)
              {
                	//if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");

                    }
                    else
                    {
                        重试计数++;
                        写多线圈(startingAddress, values);
                    }
                }
            }
        }

        /// <summary> 将多个寄存器写入主设备（FC16）Write multiple registers to Master device (FC16) </summary>
        /// <param name="startingAddress">First register to be written</param>
        /// <param name="values">register Values to be written</param>
        public void 写多寄存器(int startingAddress, int[] values) // WriteMultipleRegisters
        {
            string debugString = "";
        	for (int i = 0; i < values.Length;i++)
        		debugString = debugString + values[i] + " ";
            //if (debug) StoreLogData.Instance.Store("FC16 (Write multiple Registers to Server device), StartingAddress: "+ startingAddress+", Values: " + debugString, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC16（将多个寄存器写入服务器设备），起始地址：" + startingAddress + ", Values: " + debugString, System.DateTime.Now);
            内部事务标识码++;
            byte byteCount = (byte)(values.Length * 2);
            byte[] quantityOfOutputs = BitConverter.GetBytes((int)values.Length);
            if (串口 != null)
                if (!串口.IsOpen)
            	{
            		//if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串行端口未打开，抛出异常", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("连接错误，抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
            this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
            this.协议标识符 = BitConverter.GetBytes((int)0x0000);
            this.长度 = BitConverter.GetBytes((int)(7+values.Length*2));
            this.功能码 = 0x10;
            this.起始地址 = BitConverter.GetBytes(startingAddress);

            Byte[] data = new byte[13+2 + values.Length*2];
            data[0] = this.事务标识符[1];
            data[1] = this.事务标识符[0];
            data[2] = this.协议标识符[1];
            data[3] = this.协议标识符[0];
            data[4] = this.长度[1];
            data[5] = this.长度[0];
            data[6] = this.单元标识符;
            data[7] = this.功能码;
            data[8] = this.起始地址[1];
            data[9] = this.起始地址[0];
            data[10] = quantityOfOutputs[1];
            data[11] = quantityOfOutputs[0];
            data[12] = byteCount;
            for (int i = 0; i < values.Length; i++)
            {
                byte[] singleRegisterValue = BitConverter.GetBytes((int)values[i]);
                data[13 + i*2] = singleRegisterValue[1];
                data[14 + i*2] = singleRegisterValue[0];
            }
            crc = BitConverter.GetBytes(CRC(data, (ushort)(data.Length - 8), 6));
            data[data.Length - 2] = crc[0];
            data[data.Length - 1] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                读字节数 = 8;
                //serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, data.Length - 6);

                if (debug)
                {
                	byte [] debugData = new byte[data.Length - 6];
            		Array.Copy(data, 6, debugData, 0, data.Length - 6);
                    //if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("串行发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
               if (发送数据已更改事件 != null)
            	{
                    发送数据 = new byte[data.Length - 6];
            		Array.Copy(data, 6, 发送数据, 0, data.Length - 6);
                    发送数据已更改事件(this);
                   
                }
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;
                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                	while (!是否收到数据 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                    	System.Threading.Thread.Sleep(1);  
                	data = new byte[2100];
                	Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                	receivedUnitIdentifier = data[6];
                }
                if (receivedUnitIdentifier != this.单元标识符)
                	data = new byte[2100];    
                else
                    重试计数 = 0;
            }
            else if (tcpClient.Client.Connected | UDP标志)
            {
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length-2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length-2);
                    if (debug)
                {
                	byte [] debugData = new byte[data.Length-2];
            		Array.Copy(data, 0, debugData, 0, data.Length-2);
                    //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                     if (发送数据已更改事件 != null)
            		{
                        发送数据 = new byte[data.Length-2];
            			Array.Copy(data, 0, 发送数据, 0, data.Length-2);
                        发送数据已更改事件(this);
            		}                   
                    data = new Byte[2100];
                    int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
            		{
                        接收数据 = new byte[NumberOfBytes];
            			Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
            		}
                }
            }
            if (data[7] == 0x90 & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("函数不支持功能代码");
            }
            if (data[7] == 0x90 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x90 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效, 抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x90 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取错误");
            }
            if (串口 != null)
            {
             crc = BitConverter.GetBytes(CRC(data, 6, 6));           
             if ((crc[0] != data[12] | crc[1] != data[13])  & 是否收到数据)
             {
                    //if (debug) StoreLogData.Instance.Store("CRCCheckFailedException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("CRC检查失败，引发异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new EasyModbus.Exceptions.CRCCheckFailedException("Response CRC check failed");
                        throw new EasyModbus.Exceptions.CRCCheckFailedException("响应CRC检查失败");
                    }
                    else
                    {
                        重试计数++;
                        写多寄存器(startingAddress, values);
                    }
                }
             else if (!是否收到数据)
             {
                	//if (debug) StoreLogData.Instance.Store("TimeoutException Throwed", System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("抛出超时异常", System.DateTime.Now);
                    if (重试次数 <= 重试计数)
                    {
                        重试计数 = 0;
                        //throw new TimeoutException("No Response from Modbus Slave");
                        throw new TimeoutException("Modbus从站无响应");
                    }
                    else
                    {
                        重试计数++;
                        写多寄存器(startingAddress, values);
                    }
                }
            }
        }

        /// <summary> 读/写多个寄存器(FC23) Read/Write Multiple Registers (FC23) </summary>
        /// <param name="startingAddressRead">First input register to read</param>
        /// <param name="quantityRead">Number of input registers to read</param>
        /// <param name="startingAddressWrite">First input register to write</param>
        /// <param name="values">Values to write</param>
        /// <returns>Int Array which contains the Holding registers</returns>
        public int[] 读写多寄存器(int startingAddressRead, int quantityRead, int startingAddressWrite, int[] values) //ReadWriteMultipleRegisters
        {
            string debugString = "";
        	for (int i = 0; i < values.Length;i++)
        		debugString = debugString + values[i] + " ";
            //if (debug) StoreLogData.Instance.Store("FC23 (Read and Write multiple Registers to Server device), StartingAddress Read: "+ startingAddressRead+ ", Quantity Read: "+quantityRead+", startingAddressWrite: " + startingAddressWrite +", Values: " + debugString, System.DateTime.Now);
            if (debug) StoreLogData.Instance.Store("FC23 (向服务器设备读写多个寄存器), 开始地址读取：" + startingAddressRead + ", 读取的数量：" + quantityRead + ", 起始地址写入：" + startingAddressWrite + ", Values: " + debugString, System.DateTime.Now);
            内部事务标识码++;
            byte [] startingAddressReadLocal = new byte[2];
		    byte [] quantityReadLocal = new byte[2];
            byte[] startingAddressWriteLocal = new byte[2];
            byte[] quantityWriteLocal = new byte[2];
            byte writeByteCountLocal = 0;
            if (串口 != null)
                if (!串口.IsOpen)
            	{
                    //if (debug) StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", System.DateTime.Now);
                    //      throw new EasyModbus.Exceptions.SerialPortNotOpenedException("serial port not opened");
                    if (debug) StoreLogData.Instance.Store("串口未打开, 抛出异常", System.DateTime.Now);
                    throw new EasyModbus.Exceptions.SerialPortNotOpenedException("串口未打开");
                }
            if (tcpClient == null & !UDP标志 & 串口 == null)
            {
                //if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
                //            throw new EasyModbus.Exceptions.ConnectionException("connection error");
                if (debug) StoreLogData.Instance.Store("连接抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ConnectionException("连接错误");
            }
            if (startingAddressRead > 65535 | quantityRead > 125 | startingAddressWrite > 65535 | values.Length > 121)
            {
                //if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
                //throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
                if (debug) StoreLogData.Instance.Store("参数异常抛出", System.DateTime.Now);
                throw new ArgumentException("起始地址必须为0-65535；数量必须为0-2000");
            }
            int[] response;
            this.事务标识符 = BitConverter.GetBytes((uint)内部事务标识码);
            this.协议标识符 = BitConverter.GetBytes((int)0x0000);
            this.长度 = BitConverter.GetBytes((int)11 + values.Length * 2);
            this.功能码 = 0x17;
            startingAddressReadLocal = BitConverter.GetBytes(startingAddressRead);
            quantityReadLocal = BitConverter.GetBytes(quantityRead);
            startingAddressWriteLocal = BitConverter.GetBytes(startingAddressWrite);
            quantityWriteLocal = BitConverter.GetBytes(values.Length);
            writeByteCountLocal = Convert.ToByte(values.Length * 2);
            Byte[] data = new byte[17 +2+ values.Length*2];
            data[0] =               this.事务标识符[1];
            data[1] =   		    this.事务标识符[0];
			data[2] =   			this.协议标识符[1];
			data[3] =   			this.协议标识符[0];
			data[4] =   			this.长度[1];
			data[5] =   			this.长度[0];
			data[6] =   			this.单元标识符;
			data[7] =   		    this.功能码;
			data[8] =   			startingAddressReadLocal[1];
			data[9] =   			startingAddressReadLocal[0];
			data[10] =   			quantityReadLocal[1];
			data[11] =   			quantityReadLocal[0];
            data[12] =               startingAddressWriteLocal[1];
			data[13] =   			startingAddressWriteLocal[0];
			data[14] =   			quantityWriteLocal[1];
			data[15] =   			quantityWriteLocal[0];
            data[16] =              writeByteCountLocal;

            for (int i = 0; i < values.Length; i++)
            {
                byte[] singleRegisterValue = BitConverter.GetBytes((int)values[i]);
                data[17 + i*2] = singleRegisterValue[1];
                data[18 + i*2] = singleRegisterValue[0];
            }
            crc = BitConverter.GetBytes(CRC(data, (ushort)(data.Length - 8), 6));
            data[data.Length - 2] = crc[0];
            data[data.Length - 1] = crc[1];
            if (串口 != null)
            {
                是否收到数据 = false;
                读字节数 = 5 + 2*quantityRead;
                //               serialport.ReceivedBytesThreshold = bytesToRead;
                串口.Write(data, 6, data.Length - 6);
                if (debug)
                {
                	byte [] debugData = new byte[data.Length - 6];
            		Array.Copy(data, 6, debugData, 0, data.Length - 6);
                    //if (debug) StoreLogData.Instance.Store("Send Serial-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                    if (debug) StoreLogData.Instance.Store("发送串行数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                }
               if (发送数据已更改事件 != null)
            	{
                    发送数据 = new byte[data.Length - 6];
            		Array.Copy(data, 6, 发送数据, 0, data.Length - 6);
                    发送数据已更改事件(this);
            	}
                data = new byte[2100];
                读字节缓冲 = new byte[256];
                DateTime dateTimeSend = DateTime.Now;
                byte receivedUnitIdentifier = 0xFF;
                while (receivedUnitIdentifier != this.单元标识符 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                {
                	while (!是否收到数据 & !((DateTime.Now.Ticks - dateTimeSend.Ticks) > TimeSpan.TicksPerMillisecond * this.读超时))
                   	 	System.Threading.Thread.Sleep(1);  
                	data = new byte[2100];
                	Array.Copy(读字节缓冲, 0, data, 6, 读字节缓冲.Length);
                	receivedUnitIdentifier = data[6];
                }
                if (receivedUnitIdentifier != this.单元标识符)
              	 	data = new byte[2100];               
                else
                    重试计数 = 0;
            }
            else if (tcpClient.Client.Connected | UDP标志)
            {
                if (UDP标志)
                {
                    UdpClient udpClient = new UdpClient();
                    IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
                    udpClient.Send(data, data.Length-2, endPoint);
                    portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
                    udpClient.Client.ReceiveTimeout = 5000;
                    endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
                    data = udpClient.Receive(ref endPoint);
                }
                else
                {
                    Net基础流.Write(data, 0, data.Length - 2);
                    if (debug)
                    {
                        byte[] debugData = new byte[data.Length - 2];
                        Array.Copy(data, 0, debugData, 0, data.Length - 2);
                        //if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: "+BitConverter.ToString(debugData) ,System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP发送数据：" + BitConverter.ToString(debugData), System.DateTime.Now);
                    }
                    if (发送数据已更改事件 != null)
            		{
                        发送数据 = new byte[data.Length-2];
            			Array.Copy(data, 0, 发送数据, 0, data.Length-2);
                        发送数据已更改事件(this);
                        
                    }                   
                    data = new Byte[2100];
                     int NumberOfBytes = Net基础流.Read(data, 0, data.Length);
                    if (接收数据已更改事件 != null)
            		{
                        接收数据 = new byte[NumberOfBytes];
            			Array.Copy(data, 0, 接收数据, 0, NumberOfBytes);
                        //if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(接收数据), System.DateTime.Now);
                        if (debug) StoreLogData.Instance.Store("ModbusTCP接收数据：" + BitConverter.ToString(接收数据), System.DateTime.Now);
                        接收数据已更改事件(this);
            		}
                }
            }
            if (data[7] == 0x97 & data[8] == 0x01)
            {
                //if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
                //   throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
                if (debug) StoreLogData.Instance.Store("不支持函数代码，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.FunctionCodeNotSupportedException("主程序不支持功能代码");
            }
            if (data[7] == 0x97 & data[8] == 0x02)
            {
                //if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
                if (debug) StoreLogData.Instance.Store("起始地址无效，引发异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.StartingAddressInvalidException("起始地址无效或起始地址+数量无效");
            }
            if (data[7] == 0x97 & data[8] == 0x03)
            {
                //if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.QuantityInvalidException("quantity invalid");
                if (debug) StoreLogData.Instance.Store("数量无效, 抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.QuantityInvalidException("数量无效");
            }
            if (data[7] == 0x97 & data[8] == 0x04)
            {
                //if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
                //throw new EasyModbus.Exceptions.ModbusException("error reading");
                if (debug) StoreLogData.Instance.Store("Modbus抛出异常", System.DateTime.Now);
                throw new EasyModbus.Exceptions.ModbusException("读取错误");
            }
            response = new int[quantityRead];
            for (int i = 0; i < quantityRead; i++)
            {
                byte lowByte;
                byte highByte;
                highByte = data[9 + i * 2];
                lowByte = data[9 + i * 2 + 1];

                data[9 + i * 2] = lowByte;
                data[9 + i * 2 + 1] = highByte;

                response[i] = BitConverter.ToInt16(data, (9 + i * 2));
            }
            return (response);
        }
        #endregion
     

        /// <summary> 如果客户端连接到服务器，则返回“TRUE”，否则返回“FALSE”。如果Modbus RTU在COM端口打开时返回
        /// Returns "TRUE" if Client is connected to Server and "FALSE" if not. In case of Modbus RTU returns if COM-Port is opened
        /// </summary>
		public bool Connected//查询连接状态
		{
			get
			{
                if (串口 != null)
                {
                    return (串口.IsOpen);
                }

                if (UDP标志 & tcpClient != null)
                    return true;
                if (tcpClient == null)
                    return false;
                else
                {
                    return 连接OK;

                }
			}
		}

        /// <summary> Ping IP 是否能Ping通 </summary>
        /// <param name="timeout"> 等待答复时间MS </param>
        /// <returns> true, false</returns>
        public bool PingIP(int timeout) //检查是否可用
        {
            //ipAddress = "192.168.1.65";
            // Ping IP
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            IPAddress address = System.Net.IPAddress.Parse(ipAddress);
           
            // 创建一个包含32字节数据的缓冲区以进行传输
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);

            // 等待10秒钟，等待答复
            System.Net.NetworkInformation.PingReply reply = pingSender.Send(address, timeout, buffer);

            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                return true;
            else
                return false;
        }

        #region 计算CRC
        /// <summary>
        /// Calculates the CRC16 for Modbus-RTU
        /// </summary>
        /// <param name="data">要计算CRC的字节数组  Byte buffer to send</param>
        /// <param name="numberOfBytes"> 要计算CRC的字节数 Number of bytes to calculate CRC</param>
        /// <param name="startByte"> 从第几个字节开始计算 First byte in buffer to start calculating CRC</param>
        public static UInt16 CRC(byte[] 字节数组, ushort 要计算的字节数, int 开始计算的字节位置)//calculateCRC
        {
            //long START = DateTime.Now.Ticks;
            byte[] auchCRCHi = {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
            0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
            0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
            0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
            0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
            0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
            0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
            0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
            0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
            0x40
            };

            byte[] auchCRCLo = {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
            0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
            0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
            0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
            0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
            0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
            0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
            0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
            0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
            0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
            0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
            0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
            0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
            0x40
            };
            ushort usDataLen = 要计算的字节数;
            byte CRC_Hi = 0xFF;
            byte CRC_Lo = 0xFF;
            int i = 0;
            int uIndex;
            while (usDataLen > 0)
            {
                usDataLen--;
                if ((i + 开始计算的字节位置) < 字节数组.Length)
                {
                    uIndex = CRC_Lo ^ 字节数组[i + 开始计算的字节位置];
                    CRC_Lo = (byte)(CRC_Hi ^ auchCRCHi[uIndex]);
                    CRC_Hi = auchCRCLo[uIndex];
                }
                i++;
            }
            //long END = DateTime.Now.Ticks;
            //Console.WriteLine((END - START));
            return (ushort)((ushort)CRC_Hi << 8 | CRC_Lo);
        }

        public static ushort CRC2(byte[] 字节数组, ushort 要计算的字节数, int 开始计算的字节位置)
        {
            long START = DateTime.Now.Ticks;

            int Len = 要计算的字节数;
            ushort crc = 0xFFFF;
            for (int i = 开始计算的字节位置; i < Len; i++)
            {
                crc = (ushort)(crc ^ 字节数组[i]);

                for (int j = 0; j < 8; j++)
                {
                    crc >>= 1;
                    if ((crc & 0x01)== 0x01)
                        crc ^= 0xA001;
                }
            }
            long END = DateTime.Now.Ticks;
            Console.WriteLine((END - START));
            return crc;
        }



        #endregion


        #region 公共变量
        /// <summary> 获取或设置服务器的IP地址
        /// Gets or Sets the IP-Address of the Server.
        /// </summary>
        public string IPAddress
		{
			get { return ipAddress; }
			set { ipAddress = value; }
		}

        /// <summary> 获取或设置Modbus TCP服务器可访问的端口（标准为502）
        /// Gets or Sets the Port were the Modbus-TCP Server is reachable (Standard is 502).
        /// </summary>
		public int Port
		{
			get { return port; }
			set { port = value; }
		}

        /// <summary> 获取或设置UDP标志以激活Modbus UDP
        /// Gets or Sets the UDP-Flag to activate Modbus UDP.
        /// </summary>
        public bool UDPFlag
        {
            get { return UDP标志; }
            set { UDP标志 = value;}
        }

        /// <summary> 获取或设置串行连接时的单元标识符（默认值为0）
        /// Gets or Sets the Unit identifier in case of serial connection (Default = 0)
        /// </summary>
        public byte UnitIdentifier
        {
            get { return 单元标识符; }
            set
            { 单元标识符 = value; }
        }

        /// <summary> 获取或设置串行连接的波特率（默认值为9600）
        /// Gets or Sets the Baudrate for serial connection (Default = 9600)
        /// </summary>
        public int Baudrate
        {
            get { return 波特率; }
            set { 波特率 = value; }
        }

        /// <summary> 获取或设置串口 奇偶校验
        /// Gets or Sets the of Parity in case of serial connection
        /// </summary>
        public Parity Parity
        {
            get
            {
                if (串口 != null)
                    return 奇偶校验;
                else
                    return Parity.Even;
            }
            set
            {
                if (串口 != null)
                    奇偶校验 = value;
            }
        }

        /// <summary> 获取或设置串口 停止位
        /// Gets or Sets the number of stopbits in case of serial connection
        /// </summary>
        public StopBits StopBits
        {
            get
            {
                if (串口 != null)
                    return 停止位;
                else
                    return StopBits.One;
            }
            set
            {
                if (串口 != null)
                    停止位 = value;
            }
        }

        /// <summary> 获取或设置ModbusTCP连接的超时时间
        /// Gets or Sets the connection Timeout in case of ModbusTCP connection
        /// </summary>
        public int ConnectionTimeout
        {
            get { return 读超时; }
            set { 读超时 = value; }
        }

        /// <summary> 获取或设置串行端口
        /// Gets or Sets the serial Port
        /// </summary>
        public string SerialPort
        {
            get
            {

                return 串口.PortName;
            }
            set
            {
                if (value == null)
                {
                    串口 = null;
                    return;
                }
                if (串口 != null)
                    串口.Close();
                this.串口 = new SerialPort();
                this.串口.PortName = value;
                串口.BaudRate = 波特率;
                串口.Parity = 奇偶校验;
                串口.StopBits = 停止位;
                串口.WriteTimeout = 写超时;
                串口.ReadTimeout = 读超时;
                串口.DataReceived += new SerialDataReceivedEventHandler(串口接收数据事件);
            }
        }

        /// <summary> 获取或设置日志文件的文件名
        /// Gets or Sets the Filename for the LogFile
        /// </summary>
        public string DebugLogFileName//
        {
            get { return StoreLogData.Instance.DebugLogFileName; }
            set 
            {
                StoreLogData.Instance.DebugLogFileName = value;
                if (StoreLogData.Instance.DebugLogFileName != null)
                    debug = true;
                else
                    debug = false;
            }
        }
        public string ErrorLogFileName
        {
            get { return StoreLogData.Instance.DebugLogFileName; }
            set
            {
                StoreLogData.Instance.DebugLogFileName = value;
                if (StoreLogData.Instance.DebugLogFileName != null)
                    debug = true;
                else
                    debug = false;
            }
        }

        #endregion


    }
}
