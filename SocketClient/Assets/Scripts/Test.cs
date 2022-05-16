using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class Message
{

    [ProtoMember(2)]
    public string msg;
    

}


public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string str = PlayerPrefs.GetString("Test");
        
        Message ms = ProtobufHelper.DeSerialize<Message>(str);

        Debug.Log(ms);


    }
}
