using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EasyModbusServerSimulator
{
    
    public partial class 属性设置窗口 : Form
    {
        public delegate void settingsChangedEvent();
        public event settingsChangedEvent SettingsChangedEvent;

        连接设置类 settings = new 连接设置类();
        连接设置类 settingsFromMainForm = new 连接设置类();
        public  属性设置窗口(连接设置类 settings)
        {
            this.settingsFromMainForm.Port = settings.Port;
            this.settingsFromMainForm.ModbusTypeSelection = settings.ModbusTypeSelection;
            this.settings = settings;
            InitializeComponent();
            propertyGrid1.SelectedObject = settings;
        }

        private void btnDischard_Click(object sender, EventArgs e)
        {
            settings.Port = settingsFromMainForm.Port;
            settings.ModbusTypeSelection = settingsFromMainForm.ModbusTypeSelection;
            if (SettingsChangedEvent != null)
                SettingsChangedEvent();
            this.Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
           
            if (SettingsChangedEvent != null)
                SettingsChangedEvent();
            this.Close();
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }
    }
}
