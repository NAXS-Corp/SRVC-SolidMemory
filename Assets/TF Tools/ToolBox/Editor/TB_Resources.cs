using UnityEngine;
using UnityEditor;

namespace TFTools.ToolBox
{
	public static partial class TB
	{
		static string m_Path = "ToolBox/";

		/// <summary>
		/// Load an asset at the specified custom path.
		/// </summary>
		public static T Load <T> ( string pathEnd ) where T : Object
		{
			return TFToolsEditor.TFToolsLocation.Load <T> ( m_Path + pathEnd );
		}
	}
}