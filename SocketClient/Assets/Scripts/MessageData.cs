using ProtoBuf;

[ProtoContract]
public class MessageData
{
   [ProtoMember(1)]
   public string name;

   [ProtoMember(2)]
   public string message;

   [ProtoMember(3)]
   public string img;

   public override string ToString()
   {
      return name +":"+ message;
   }
}
