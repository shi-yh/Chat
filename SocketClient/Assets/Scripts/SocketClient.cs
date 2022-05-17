using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Application = UnityEngine.Application;

public class SocketClient : MonoBehaviour
{
    private static int _picIdx;

    public static int PicIdx
    {
        get
        {
            _picIdx++;
            return _picIdx;
        }
    }
    
    
    public string ipaddress = "192.168.65.11";

    public int port = 7788;

    private Socket clientSocket;

    private Thread _thread;

    private byte[] _data = new byte[1024*1024*20];

    public InputField InputField;
    
    public InputField myName;

    private Queue<MessageData>  _messages = new Queue<MessageData>();

    public TextCell textCell;

    public ImgCell imgCell;

    public Transform cellparent;

    public ScrollRect scroll;
    

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
                byte[] resultData = new byte[length];
                
                Buffer.BlockCopy(_data,0,resultData,0,length);
                
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
    void SendMessage(string message)
    {

        MessageData md = new MessageData();

        md.name = myName.text;

        md.message = message;

        byte[] send = ProtobufHelper.Serialize2Bytes(md);
        
        clientSocket.Send(send);
    }

    void SendMessage(byte[] img)
    {
        MessageData md = new MessageData();

        md.name = myName.text;

        md.img = img;
        
        byte[] send = ProtobufHelper.Serialize2Bytes(md);
        
        clientSocket.Send(send);
    }
    

    public void OnSendButtonClick()
    {
        SendMessage(InputField.text);
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
            using (FileStream Fs = new FileStream(od.FileName, FileMode.Open))
            {
                int length = Fs.Read(_data, 0, _data.Length);

                ///TODO 这里要处理一下读到头的问题
                byte[] img = new byte[length];
                
                Buffer.BlockCopy(_data,0,img,0,length);
                
                SendMessage(img);
            }
        }
    }


    private void OnDestroy()
    {
        _thread?.Abort();
        ///既不接受也不发送
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
    
    private void OnApplicationQuit()
    {
        DeleteTempRes();
    }
    
    private void DeleteTempRes()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath);
    
        FileInfo[] files = dir.GetFiles();
    
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(Application.streamingAssetsPath+"/"+files[i].Name);
        }
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
        if (!string.IsNullOrEmpty(md.message))
        {
            TextCell tc = Instantiate(textCell,cellparent);
            
            tc.RefreshView(md);
        }

        if (md.img!=null&&md.img.Length>0)
        {
            ImgCell ic = Instantiate(imgCell, cellparent);
            
            ic.RefershView(md);
        }


        scroll.verticalNormalizedPosition = 0;


    }
    
    
}