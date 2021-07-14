 using UnityEngine;
 using System.Collections;
 using System;
 
 public static class NXHelper_Time  {
 
     public static int Current()
     {
         DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
         int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
 
         return currentEpochTime;
     }
 
     public static int SecondsElapsed(int t1)
     {
         int difference = Current() - t1;
 
         return Mathf.Abs(difference);
     }
 
     public static int SecondsElapsed(int t1, int t2)
     {
         int difference = t1 - t2;
 
         return Mathf.Abs(difference);
     }
 }
