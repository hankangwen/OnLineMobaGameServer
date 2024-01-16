using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBMessage;
using ProtoBuf;

public class MsgHandler
{
    //public static void MsgPing(ClientState c, IExtensible msgBase)
    //{
        //Console.WriteLine("MsgPing:" + c.socket.RemoteEndPoint);
        //c.lastPingTime = NetManager.GetTimeStamp();
        //MsgPong msgPong = new MsgPong();
        //NetManager.Send(c, msgPong);
    //}

    public static void MsgTest(uint guid, IExtensible msgBase)
    {
        Console.WriteLine($"guid:{guid}, MsgTest:{msgBase.ToString()}");
        NetManager.Send(msgBase, guid);
    }

    //public static void MsgPing(ClientState c, MsgBase msgBase)
    //{
    //    Console.WriteLine("MsgPing:" + c.socket.RemoteEndPoint);
    //    c.lastPingTime = NetManager.GetTimeStamp();
    //    MsgPong msgPong = new MsgPong();
    //    NetManager.Send(c, msgPong);
    //}
}
