/*

*/
using System;
using System.Runtime.Serialization;

namespace EasyModbus.Exceptions
{
    /// <summary>
    /// 如果串行端口未打开，将引发异常
    /// </summary>
    public class SerialPortNotOpenedException : ModbusException
	{
   		public SerialPortNotOpenedException() : base() { }

    	public SerialPortNotOpenedException(string message) : base(message) { }

    	public SerialPortNotOpenedException(string message, Exception innerException) : base(message, innerException) { }

    	protected SerialPortNotOpenedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

    /// <summary>
    /// 如果与Modbus设备的连接失败，将引发异常
    /// </summary>
    public class ConnectionException : ModbusException
	{
   		public ConnectionException(): base() { }

    	public ConnectionException(string message): base(message) { }

    	public ConnectionException(string message, Exception innerException) : base(message, innerException) { }

    	protected ConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

    /// <summary>
    /// 如果Modbus服务器返回错误代码“功能代码不受支持”，则引发异常
    /// </summary>
    public class FunctionCodeNotSupportedException : ModbusException
	{
   		public FunctionCodeNotSupportedException() : base() { }

    	public FunctionCodeNotSupportedException(string message) : base(message) { }

    	public FunctionCodeNotSupportedException(string message, Exception innerException) : base(message, innerException) { }

    	protected FunctionCodeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

    /// <summary>
    /// 如果Modbus服务器返回错误代码“数无效”，则引发异常
    /// </summary>
    public class QuantityInvalidException : ModbusException
	{
   		public QuantityInvalidException() : base() { }

    	public QuantityInvalidException(string message) : base(message) { }

    	public QuantityInvalidException(string message, Exception innerException) : base(message, innerException) { }

    	protected QuantityInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

    /// <summary>
    /// 如果Modbus服务器返回错误代码“开始地址和数量无效”，则引发异常
    /// </summary>
    public class StartingAddressInvalidException : ModbusException
	{
   		public StartingAddressInvalidException() : base() { }

    	public StartingAddressInvalidException(string message) : base(message) { }

    	public StartingAddressInvalidException(string message, Exception innerException) : base(message, innerException) { }

    	protected StartingAddressInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

    /// <summary>
    /// 如果Modbus服务器返回错误代码“未执行功能代码（0x04）”将引发异常
    /// Exception to be thrown if Modbus Server returns error code "Function Code not executed (0x04)"
    /// </summary>
    public class ModbusException : Exception
	{
   		public ModbusException() : base() { }

    	public ModbusException(string message) : base(message) { }

    	public ModbusException(string message, Exception innerException) : base(message, innerException) { }

    	protected ModbusException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

    /// <summary>
    /// 如果CRC检查失败，将引发异常
    /// Exception to be thrown if CRC Check failed
    /// </summary>
    public class CRCCheckFailedException : ModbusException
	{
   		public CRCCheckFailedException() : base() { }

    	public CRCCheckFailedException(string message) : base(message) { }

    	public CRCCheckFailedException(string message, Exception innerException) : base(message, innerException) { }

    	protected CRCCheckFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
	
}
