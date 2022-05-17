using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImgCell : MonoBehaviour
{
    public Text name;

    public Image img;
    
    public void RefershView(MessageData md)
    {
        gameObject.SetActive(true);
        
         name.text = md.name;

        using (FileStream fs = File.Create(Application.streamingAssetsPath + "/" + SocketClient.PicIdx+".jpg"))
        {
            fs.Write(md.img,0,md.img.Length);
        }

        Texture2D tex = new Texture2D(10,10);

        tex.LoadImage(md.img);
        
        Sprite sp = Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),Vector2.zero);
        
        img.sprite = sp;

        float width = GetComponent<RectTransform>().rect.width*0.7f;
        
        if (sp.rect.size.x<width)
        {
            img.SetNativeSize();
        }
        else
        {
            float rate =  width/sp.rect.size.x ;

            Vector2 size = new Vector2(width, sp.rect.size.y * rate);

            img.rectTransform.sizeDelta = size;
        }

        ///偷懒了偷懒了
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width, img.rectTransform.sizeDelta.y + 150);



    }
    
}
