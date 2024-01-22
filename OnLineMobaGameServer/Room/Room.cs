using System;
using System.Threading;
using System.Collections.Generic;
using ProtoBuf;

public class Room
{
    /// <summary>
    /// 房间号
    /// </summary>
    public int id = 0;

    /// <summary>
    /// 房间最大人数
    /// </summary>
    public int maxPlayer = 2;

    /// <summary>
    /// 玩家列表
    /// </summary>
    public List<uint> playerIds = new List<uint>();

    /// <summary>
    /// 帧同步的对象
    /// </summary>
    public LockStepManager lockStepManager = new LockStepManager();

    /// <summary>
    /// 记录玩家同步到的帧数
    /// key:PlayerId, value:帧数
    /// </summary>
    public Dictionary<uint, int> playerUsyncOpt = new Dictionary<uint, int>();

    /// <summary>
    /// 开启帧同步
    /// </summary>
    public void StartLockStep()
    {
        //新开一条线程来做帧同步
        Thread thread = new Thread(lockStepManager.Run);
        thread.Start();
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="id">玩家的guid</param>
    /// <returns>是否成功</returns>
    public bool AddPlayer(uint id)
    {
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("AddPlayer失败，玩家为空");
            return false;
        }
        if (playerIds.Count >= maxPlayer)
        {
            Console.WriteLine("AddPlayer失败，房间满了");
            return false;
        }
        if (playerIds.Contains(id))
        {
            Console.WriteLine("AddPlayer失败，玩家已经在房间");
            return false;
        }

        playerIds.Add(id);
        player.roomId = this.id;
        playerUsyncOpt.Add(id, 0);
        return true;
    }

    /// <summary>
    /// Tcp广播
    /// </summary>
    /// <param name="msg"></param>
    public void TcpBroadcast(IExtensible msg)
    {
        for (int i = 0; i < playerIds.Count; i++)
        {
            PlayerManager.GetPlayer(playerIds[i]).Send(msg);
        }
    }

    /// <summary>
    /// Udp广播
    /// </summary>
    /// <param name="msg"></param>
    public void UdpBroadcast(IExtensible msg)
    {
        for (int i = 0; i < playerIds.Count; i++)
        {
            PlayerManager.GetPlayer(playerIds[i]).SendTo(msg);
        }
    }
}
