using System.Collections.Generic;

using UnityEngine;

using Dropship.Performance;
using Dropship.Random.Extension;

namespace Dropship.AmongUs;

public static class Task
{
    public static int GetRandomCommonTaskId()
        => GetRandomCommonTaskId(new HashSet<TaskTypes>());

    public static int GetRandomNormalTaskId()
        => GetRandomNormalTaskId(new HashSet<TaskTypes>());

    public static int GetRandomLongTaskId()
        => GetRandomLongTaskId(new HashSet<TaskTypes>());

    public static int GetRandomCommonTaskId(HashSet<TaskTypes> blackList)
    {
        if (CachedShipStatus.IsNull) { return int.MinValue; }

        return GetTaskId(
            CachedShipStatus.Instance.CommonTasks, blackList);
    }

    public static int GetRandomNormalTaskId(HashSet<TaskTypes> blackList)
    {
        if (CachedShipStatus.IsNull) { return int.MinValue; }

        return GetTaskId(
            CachedShipStatus.Instance.NormalTasks, blackList);
    }

    public static int GetRandomLongTaskId(HashSet<TaskTypes> blackList)
    {
        if (CachedShipStatus.IsNull) { return int.MinValue; }

        return GetTaskId(
            CachedShipStatus.Instance.LongTasks, blackList);
    }

    public static bool IsValidConsole(PlayerControl? player, Console? console)
    {
        if (player == null || console == null) { return false; }

        Vector2 playerPos = player.GetTruePosition();
        Vector2 consolePos = console.transform.position;

        bool isCheckWall = console.checkWalls;

        return
            player.CanMove &&
            (!console.onlySameRoom || console.InRoom(playerPos)) &&
            (!console.onlyFromBelow || playerPos.y < consolePos.y) &&
            Vector2.Distance(playerPos, consolePos) <= console.UsableDistance &&
            (
                !isCheckWall ||
                !PhysicsHelpers.AnythingBetween(
                        playerPos, consolePos, Constants.ShadowMask, false)
            );
    }

    public static Minigame OpenMinigame(
        Minigame prefab,
        PlayerTask? task = null,
        Console? console = null)
    {
        Transform cmaraTrans = Camera.main.transform;
        Minigame minigame = Object.Instantiate(
            prefab, cmaraTrans, false);
        minigame.transform.SetParent(cmaraTrans, false);
        minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
        if (console != null)
        {
            minigame.Console = console;
        }
        minigame.Begin(task);

        return minigame;
    }

    private static int GetTaskId(NormalPlayerTask[] task, HashSet<TaskTypes> blackList)
    {
        NormalPlayerTask? result;
        do
        {
            int index = task.GetRandomIndex();
            result = task[index];

        } while (blackList.Contains(result.TaskType));

        if (result == null) { return int.MinValue; }

        return result.Index;
    }
}
