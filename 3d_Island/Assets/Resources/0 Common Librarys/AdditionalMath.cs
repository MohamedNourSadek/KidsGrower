using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalMath 
{
    //returns angles between two vectors
    public static float CircleAngle(Vector2 origin, Vector2 target)
    {
        Vector2 Direction = (target - origin);

        var Angle = Mathf.Rad2Deg * Mathf.Atan(Direction.y / Direction.x);

        if (Direction.y > 0 && Direction.x < 0 || Direction.y < 0 && Direction.x < 0)
            return Angle + 180;

        if (Direction.y < 0 && Direction.x > 0)
            return Angle + 360;

        else return Angle;
    }


    //returns the angle between two vectors
    public static float Get_Angle(Vector3 to, Vector3 from, bool Return_The_Smaller, bool ChangeToRad)
    {
        float Angle = Vector3.Angle(to, from);

        if (Return_The_Smaller)
        {
            Angle = (Angle <= 90f) ? Angle : (180f - Angle);
        }

        return ChangeToRad ? Angle * Mathf.Deg2Rad : Angle;
    }
    public static bool IsTheSameSign(float x1, float x2)
    {
        if (x1 > 0f && x2 > 0f)
            return true;
        else if (x1 < 0f && x2 < 0f)
            return true;
        else
            return false;
    }

      
    //Used for Remaping different ranges
    public static float RemapRange(float Value, Vector2 InitialRange, Vector2 FinalRange)
    {
        return (FinalRange.y - FinalRange.x) * ((Value - InitialRange.x) / (InitialRange.y - InitialRange.x)); ;
    }


    //returns angle from y axis to 2D vector
    
    public static float AngleFromY(Vector2 _vector)
    {
        float Angle = Mathf.Atan(_vector.x / _vector.y) * Mathf.Rad2Deg;

        bool _isXpositive = _vector.x >= 0;
        bool _isYpositive = _vector.y >= 0;

        if (_isXpositive && _isYpositive)
        {
            Angle += 0;
        }
        else if (_isXpositive && !_isYpositive)
        {
            Angle += 180f;
        }
        else if (!_isXpositive && !_isYpositive)
        {
            Angle += 180f;

        }
        else if (!_isXpositive && _isYpositive)
        {
            Angle += 360f;
        }

        return Angle;
    }
}
