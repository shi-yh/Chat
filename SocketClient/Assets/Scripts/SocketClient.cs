using System;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using Application = UnityEngine.Application;
using Button = UnityEngine.UI.Button;

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
    

    public InputField InputField;
    
    public InputField myName;
    
    public TextCell textCell;

    public ImgCell imgCell;

    public Transform cellparent;

    public ScrollRect scroll;

    public Button connectButton;
    
    public Button _avatar;

    private SocketClientData _controller;

    
    public void OnClickConnect()
    {
        if (string.IsNullOrEmpty(myName.text))
        {
            Debug.LogError("请输入昵称");
            return;
        }
        
        _controller = new SocketClientData(null,myName.text);

        myName.enabled = false;
        
        connectButton.gameObject.SetActive(false);
    }
    
    
    public void OnSendButtonClick()
    {
        if (string.IsNullOrEmpty(InputField.text) )
        {
            return;
        }
        
        _controller.SendMessage(InputField.text);
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
                int length = Fs.Read(Global.buffer, 0, Global.buffer.Length);

                ///TODO 这里要处理一下读到头的问题
                byte[] img = new byte[length];
                
                Buffer.BlockCopy(Global.buffer,0,img,0,length);
                
                _controller.SendMessage(img);
            }
        }
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
        if (_controller!=null)
        {
            AddMessage(_controller.GetMessage());
        }
    }


    private void AddMessage(MessageData md)
    {
        if (md==null)
        {
            return;;
        }
        
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