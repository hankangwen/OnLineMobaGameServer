﻿using System;
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
        Console.WriteLine($"[{DateTime.Now.TimeOfDay}]收到网关转发的消息，来自客户端guid:{guid}, MsgTest:{msgBase.ToString()}");
        NetManager.SendTo(msgBase, guid);
    }

    //public static void MsgPing(ClientState c, MsgBase msgBase)
    //{
    //    Console.WriteLine("MsgPing:" + c.socket.RemoteEndPoint);
    //    c.lastPingTime = NetManager.GetTimeStamp();
    //    MsgPong msgPong = new MsgPong();
    //    NetManager.Send(c, msgPong);
    //}
}
