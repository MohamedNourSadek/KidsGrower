using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestSuit
{
    public static int resolution = 500;
    public static int margin = 100;
    public static int border = 10;
    public static Vector2 defaultRange = new Vector2(0,250);
    public static int defaultIncrements = 500;
    public static string defaultpath = "C:\\pic.png";

    public static void ComputeTime_Graph_AndSave(Action _function)
    {
        ComputeTime_Graph_AndSave(_function, defaultRange, defaultIncrements, defaultpath);
    }
    public static void ComputeTime_Graph_AndSave(Action _function, Vector2 _testRange, int _numberOfIncrements, string path)
    {
        Dictionary<int, float> _tests = new Dictionary<int, float>();
        int _increment = (int)((_testRange.y - _testRange.x) / _numberOfIncrements);
        for (int i = 0; i <= _numberOfIncrements; i++)
        {
            int repeats = _increment * i;
            float time = ComputeTime(_function, repeats);

            _tests.Add(repeats, time);
        }

        Texture2D texture = CreateGraph(_tests);

        SaveToPNG(texture, path);
    }

    public static void ComputeTime_StringFormated(Action _function)
    {
        Debug.Log(GetTimeFormated(ComputeTime(_function, (int)defaultRange.y)));
    }
    public static void ComputeTime_StringFormated(Action _function, int repeats)
    {
        Debug.Log(GetTimeFormated(ComputeTime(_function, repeats)));
    }
    public static float ComputeTime(Action _function, int repeats)
    {
        float starttime = 0f;

        starttime = Time.realtimeSinceStartup;

        for (int i = 0; i <= repeats; i++)
        {
            _function();
        }

        float endTime = Time.realtimeSinceStartup;

        float delta = endTime - starttime;

        return delta;
    }
    
    
    
    static Texture2D CreateGraph(Dictionary<int, float> _tests)
    {
        Texture2D snap = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        string GraphData = "";



        for (int x = border; x < resolution - border; x++)
        {
            for (int y = border; y < resolution - border; y++)
            {
                snap.SetPixel(x, y, Color.black);
            }
        }


        float _maxX = 0f;
        float _maxY = 0f;
        foreach (var t in _tests)
        {
            if (t.Key >= _maxX)
                _maxX = t.Key;

            if (t.Value >= _maxY)
                _maxY = t.Value;
        }


        foreach (var t in _tests)
        {
            int value_x = ((int)((t.Key / _maxX) * (resolution - 2 * margin))) + margin;
            int value_y = ((int)((t.Value / _maxY) * (resolution - 2 * margin))) + margin;

            snap.SetPixel(value_x, value_y, Color.white);

            GraphData += "repeats is " + t.Key + " and took " + GetTimeFormated(t.Value) + "\n";
        }

        Debug.Log(GraphData);

        return snap;

    }
    static void SaveToPNG(Texture2D snap, string path)
    {
        string iter = System.DateTime.UtcNow.ToLongTimeString().Replace(":", "_");
        byte[] bytes;
        bytes = snap.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
    }
    static public string GetTimeFormated(float time)
    {
        if (time <= 0)
        {
            return "0 s";
        }
        else if (time >= 1)
        {
            return time + " s";
        }
        else if (time >= 1f / 1000f)
        {
            return (time * 1000) + " ms";
        }
        else if (time >= 1f / 1000000f)
        {
            return (time * 1000000) + " us";
        }
        else
        {
            return (time * 1000000000) + " ns";
        }
    }
}
