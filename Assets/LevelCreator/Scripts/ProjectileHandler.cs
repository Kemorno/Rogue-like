using System;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileHandler
{
    public static Vector2 GetDirection(Vector2 ThrowerPos, Vector2 TargetPos, float Deviation = 1f)
    {
        Vector2 Direction = Vector2.zero;

        Direction = TargetPos - ThrowerPos;

        return GetDeviation(Direction, Deviation);
    }

    public static Vector2 GetVelocityMultiplier(Vector2 ThrowerPos, Vector2 TargetPos)
    {
        Vector2 Direction = Vector2.zero;

        Direction = TargetPos - ThrowerPos;

        return Direction.normalized;
    }

    public static Vector2 GetForce(Vector2 ThrowerPos, Vector2 TargetPos, float Deviation = 1f)
    {
        Vector2 Direction = Vector2.zero;

        Direction = TargetPos - ThrowerPos;

        return GetDeviation(Direction, Deviation).normalized;
    }

    public static Vector2 GetForce(Vector2 ThrowerPos, Vector2 TargetPos, float forceCap, float Deviation = 1f)
    {
        Vector2 Direction = Vector2.zero;

        Direction = TargetPos - ThrowerPos;

        if (Direction.x > forceCap)
            Direction.x = forceCap;
        if (Direction.y > forceCap)
            Direction.y = forceCap;

        return GetDeviation(Direction, Deviation);
    }

    private static Vector2 GetDeviation(Vector2 V2, float Deviation)
    {
        Vector2 Direction = V2;

        Direction.x += UnityEngine.Random.Range(-Deviation / 2f, Deviation / 2f);
        Direction.y += UnityEngine.Random.Range(-Deviation / 2f, Deviation / 2f);

        return Direction;
    }
}

