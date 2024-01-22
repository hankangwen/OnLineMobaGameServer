using PBMessage;
using System;
using System.Collections.Generic;

/// <summary>
/// 锁帧管理器（帧同步管理器）
/// </summary>
public class LockStepManager
{
    /// <summary>
    /// 上一帧时间
    /// </summary>
    private long _lastTime;

    /// <summary>
    /// 当前时间
    /// </summary>
    private long _currTime;

    /// <summary>
    /// 一帧的时间间隔
    /// </summary>
    private long _timeInterval = 20;

    /// <summary>
    /// 是否开启
    /// </summary>
    public bool isStarted;

    /// <summary>
    /// 帧数
    /// </summary>
    public int turn = 0;

    /// <summary>
    /// 运行的房间
    /// </summary>
    public Room room;

    //todo hankangwen:此处需要优化，帧数可以改为uint
    /// <summary>
    /// 所有操作
    /// key:帧数， value:当前帧的所有操作
    /// </summary>
    public Dictionary<int, List<Opts>> allOpt = new Dictionary<int, List<Opts>>();

    /// <summary>
    /// 运行帧同步
    /// </summary>
    public void Run()
    {
        if (isStarted)
            return;

        isStarted = true;
        _lastTime = (long)(DateTime.Now - unixEpochDateTime).TotalMilliseconds;

        while (true)
        {
            _currTime = (long)(DateTime.Now - unixEpochDateTime).TotalMilliseconds;

            long time = _currTime - _lastTime;
            if(time > _timeInterval)
            {
                _lastTime = _currTime;
                turn++;

                MsgLockStepBack msgLockStepBack = new MsgLockStepBack();
                msgLockStepBack.turn = turn;

                foreach (var player in room.playerUsyncOpt)
                {
                    List<UnsyncOpts> unsyncOpts = new List<UnsyncOpts>();
                    
                    //player.Value是当前玩家同步到的帧数，turn表示服务器的帧数
                    //player.Value到turn之间表示当前客户端没有同步到的帧数
                    if(player.Value < turn)
                    {
                        UnsyncOpts unsyncOpt = new UnsyncOpts();
                        for (int i = player.Value; i < turn; i++)
                        {
                            unsyncOpt.turn = i;
                            if (allOpt.ContainsKey(i))
                            {
                                //unsyncOpt.opts = allOpt[i].ToArray();
                                unsyncOpt.opts.AddRange(allOpt[i].ToArray());
                            }
                        }
                        unsyncOpts.Add(unsyncOpt);
                    }
                    //msgLockStepBack.unsyncOpts = unsyncOpts.ToArray();
                    msgLockStepBack.unsyncOpts.AddRange(unsyncOpts.ToArray());
                    NetManager.SendTo(msgLockStepBack, player.Key);
                }
            }
        }
    }

    #region 获取1970年1月1日的DataTime

    private DateTime _unixEpochDateTime;
    /// <summary>
    /// 获取1970年1月1日的DataTime
    /// </summary>
    private DateTime unixEpochDateTime
    {
        get
        {
            if (_unixEpochDateTime == null)
                _unixEpochDateTime = new DateTime(1970, 1, 1);
            return _unixEpochDateTime;
        }
    }

    #endregion
}
