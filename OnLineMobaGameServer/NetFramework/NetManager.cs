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
    /// 用于检测的列表
    /// </summary>
    public static List<Socket> sockets = new List<Socket>();

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
            sockets.Clear();
            //放服务端的Socket
            sockets.Add(listenfd);
            //放客户端的Socket
            foreach (Socket socket in states.Keys)
            {
                sockets.Add(socket);
            }
            Socket.Select(sockets, null, null, 1000);
            for (int i = 0; i < sockets.Count; i++)
            {
                Socket s = sockets[i];
                if (s == listenfd)
                {
                    //有客户端要连接
                    Accept(listenfd);
                }
                else
                {
                    //客户端发消息过来了
                    Receive(s);
                }
            }
        }
    }

    /// <summary>
    /// 接收客户端连接
    /// </summary>
    /// <param name="listenfd">服务端的Socket</param>
    private static void Accept(Socket listenfd)
    {
        try
        {
            Socket socket = listenfd.Accept();
            Console.WriteLine("Accept " + socket.RemoteEndPoint.ToString());
            //创建描述客户端的对象
            ClientState state = new ClientState();
            state.socket = socket;

            states.Add(socket, state);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Accept 失败" + e.Message);
        }
    }

    /// <summary>
    /// 接收客户端发送过来的消息
    /// </summary>
    /// <param name="socket">客户端的Socket</param>
    private static void Receive(Socket socket)
    {
        ClientState state = states[socket];
        ByteArray readBuffer = state.readBuffer;

        if (readBuffer.Remain <= 0)
        {
            readBuffer.MoveBytes();
        }
        if (readBuffer.Remain <= 0)
        {
            Console.WriteLine("Receive 失败， 数组不够大");
            return;
        }

        int count = 0;
        try
        {
            count = socket.Receive(readBuffer.bytes, readBuffer.writeIndex, readBuffer.Remain, SocketFlags.None);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Receive 失败，" + e.Message);
            return;
        }
        //客户端主动关闭
        if (count <= 0)
        {
            Console.WriteLine("Socket Close:" + socket.RemoteEndPoint.ToString());
            return;
        }
        readBuffer.writeIndex += count;
        //处理消息
        OnReveiceData(state);
        readBuffer.MoveBytes();
    }

    private static void OnReveiceData(ClientState state)
    {

    }
}
