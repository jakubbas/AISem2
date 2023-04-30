using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static float Magnitude(Vector2 a)
    {

        float magnitude;
        magnitude = (a.x * a.x + a.y * a.y);
        magnitude = Mathf.Sqrt(magnitude);

        return magnitude;
    }

    public static Vector2 Normalise(Vector2 a)
    {
        float magnitude = Magnitude(a);

        a = new Vector2(a.x / magnitude, a.y / magnitude);

        return a;
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        float dot;
        dot = (lhs.x * rhs.x) + (lhs.y * rhs.y);
        return dot;
    }

    public static float Angle(Vector2 lhs, Vector2 rhs)
    {
        Vector2 unitLhs = Normalise(lhs);
        Vector2 unitRhs = Normalise(rhs);

        float unitDot = Dot(unitLhs, unitRhs);

        float angle = Mathf.Acos(unitDot);

        return angle;
    }

    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        angle = angle * Mathf.Rad2Deg;
        float newX = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
        float newY = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);
        Vector2 rotatedVector = new Vector2(newX, newY);

        return rotatedVector;
    }

    public static Vector2 RotateVectorAroundPoint(Vector2 originVector, Vector2 pointVector, float angle)
    {
        Vector2 localOffset = originVector - pointVector;

        Vector2 rotatedOffSet = RotateVector(localOffset, angle);

        Vector2 newPosition = pointVector + rotatedOffSet;

        return newPosition;
    }

}
