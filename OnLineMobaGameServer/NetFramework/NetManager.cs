using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

public static class NetManager
{
    /// <summary>
    /// 服务端socket
    /// </summary>
    public static Socket listenfd;

    /// <summary>
    /// 客户端字典
    /// </summary>
    public static Dictionary<Socket, ClientState> states = new Dictionary<Socket, ClientState>();

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ip">ip地址</param>
    /// <param name="port">端口号</param>
    public static void Connect(string ip, int port)
    {
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress iPAddress = IPAddress.Parse(ip);
        IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
        listenfd.Bind(iPEndPoint);
        listenfd.Listen(0);

        Console.WriteLine("服务器启动成功");
        while (true)
        {

        }
    }
}
