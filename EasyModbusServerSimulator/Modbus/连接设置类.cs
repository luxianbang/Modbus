using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace EasyModbusServerSimulator
{
    public class 连接设置类
    {
        public enum ModbusType { ModbusTCP, ModbusUDP, ModbusRTU };
        private int port = 8000;
        [DescriptionAttribute("Listenig Port for Modbus-TCP or Modbus-UDP Server")]
        [CategoryAttribute("Modbus属性")]
        public int Port//端口
        {
            get { return port; }
            set { port = value; }
        }

        private ModbusType modbusType;
        [DescriptionAttribute("Activate Modbus UDP; Disable Modbus TCP")]
        [CategoryAttribute("Modbus属性")]
        public ModbusType ModbusTypeSelection
        {
            get
            {
                return modbusType;
            }
            set
            {
                modbusType = value;
            }
        }

        private string comPort;
        [DescriptionAttribute("ComPort Used for Modbus RTU connection ")]
        [CategoryAttribute("Modbus RTU 属性")]
        public string ComPort
        {
            get
            {
                return comPort;
            }
            set
            {
                comPort = value;
            }
        }

        private byte slaveAddress;
        [DescriptionAttribute("UnitIdentifier (Slave address) for Modbus RTU connection")]
        [CategoryAttribute("Modbus RTU 属性")]
        public byte SlaveAddress
        {
            get
            {
                return slaveAddress;
            }
            set
            {
                slaveAddress = value;
            }
        }

    }
}
