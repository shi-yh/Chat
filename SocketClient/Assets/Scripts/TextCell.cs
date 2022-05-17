using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCell : MonoBehaviour
{
    public Text name;

    public Text msg;

    public void RefreshView(MessageData md)
    {
        gameObject.SetActive(true);
        
        name.text = md.name;

        msg.text = md.message;
    }
}
