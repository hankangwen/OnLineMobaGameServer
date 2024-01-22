using System;
using ProtoBuf;

public class Player
{
    /// <summary>
    /// 玩家GUID
    /// </summary>
    public uint guid;

    /// <summary>
    /// 所在房间号
    /// </summary>
    public int roomId = -1;

    public Player(uint guid)
    {
        this.guid = guid;
    }

    /// <summary>
    /// TCP发送
    /// </summary>
    /// <param name="msg"></param>
    public void Send(IExtensible msg)
    {
        Console.WriteLine($"msg:{msg.ToString()}, guid:{guid}");
        NetManager.Send(msg, guid);
    }

    /// <summary>
    /// UDP发送
    /// </summary>
    /// <param name="msg"></param>
    public void SendTo(IExtensible msg)
    {
        NetManager.SendTo(msg, guid);
    }
}
