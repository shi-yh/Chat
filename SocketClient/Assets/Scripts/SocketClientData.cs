using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SocketClientData 
{
    private const string ipaddress = "192.168.65.11";

    private const int port = 7788;

    private Socket clientSocket;
    
    private Thread _thread;
    
    private Queue<MessageData>  _messages = new Queue<MessageData>();

    private MessageData _myMessageData;

    public MessageData GetMessage()
    {
        if (_messages.Count<=0)
        {
            return null;
        }

        return _messages.Dequeue();
    } 
    
    
    public SocketClientData(byte[] avator,string name)
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipaddress), port));

        _thread = new Thread(ReceiveMessage);

        _thread.Start();

        _myMessageData = new MessageData {avatar = avator, name = name};

    }

    private void ReceiveMessage()
    {
        while (true)
        {
            if (clientSocket.Connected == false)
            {
                break;
            }

            int length = clientSocket.Receive(Global.buffer);
            
            if (length>0)
            {
                byte[] resultData = new byte[length];
                
                Buffer.BlockCopy(Global.buffer,0,resultData,0,length);
                
                MessageData md = ProtobufHelper.DeSerialize<MessageData>(resultData);
                _messages.Enqueue(md);
            }
        }
    }
    
    /// <summary>
    /// 一个中文三个字节
    /// </summary>
    /// <param name="name"></param>
    /// <param name="message"></param>
    public void SendMessage(string message)
    {
        _myMessageData.message = message;

        byte[] send = ProtobufHelper.Serialize2Bytes(_myMessageData);
        
        clientSocket.Send(send);

        _myMessageData.message = "";

    }

    public void SendMessage(byte[] img)
    {
        _myMessageData.img = img;
        
        byte[] send = ProtobufHelper.Serialize2Bytes(_myMessageData);
        
        clientSocket.Send(send);

        _myMessageData.img = null;
    }

    public void SendMessage()
    {
        byte[] send = ProtobufHelper.Serialize2Bytes(_myMessageData);
        
        clientSocket.Send(send);
    }

    ~SocketClientData()
    {
        _thread?.Abort();
        ///既不接受也不发送
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
    
    
    
    
    
    
    
}
