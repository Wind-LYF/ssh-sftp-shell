﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace 文件获取FTP工具
{
    interface SFTP
    {
        public class SFTPHelper
        {
            //[DllImport("Tamir.SharpSSH.dll")]
            //private extern static IntPtr LoadLibrary(String path);
            //[DllImport("Tamir.SharpSSH.dll")]
            //private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
            //[DllImport("Tamir.SharpSSH.dll")]
            //private extern static bool FreeLibrary(IntPtr lib);
            //[DllImport("DiffieHellman.dll")]
            //private extern static IntPtr LoadLibrary(String path);
            //[DllImport("DiffieHellman.dll")]
            //private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
            //[DllImport("DiffieHellman.dll")]
            //private extern static bool FreeLibrary(IntPtr lib);
            //[DllImport("Org.Mentalis.Security.dll")]
            //private extern static IntPtr LoadLibrary(String path);
            //[DllImport("Org.Mentalis.Security.dll")]
            //private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
            //[DllImport("Org.Mentalis.Security.dll")]
            private extern static bool FreeLibrary(IntPtr lib);
            private Session m_session;
            private Channel m_channel;
            private ChannelSftp m_sftp;

            //host:sftp地址   user：用户名   pwd：密码        
            public SFTPHelper(string host, string user, string pwd)
            {
                string[] arr = host.Split(':');
                string ip = arr[0];
                int port = 22;
                if (arr.Length > 1) port = Int32.Parse(arr[1]);

                JSch jsch = new JSch();
                m_session = jsch.getSession(user, ip, port);
                MyUserInfo ui = new MyUserInfo();
                ui.setPassword(pwd);
                m_session.setUserInfo(ui);

            }

            //SFTP连接状态        
            public bool Connected { get { return m_session.isConnected(); } }

            //连接SFTP        
            public bool Connect()
            {
                try
                {
                    if (!Connected)
                    {
                        m_session.connect();
                        m_channel = m_session.openChannel("sftp");
                        m_channel.connect();
                        m_sftp = (ChannelSftp)m_channel;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //断开SFTP        
            public void Disconnect()
            {
                if (Connected)
                {
                    m_channel.disconnect();
                    m_session.disconnect();
                }
            }

            //SFTP存放文件        
            public bool Put(string localPath, string remotePath)
            {
                try
                {
                    Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(localPath);
                    Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(remotePath);
                    m_sftp.put(src, dst);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //SFTP获取文件        
            public bool Get(string remotePath, string localPath)
            {
                try
                {
                    Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(remotePath);
                    Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(localPath);
                    m_sftp.get(src, dst);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            //删除SFTP文件
            public bool Delete(string remoteFile)
            {
                try
                {
                    m_sftp.rm(remoteFile);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //获取SFTP文件列表        
            public ArrayList GetFileList(string remotePath, string fileType)
            {
                try
                {
                    Tamir.SharpSsh.java.util.Vector vvv = m_sftp.ls(remotePath);
                    ArrayList objList = new ArrayList();
                    foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry qqq in vvv)
                    {
                        string sss = qqq.getFilename();
                        if (sss.Length > (fileType.Length + 1) && fileType == sss.Substring(sss.Length - fileType.Length))
                        { objList.Add(sss); }
                        else { continue; }
                    }

                    return objList;
                }
                catch
                {
                    return null;
                }
            }


            //登录验证信息        
            public class MyUserInfo : UserInfo
            {
                String passwd;
                public String getPassword() { return passwd; }
                public void setPassword(String passwd) { this.passwd = passwd; }

                public String getPassphrase() { return null; }
                public bool promptPassphrase(String message) { return true; }

                public bool promptPassword(String message) { return true; }
                public bool promptYesNo(String message) { return true; }
                public void showMessage(String message) { }
            }
        }
    }
}
