using UnityEngine;
using UnityEditor;

namespace TFTools.ToolBox
{
	public static partial class TB
	{
		public static partial class TextureTools
		{
			static string m_Path = "TB_TextureTools/";

			/// <summary>
			/// Load an asset at the specified custom path.
			/// </summary>
			public static T Load <T> ( string pathEnd ) where T : Object
			{
				return TB.Load <T> ( m_Path + pathEnd );
			}
		}
	}
}