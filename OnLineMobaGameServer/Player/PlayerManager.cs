using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PlayerManager
{
    /// <summary>
    /// 所有玩家字典
    /// </summary>
    public static Dictionary<uint, Player> players = new Dictionary<uint, Player>();

    /// <summary>
    /// 获取玩家
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static Player GetPlayer(uint guid)
    {
        if (players.ContainsKey(guid))
        {
            return players[guid];
        }
        return null;
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="player"></param>
    public static void AddPlayer(uint guid, Player player)
    {
        players.Add(guid, player);
    }

    /// <summary>
    /// 移除玩家
    /// </summary>
    /// <param name="guid"></param>
    public static void RemovePlayer(uint guid)
    {
        players.Remove(guid);
    }
}
