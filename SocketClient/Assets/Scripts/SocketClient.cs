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

    private Queue<MessageData>  _messages = new Queue<MessageData>();
    

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
                MessageData md = ProtobufHelper.DeSerialize<MessageData>(Encoding.UTF8.GetString(_data,0,length));
                _messages.Enqueue(md);
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

        MessageData md = new MessageData();

        md.name = name;

        md.message = message;

        byte[] send = Encoding.UTF8.GetBytes(ProtobufHelper.Serialize(md));
        
        clientSocket.Send(send);
    }

    public void OnSendButtonClick()
    {
        SendMessage(myName.text,InputField.text);
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
        while (_messages.Count>0)
        {
            AddMessage(_messages.Dequeue());
        }
    }

    private void AddMessage(MessageData md)
    {
        Debug.Log(md.ToString());
    }
    
    
}