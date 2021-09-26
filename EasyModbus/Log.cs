using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace EasyModbus
{
    class Log
    {

        //获取私有配置文件字符串
        [System.Runtime.InteropServices.DllImport("kernel32")] private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        //写入私有配置文件字符串
        [System.Runtime.InteropServices.DllImport("kernel32")] private static extern long WritePrivateProfileString(string mpAppName, string mpKeyName, string mpDefault, string mpFileName);

        private Int64 FileSize = 0;//文件大小

        public void Write()
        { 
        

        }

        public void Read()
        {


        }


    }
}
