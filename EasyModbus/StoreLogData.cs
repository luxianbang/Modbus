
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyModbus
{
    /// <summary>
    /// Store Log-Data in a File
    /// 将日志数据存储在文件中
    /// </summary>
    public sealed class StoreLogData
    {
    	private String debugLogFileName = null;
        private String errorLogFileName = null;
        private static volatile StoreLogData 实例;// instance
        private static object 同步对象 = new Object();// syncObject
       
        /// <summary>
        /// Private constructor; Ensures the access of the class only via "instance"
        /// 私有的；确保仅通过“实例”访问类
        /// </summary>
        private StoreLogData()
        {
        }

        /// <summary>
        /// Returns the instance of the class (singleton)
        /// 返回类的实例（单例）
        /// </summary>
        /// <returns>instance (Singleton)</returns>
        public static StoreLogData Instance 
        {
            get
            {
                if (实例 == null)
                {
                    lock (同步对象)
                    {
                        if (实例 == null)
                            实例 = new StoreLogData();
                    }
                }
                return 实例;
            }
        }

        /// <summary>
        /// Store message in Log-File
        /// 将消息存储在日志文件中
        /// </summary>
        /// <param name="message">Message to append to the Log-File</param>
        public void Store(String message)
        {
            if (this.debugLogFileName == null)
                return;
            using (System.IO.StreamWriter file =  new System.IO.StreamWriter(debugLogFileName, true))
                file.WriteLine(message);
        }

        /// <summary>
        /// Store message in Log-File including Timestamp
        /// 在日志文件中存储消息，包括时间戳
        /// </summary>
        /// <param name="message">Message to append to the Log-File</param>
        /// <param name="timestamp">Timestamp to add to the same Row</param>
        public void Store(String message, DateTime timestamp)
        {
            try
            {
                if (this.debugLogFileName == null)
                    return;

                FileInfo gg = new FileInfo(debugLogFileName);

              Int64  FileSize = gg.Length;// FileSize/1024=KB

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(debugLogFileName, true))
                    file.WriteLine(timestamp.ToString("yyyy.MM.dd H:mm:ss.ff ") + message);
            }
            catch (Exception){}
        }


        /// <summary>
        /// Gets or Sets the Filename to Store Strings in a File
        /// 获取或设置用于在文件中存储字符串的文件名
        /// </summary>
        public string DebugLogFileName
        {
			get { return debugLogFileName; }
			set { debugLogFileName = value; }
		}
    }
}
