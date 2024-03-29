using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using ProtoBuf;
using PBMessage;
using System.Net;

public static partial class NetManager
{
    public enum ServerType
    {
        Gateway,//网关服务器
        Fighter,//战斗服务器
    }

    /// <summary>
    /// 客户端套接字
    /// </summary>
    private static Socket _socket;

    /// <summary>
    /// 存放消息的字节数组
    /// </summary>
    private static ByteArray _byteArray;

    /// <summary>
    /// 消息列表
    /// </summary>
    private static List<IExtensible> _msgList;

    /// <summary>
    /// 是否正在连接
    /// </summary>
    private static bool _isConnecting;

    /// <summary>
    /// 是否正在关闭
    /// </summary>
    private static bool _isClosing;

    /// <summary>
    /// 发送队列
    /// </summary>
    private static Queue<ByteArray> _writeQueue;

    /// <summary>
    /// 一帧处理的最大消息量
    /// </summary>
    private static readonly int MaxProcessMsgCount = 10;

    /// <summary>
    /// 是否启用心跳机制
    /// </summary>
    private static bool _isUsePing = true;

    /// <summary>
    /// 上一次发送Ping的时间
    /// </summary>
    private static float _lastPingTime = 0;

    /// <summary>
    /// 上一次收到Point的时间
    /// </summary>
    private static float _lastPongTime = 0;

    /// <summary>
    /// 心跳机制的时间间隔
    /// </summary>
    private static float _pingInterval = 2;

    private static UdpClient _udpClient;

    /// <summary>
    /// 初始化
    /// </summary>
    private static void Init()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _byteArray = new ByteArray();
        _msgList = new List<IExtensible>();
        _writeQueue = new Queue<ByteArray>();
        _isConnecting = false;
        _isClosing = false;
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip">ip地址</param>
    /// <param name="port">端口号</param>
    public static void Connect(string ip, int port)
    {
        if (_socket != null && _socket.Connected)
        {
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]连接失败，已经连接过了");
            return;
        }
        if (_isConnecting)
        {
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]连接失败，正在连接中");
            return;
        }
        Init();
        _isConnecting = true;
        _socket?.BeginConnect(ip, port, ConnectCallback, _socket);
    }

    /// <summary>
    /// 连接回调
    /// </summary>
    /// <param name="ar"></param>
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]Connect Success!");

            _udpClient = new UdpClient((IPEndPoint)socket.LocalEndPoint);
            _udpClient.Connect((IPEndPoint)socket.RemoteEndPoint);
            _udpClient.BeginReceive(ReceiveUdpCallback, null);

            _isConnecting = false;

            //接收消息
            socket.BeginReceive(_byteArray.bytes, _byteArray.writeIndex, _byteArray.Remain, SocketFlags.None,
                ReceiveCallback, socket);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]连接失败" + e.Message);
            _isConnecting = false;
        }
    }

    /// <summary>
    /// 接收回调
    /// </summary>
    /// <param name="ar"></param>
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //接收的数据量
            int count = socket.EndReceive(ar);
            //断开连接
            if (count == 0)
            {
                Close();
                return;
            }
            //接收数据
            _byteArray.writeIndex += count;

            //处理消息
            OnReceiveData();
            //如果长度过小，扩容
            if (_byteArray.Remain < 8)
            {
                _byteArray.MoveBytes();
                _byteArray.Resize(_byteArray.Length * 2);
            }

            _socket.BeginReceive(_byteArray.bytes, _byteArray.writeIndex, _byteArray.Remain, SocketFlags.None,
                ReceiveCallback, socket);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]接收失败" + e.Message);
        }
    }

    /// <summary>
    /// 关闭客户端
    /// </summary>
    private static void Close()
    {
        if (_socket == null || !_socket.Connected)
            return;
        if (_isConnecting)
            return;

        //消息还没有发送完
        if (_writeQueue.Count > 0)
        {
            _isClosing = true;
        }
        else
        {
            _socket.Close();
        }
    }

    /// <summary>
    /// 处理接收过来的消息
    /// </summary>
    private static void OnReceiveData()
    {
        if (_byteArray.Length <= 2) 
            return;

        byte[] bytes = _byteArray.bytes;
        int readIndex = _byteArray.readIndex;
        //解析消息总体长度
        short length = (short)(bytes[readIndex + 1] * 256 + bytes[readIndex]);

        if (_byteArray.Length < length + 2) 
            return;
        uint guid = (uint)(bytes[readIndex + 2] << 24 |
                    bytes[readIndex + 3] << 16 |
                    bytes[readIndex + 4] << 8 |
                    bytes[readIndex + 5]);
        _byteArray.readIndex += 6;
        int nameCount = 0;
        string protoName = ProtobufTool.DecodeName(_byteArray.bytes, _byteArray.readIndex, out nameCount);
        if (protoName == "")
        {
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]协议名解析失败");
            return;
        }
        _byteArray.readIndex += nameCount;

        //解析协议体
        int bodyLength = length - nameCount - 4;
        IExtensible msgBase = ProtobufTool.Decode(protoName, _byteArray.bytes, _byteArray.readIndex, bodyLength);
        _byteArray.readIndex += bodyLength;

        //移动数据
        _byteArray.MoveBytes();

        MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
        if (mi != null)
        {
            object[] o = { guid, msgBase };
            mi.Invoke(null, o);
        }
        else
        {
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]OnReceiveData fail");
        }

        if (_byteArray.Length > 2)
        {
            OnReceiveData();
        }
    }

    /// <summary>
    /// 发送协议
    /// </summary>
    /// <param name="msg"></param>
    public static void Send(IExtensible msg, uint guid)
    {
        if (_socket == null || !_socket.Connected)
            return;
        if (_isConnecting)
            return;
        if (_isClosing)
            return;

        //编码
        byte[] nameBytes = ProtobufTool.EncodeName(msg);
        byte[] bodyBytes = ProtobufTool.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length + 4;
        byte[] sendBytes = new byte[len + 2];
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        sendBytes[2] = (byte)(guid >> 24);
        sendBytes[3] = (byte)((guid >> 16) & 0xff);
        sendBytes[4] = (byte)((guid >> 8) & 0xff);
        sendBytes[5] = (byte)(guid & 0xff);
        Array.Copy(nameBytes, 0, sendBytes, 6, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 6 + nameBytes.Length, bodyBytes.Length);

        Console.WriteLine($"[{DateTime.Now.TimeOfDay}]发送消息到网关，让网关转发给{guid}，msg:{msg.ToString()}");
        _socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, SendCallback, _socket);
    }

    /// <summary>
    /// 发送回调
    /// </summary>
    /// <param name="ar"></param>
    private static void SendCallback(IAsyncResult ar)
    {
        Socket socket = (Socket)ar.AsyncState;
        if (socket == null || !socket.Connected)
            return;

        socket.EndSend(ar);
    }

    #region udp

    private static void ReceiveUdpCallback(IAsyncResult ar)
    {
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] receiveBuf = _udpClient.EndReceive(ar, ref iPEndPoint);

        uint guid = (uint)(receiveBuf[0] << 24 |
                    receiveBuf[1] << 16 |
                    receiveBuf[2] << 8 |
                    receiveBuf[3]);

        int nameCount = 0;
        string protoName = ProtobufTool.DecodeName(receiveBuf, 4, out nameCount);
        if(protoName == "")
        {
            _udpClient.BeginReceive(ReceiveUdpCallback, null);
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]解析失败");
            return;
        }

        int bodyCount = receiveBuf.Length - nameCount - 4;
        var msgBase = ProtobufTool.Decode(protoName, receiveBuf, 4 + nameCount, bodyCount);
        if(msgBase == null)
        {
            _udpClient.BeginReceive(ReceiveUdpCallback, null);
            return;
        }

        MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
        if(mi != null)
        {
            object[] o = { guid, msgBase };
            mi.Invoke(null, o);
        }
        else
        {
            Console.WriteLine($"[{DateTime.Now.TimeOfDay}]调用函数失败");
        }
        _udpClient.BeginReceive(ReceiveUdpCallback, null);
    }

    /// <summary>
    /// udp发送
    /// </summary>
    /// <param name="msg">消息</param>
    /// <param name="guid">客户端的guid</param>
    public static void SendTo(IExtensible msg, uint guid)
    {
        //编码
        byte[] nameBytes = ProtobufTool.EncodeName(msg);
        byte[] bodyBytes = ProtobufTool.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length + 4;
        byte[] sendBytes = new byte[len];
        //打包guid
        sendBytes[0] = (byte)(guid >> 24);
        sendBytes[1] = (byte)((guid >> 16) & 0xff);
        sendBytes[2] = (byte)((guid >> 8) & 0xff);
        sendBytes[3] = (byte)(guid & 0xff);
        //拷贝到发送数组当中
        Array.Copy(nameBytes, 0, sendBytes, 4, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 4 + nameBytes.Length, bodyBytes.Length);

        _udpClient.Send(sendBytes, sendBytes.Length);
    }

    #endregion
}
