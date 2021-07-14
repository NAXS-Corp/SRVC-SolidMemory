using System.Runtime.InteropServices;
/// <summary>
/// Class with a JS Plugin functions for WebGL.
/// </summary>
public static class WebGLPluginJS
{
   #if UNITY_WEBGL && !UNITY_EDITOR
   // Importing "CallFunction"
   [DllImport("__Internal")]
   public static extern void CallFunction();
   // Importing "PassTextParam"
   [DllImport("__Internal")]
   public static extern void PassTextParam(string text);
   // Importing "PassNumberParam"
   [DllImport("__Internal")]
   public static extern void PassNumberParam(int number);
   // Importing "GetTextValue"
   [DllImport("__Internal")]
   public static extern string GetTextValue();
   // Importing "GetNumberValue"
   [DllImport("__Internal")]
   public static extern int GetNumberValue();
   #endif
}