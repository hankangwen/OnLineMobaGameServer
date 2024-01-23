using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class RoomManager
{
    /// <summary>
    /// 最大房间号
    /// </summary>
    private static int maxId = 0;

    /// <summary>
    /// 房间列表
    /// </summary>
    public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

    /// <summary>
    /// 获取房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Room GetRoom(int id)
    {
        if(rooms.TryGetValue(id, out var room))
            return room;
        return null;
    }

    /// <summary>
    /// 添加房间
    /// </summary>
    /// <returns></returns>
    public static Room AddRoom()
    {
        maxId++;
        Room room = new Room();
        room.id = maxId;
        room.lockStepManager.room = room;
        rooms.Add(room.id, room);
        return room;
    }
}
