using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SocketClient : MonoBehaviour
{
    public string ipaddress = "192.168.65.11";

    public int port = 7788;

    private Socket clientSocket;

    private Thread _thread;

    private byte[] _data = new byte[1024];

    public InputField InputField;
    
    public InputField myName;

    public string message = "";

    
    

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipaddress), port));

        _thread = new Thread(ReceiveMessage);

        _thread.Start();
    }

    private void ReceiveMessage()
    {
        while (true)
        {
            if (clientSocket.Connected == false)
            {
                break;
            }

            int length = clientSocket.Receive(_data);

            if (length>0)
            {
                if (_data[0]==0)
                {
                    message = Encoding.UTF8.GetString(_data, 1, length);
                }
            }
        }
    }

    /// <summary>
    /// 一个中文三个字节
    /// </summary>
    /// <param name="name"></param>
    /// <param name="message"></param>
    void SendMessage(string name, string message)
    {
        byte[] arrMsg = Encoding.UTF8.GetBytes(message);

        byte[] nameMsg = Encoding.UTF8.GetBytes(name);
        ///名字不大于
        byte[] send = new byte[arrMsg.Length + 1 +22];

        send[0] = 0;

        Buffer.BlockCopy(arrMsg, 0, send, 1, arrMsg.Length);
        
        clientSocket.Send(send);
    }

    public void OnSendButtonClick()
    {
        string value = myName.text + ":" + InputField.text;
        SendMessage("",value);
        InputField.text = "";
    }

    /// <summary>
    /// 选择文件
    /// </summary>
    public void SelectFileBtnClick()
    {
        OpenFileDialog od = new OpenFileDialog();
        od.Title = "请选择头像图片";
        od.Multiselect = false;
        od.Filter = "图片文件(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";


        if (od.ShowDialog() == DialogResult.OK)
        {
        }
    }


    private void OnDestroy()
    {
        _thread?.Abort();
        ///既不接受也不发送
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }


    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(message))
        {
            // chat.text += "\n" + message;
            message = "";
        }
    }
}