using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalMath 
{
    //returns angles between two vectors
    public static float CircleAngle(Vector2 _origin, Vector2 _target)
    {
        Vector2 _direction = (_target - _origin);

        var _angle = Mathf.Rad2Deg * Mathf.Atan(_direction.y / _direction.x);

        if (_direction.y > 0 && _direction.x < 0 || _direction.y < 0 && _direction.x < 0)
            return _angle + 180;

        if (_direction.y < 0 && _direction.x > 0)
            return _angle + 360;

        else return _angle;
    }


    //returns the angle between two vectors
    public static float Get_Angle(Vector3 _to, Vector3 _from, bool _return_The_Smaller, bool _changeToRad)
    {
        float _angle = Vector3.Angle(_to, _from);

        if (_return_The_Smaller)
        {
            _angle = (_angle <= 90f) ? _angle : (180f - _angle);
        }

        return _changeToRad ? _angle * Mathf.Deg2Rad : _angle;
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
    public static float RemapRange(float _value, Vector2 _initialRange, Vector2 _finalRange)
    {
        return (_finalRange.y - _finalRange.x) * ((_value - _initialRange.x) / (_initialRange.y - _initialRange.x)); ;
    }


    //returns angle _from y axis _to 2D vector
    public static float AngleFromY(Vector2 _vector)
    {
        float _angle = Mathf.Atan(_vector.x / _vector.y) * Mathf.Rad2Deg;

        bool _isXpositive = _vector.x >= 0;
        bool _isYpositive = _vector.y >= 0;

        if (_isXpositive && _isYpositive)
        {
            _angle += 0;
        }
        else if (_isXpositive && !_isYpositive)
        {
            _angle += 180f;
        }
        else if (!_isXpositive && !_isYpositive)
        {
            _angle += 180f;

        }
        else if (!_isXpositive && _isYpositive)
        {
            _angle += 360f;
        }

        return _angle;
    }



}
