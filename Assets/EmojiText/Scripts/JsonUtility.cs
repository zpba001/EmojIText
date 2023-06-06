using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;

// Json工具
public static class JsonUtility
{
    // 在json中获取json字段
    public static JsonData GetJsonInJson(this JsonData jsonData, string key)
    {
        JsonData vJson = null;
        try
        {
            vJson = jsonData[key];
        }
        catch (Exception)
        {
        }
        return vJson;
    }
    // 在json中获取字符串字段
    public static string GetStrInJson(this JsonData jsonData, string key, string defV = "")
    {
        string v = defV;
        try
        {
            v = jsonData[key].ToString();
        }
        catch (Exception)
        {
        }
        return v;
    }
    // 在json中获取整数字段
    public static int GetIntInJson(this JsonData jsonData, string key, int defV = 0)
    {
        int v = defV;
        try
        {
            v = int.Parse(jsonData[key].ToString());
        }
        catch (Exception)
        {
        }
        return v;
    }
    // 在json中获取浮点字段
    public static float GetFloatInJson(this JsonData jsonData, string key, float defV = 0f)
    {
        float v = defV;
        try
        {
            v = float.Parse(jsonData[key].ToString());
        }
        catch (Exception)
        {
        }
        return v;
    }

    // 在json中获取json字段
    public static JsonData GetJsonInArr(this JsonData jsonData, int idx)
    {
        JsonData vJson = null;
        try
        {
            vJson = jsonData[idx];
        }
        catch (Exception)
        {
        }
        return vJson;
    }
    // 在json中获取字符串字段
    public static string GetStrInArr(this JsonData jsonData, int idx, string defV = "")
    {
        string v = defV;
        try
        {
            v = jsonData[idx].ToString();
        }
        catch (Exception)
        {
        }
        return v;
    }
    // 在json中获取整数字段
    public static int GetIntInArr(this JsonData jsonData, int idx, int defV = 0)
    {
        int v = defV;
        try
        {
            v = int.Parse(jsonData[idx].ToString());
        }
        catch (Exception)
        {
        }
        return v;
    }
    // 在json中获取浮点字段
    public static float GetFloatInArr(this JsonData jsonData, int idx, float defV = 0f)
    {
        float v = defV;
        try
        {
            v = float.Parse(jsonData[idx].ToString());
        }
        catch (Exception)
        {
        }
        return v;
    }

    // 获取字符串列表
    public static void GetStrListInJson(this JsonData jsonData, string key, List<string> list)
    {
        JsonData arrJson = GetJsonInJson(jsonData, key);
        if (arrJson != null && arrJson.Count > 0)
        {
            for (int i = 0; i < arrJson.Count; i++)
            {
                string temp = GetStrInArr(arrJson, i, "");
                if (temp.Length > 0)
                {
                    list.Add(temp);
                }
            }
        }
    }
    // 获取整数列表
    public static void GetStrListInArr(this JsonData jsonData, int idx, List<string> list)
    {
        JsonData arrJson = GetJsonInArr(jsonData, idx);
        if (arrJson != null && arrJson.Count > 0)
        {
            for (int i = 0; i < arrJson.Count; i++)
            {
                list.Add(arrJson.GetStrInArr(i));
            }
        }
    }
    // 获取整数列表
    public static void GetIntListInJson(this JsonData jsonData, string key, List<int> list)
    {
        JsonData arrJson = GetJsonInJson(jsonData, key);
        if (arrJson != null && arrJson.Count > 0)
        {
            for (int i = 0; i < arrJson.Count; i++)
            {
                list.Add(arrJson.GetIntInArr(i));
            }
        }
    }
    // 获取整数列表
    public static void GetIntListInArr(this JsonData jsonData, int idx, List<int> list)
    {
        JsonData arrJson = GetJsonInArr(jsonData, idx);
        if (arrJson != null && arrJson.Count > 0)
        {
            for (int i = 0; i < arrJson.Count; i++)
            {
                list.Add(arrJson.GetIntInArr(i));
            }
        }
    }

    // 获取long类型
    public static long GetLongInJson(this JsonData jsonData, string key, long defV = 0)
    {
        long v = defV;
        try
        {
            v = long.Parse(jsonData[key].ToString());
        }
        catch (Exception)
        {
        }
        return v;
    }
    // 获取long类型
    public static long GetLongInArr(this JsonData jsonData, int idx, long defV = 0)
    {
        long v = defV;
        try
        {
            v = long.Parse(jsonData[idx].ToString());
        }
        catch (Exception)
        {
        }
        return v;
    }

    // 获取坐标类型
    public static Vector2 GetVector2InJson(this JsonData jsonData, string key, Vector2 defV = default(Vector2))
    {
        JsonData posJson = jsonData.GetJsonInJson(key);
        Vector2 v = defV;
        if (posJson != null)
        {
            float x = posJson.GetFloatInArr(0);
            float y = posJson.GetFloatInArr(1);
            v.x = x;
            v.y = y;
        }
        return v;
    }
    // 获取坐标类型
    public static Vector2 GetVector2InArr(this JsonData jsonData, int idx, Vector2 defV = default(Vector2))
    {
        JsonData posJson = jsonData.GetJsonInArr(idx);
        Vector2 v = defV;
        if (posJson != null)
        {
            float x = posJson.GetFloatInArr(0);
            float y = posJson.GetFloatInArr(1);
            v.x = x;
            v.y = y;
        }
        return v;
    }

    // json对象转字符串
    public static string JsonToStr(JsonData json)
    {
        return Regex.Unescape(json.ToString());
    }
}
