using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 描述客户端的对象
/// </summary>
public class ClientState
{
    /// <summary>
    /// 客户端socket
    /// </summary>
    Socket socket;

    /// <summary>
    /// 客户端的缓冲区
    /// </summary>
    ByteArray readBuffer;
}
