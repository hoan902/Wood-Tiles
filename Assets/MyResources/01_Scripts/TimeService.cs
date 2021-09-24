using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Control time service
/// </summary>
public static class TimeService
{
    public const float MIN_SEC = 60;
    public const float HOUR_SEC = 3600;
    public const float DAY_SEC = 86400;
    public const float FRAME_SEC = 0.02f;
    
    public static long GetCurrentTimeStamp()
    {
        long unixTime = 0;
        unixTime = GetLocalTimeStamp();
        return unixTime;
    }

    public static long GetLocalTimeStamp()
    {
        long unixTime = 0;
        DateTime foo = DateTime.UtcNow;
        unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
        return unixTime;

    }

    public static long GetTimeStampBeginDay()
    {
        long unixTime = 0;
        DateTime today = DateTime.Now.Date;
      
        unixTime = ((DateTimeOffset)today).ToUnixTimeSeconds();
        return unixTime;
    }

    public static long ParseDate(string Date)
    {
        long timeStamp = 0;
        string inString = Date;
        DateTime dateValue;
        if (DateTime.TryParse(inString, out dateValue))
            Debug.Log($"Converted '{inString}' to {dateValue}.");
        else
        {
            Debug.LogError($"Unable to convert '{inString}' to a date.");
            return -1;
        }

        timeStamp = ((DateTimeOffset)dateValue).ToUnixTimeSeconds();

        return timeStamp;

    }

    public static string FormatTimeSpan(double deltaSecs)
    {
        string result = "";
        var ts = TimeSpan.FromSeconds(deltaSecs);

        if (ts.Days > 0)
            result = $"{ts.Days}d : {ts.Hours}h";
        else if (ts.Hours > 0)
            result = $"{ts.Hours}h : {ts.Minutes}m";
        else
            result = $"{ts.Minutes}m : {ts.Seconds}s";

        return result;
    }


    #region Utils
    public static DateTime UnixTimestampToDateTime(double unixTime)
    {
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
        return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
    }

    #endregion
}
