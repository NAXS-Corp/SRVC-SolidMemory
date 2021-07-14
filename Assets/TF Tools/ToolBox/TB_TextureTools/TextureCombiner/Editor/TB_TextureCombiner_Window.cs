using UnityEngine;

using UnityEditor;

using TFTools.Extensions;

using TFToolsEditor;
using TFToolsEditor.Extensions;
using TFToolsEditor.CustomGUI;

namespace TFTools.ToolBox.TextureTools.TextureCombiner
{
	public class TB_TextureCombiner_Window : EditorWindow
	{
		Texture2D m_RChannelTexture;
		Texture2D m_GChannelTexture;
		Texture2D m_BChannelTexture;
		Texture2D m_AChannelTexture;

		TextureChannelMode m_RChanelMode = TextureChannelMode.r;
		TextureChannelMode m_GChanelMode = TextureChannelMode.g;
		TextureChannelMode m_BChanelMode = TextureChannelMode.b;
		TextureChannelMode m_AChanelMode = TextureChannelMode.a;

		Color [] m_ChannelsColors = new Color[] {
			Color.red,
			Color.green,
			Color.blue,
			Color.white,
			Color.grey
		};

		float m_TextureFieldSize = 85f;
		Vector2 m_PreviewSize = new Vector2 ( 64, 64 );
		Vector2 m_TextureSize = new Vector2 ( 1024, 1024 );

		Texture2D m_PreviewTexture;

		bool m_LinkedTextures = true;

		[ MenuItem ( "TF Tools/Tool Box/Texture Combiner" )]
		public static void ShowWindow ()
		{
			EditorWindow wnd = EditorWindow.GetWindow ( typeof ( TB_TextureCombiner_Window ), true, "Texture Combiner" );
			wnd.minSize = new Vector2 ( 370f, 558f );
			wnd.maxSize = wnd.minSize;
		}

		void OnEnable ()
		{
			m_PreviewTexture = GetTexture ( m_PreviewSize );
		}

		void OnGUI ()
		{
			EditorGUI.DrawRect ( new Rect ( 0, 0, position.width, position.height ), skin.lightColor );

			GUILayout.Space ( 10f );

			#region Title

			GUILayout.BeginHorizontal ();
			GUILayout.Space ( 25f );

			EditorGUILayout.LabelField ( new GUIContent ( TFToolsResources.typesIcons [ "Texture" ] ),
			                             GUILayout.Width ( 22f ),
			                             GUILayout.Height ( 22f ) );

			EditorGUILayout.LabelField ( new GUIContent ( "Texture Channels Combiner" ),
			                             skin.label.WithFont ( 15, Color.white ),
			                             GUILayout.Height ( 22f ) );
			
			GUILayout.EndHorizontal ();

			#endregion

			DrawSeparator ( 10f );

			Rect subRect = EditorGUILayout.GetControlRect ( GUILayout.Height ( 490f ) );
			subRect.x += 15f;
			subRect.width -= 30f;
			EditorGUI.DrawRect ( subRect, skin.darkColor );
			EditorGUI.DrawRect ( subRect.Reduce ( 1f ), skin.color );
			GUILayout.Space ( -485f );

			#region Fields
			
			GUILayout.BeginHorizontal ();
			{
				GUILayout.Space ( 25f );

				ChannelField ( "R",
				               TFToolsResources.typesIcons [ "TextureR" ],
				               ref m_RChannelTexture,
				               ref m_RChanelMode,
				               m_LinkedTextures );

				GUILayout.FlexibleSpace ();

				GUILayout.Space ( -3f );

				GUILayout.BeginVertical ();
				{
					GUILayout.Space ( 55f );

					DrawTextureLinkField ();
				}
				GUILayout.EndVertical ();

				GUILayout.Space ( -3f );

				GUILayout.FlexibleSpace ();

				ChannelField ( "G",
				               TFToolsResources.typesIcons [ "TextureG" ],
				               ref  m_GChannelTexture,
				               ref m_GChanelMode,
				               m_LinkedTextures );

				GUILayout.FlexibleSpace ();

				GUILayout.Space ( -3f );

				GUILayout.BeginVertical ();
				{
					GUILayout.Space ( 55f );

					DrawTextureLinkField ();
				}
				GUILayout.EndVertical ();

				GUILayout.Space ( -3f );

				GUILayout.FlexibleSpace ();

				ChannelField ( "B",
				               TFToolsResources.typesIcons [ "TextureB" ],
				               ref m_BChannelTexture,
				               ref m_BChanelMode,
				               m_LinkedTextures );

				GUILayout.Space ( 25f );
			}
			GUILayout.EndHorizontal ();

			GUILayout.Space ( 5f );

			GUILayout.BeginHorizontal ();
			{
				GUILayout.Space ( 25f );

				ChannelField ( "Alpha",
				               TFToolsResources.typesIcons [ "TextureA" ],
				               ref m_AChannelTexture,
				               ref m_AChanelMode );

				GUILayout.Space ( 20f );

				#region Texture Size

				GUILayout.BeginVertical ();
				{
					GUILayout.Space ( 20f );

					Rect textureSizeRect = EditorGUILayout.GetControlRect ( GUILayout.Height ( m_TextureFieldSize + 20f ),
					                                                        GUILayout.Width ( 198f ) );

					EditorGUI.DrawRect ( textureSizeRect, skin.darkColor );
					EditorGUI.DrawRect ( textureSizeRect.Reduce ( 1f ), skin.lightColor );
					GUILayout.Space ( -m_TextureFieldSize - 20f );
					EditorGUILayout.LabelField ( "    Texture Size",
					                             skin.label.WithFont ( 13, Color.white ),
					                             GUILayout.Height ( 18f ) );
					
					GUILayout.Space ( 5f );

					GUILayout.BeginHorizontal ();
					{
						GUILayout.Space ( 30f );
						m_TextureSize.x = EditorGUILayout.IntField ( ( int ) m_TextureSize.x,
						                                             EditorStyles.textField.WithAlignment ( TextAnchor.MiddleCenter ),
						                                             GUILayout.Height ( 20f ) );
						EditorGUILayout.LabelField ( "x", EditorStyles.label.WithFont ( Color.white ), GUILayout.Width ( 10f ) );
						m_TextureSize.y = EditorGUILayout.IntField ( ( int ) m_TextureSize.y,
						                                             EditorStyles.textField.WithAlignment ( TextAnchor.MiddleCenter ),
						                                             GUILayout.Height ( 20f ) );
						GUILayout.Space ( 16f );
					}
					GUILayout.EndHorizontal ();

					GUILayout.Space ( 5f );

					GUILayoutOption [] options = new GUILayoutOption[] { GUILayout.Width ( 50f ), GUILayout.Height ( 18f ) };

					GUILayout.BeginHorizontal ();
					{
						GUILayout.Space ( 30f );

						if ( GUILayout.Button ( "256", skin.button, options ) )
							m_TextureSize = new Vector2 ( 256, 256 );
						if ( GUILayout.Button ( "512", skin.button, options ) )
							m_TextureSize = new Vector2 ( 512, 512 );
						if ( GUILayout.Button ( "1024", skin.button, options ) )
							m_TextureSize = new Vector2 ( 1024, 1024 );

						GUILayout.FlexibleSpace ();
					}
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					{
						GUILayout.Space ( 30f );

						if ( GUILayout.Button ( "2048", skin.button, options ) )
							m_TextureSize = new Vector2 ( 2048, 2048 );
						if ( GUILayout.Button ( "4096", skin.button, options ) )
							m_TextureSize = new Vector2 ( 4096, 4096 );
						if ( GUILayout.Button ( "8192", skin.button, options ) )
							m_TextureSize = new Vector2 ( 8192, 8192 );

						GUILayout.FlexibleSpace ();
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndVertical ();

				#endregion

				GUILayout.Space ( 25f );
			}
			GUILayout.EndHorizontal ();

			#endregion

			GUILayout.Space ( 3f );

			DrawSeparator ( 10f );

			#region Preview

			EditorGUILayout.LabelField ( "Preview",
			                             skin.label.WithAlignment ( TextAnchor.MiddleCenter ).WithFont ( Color.white ) );

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			Rect previewRect = EditorGUILayout.GetControlRect ( GUILayout.Width ( 155f ), GUILayout.Height ( 155f ) );
			EditorGUI.DrawRect ( previewRect, skin.darkColor );
			EditorGUI.DrawRect ( previewRect.Reduce ( 1f ), skin.lightColor );
			EditorGUI.DrawTextureTransparent ( previewRect.Reduce ( 5f ), m_PreviewTexture );
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			#endregion

			GUILayout.Space ( 4f );

			GUILayout.BeginHorizontal ();
			GUILayout.Space ( 25f );
			if ( GUILayout.Button ( "Export", skin.button, GUILayout.Height ( 22f ) ) )
				ExportTexture ();
			GUILayout.Space ( 20f );
			GUILayout.EndHorizontal ();
		}

		void DrawTextureLinkField ()
		{
			EditorGUI.BeginChangeCheck ();
			m_LinkedTextures = GUILayout.Toggle ( m_LinkedTextures,
			                                      "",
			                                      skin.button,
			                                      GUILayout.Width ( 17f ),
			                                      GUILayout.Height ( 17f ) );
			if ( EditorGUI.EndChangeCheck () )
			{
				if ( m_LinkedTextures )
				{
					if ( m_RChannelTexture != null )
						m_GChannelTexture = m_BChannelTexture = m_RChannelTexture;
					else if ( m_GChannelTexture != null )
						m_RChannelTexture = m_BChannelTexture = m_GChannelTexture;
					else if ( m_BChannelTexture != null )
						m_RChannelTexture = m_GChannelTexture = m_BChannelTexture;
				}
			}

			EditorGUI.LabelField ( GUILayoutUtility.GetLastRect ().Reduce ( -1f ), new GUIContent ( m_LinkedTextures ? TFToolsResources.utilityIcons [ "LinkEnabled" ] : TFToolsResources.utilityIcons [ "LinkDisabled" ] ),
			                       EditorStyles.label.WithAlignment ( TextAnchor.MiddleCenter ) );
		}

		void DrawSeparator ( float offSet = 0f )
		{
			Handles.color = skin.darkColor;

			Rect lastRect = GUILayoutUtility.GetLastRect ();
			Vector3 p1 = new Vector3 ( 30f, lastRect.y + lastRect.height + offSet, 0f );
			Vector3 p2 = new Vector3 ( position.width - 30f, lastRect.y + lastRect.height + offSet, 0f );
			Handles.DrawLine ( p1, p2 );
			GUILayout.Space ( offSet + 5f );
		}

		void ChannelField ( string label,
		                    Texture2D icon,
		                    ref Texture2D tex,
		                    ref TextureChannelMode interpretationMode,
		                    bool setRGBOnModif = false )
		{
			EditorGUI.BeginChangeCheck ();

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();

			GUILayout.FlexibleSpace ();

			EditorGUILayout.LabelField ( new GUIContent ( icon ),
			                             GUILayout.Width ( 18f ),
			                             GUILayout.Height ( 18f ) );
			
			EditorGUILayout.LabelField ( label,
			                             skin.label.WithFont ( 12, Color.white ),
			                             GUILayout.Width ( label == "Alpha" ? 40f : 15f ),
			                             GUILayout.Height ( 22f ) );
			GUILayout.FlexibleSpace ();

			GUILayout.EndHorizontal ();

			GUILayout.Space ( -4f );

			Rect objectRect = EditorGUILayout.GetControlRect ( GUILayout.Width ( m_TextureFieldSize ),
			                                                   GUILayout.Height ( m_TextureFieldSize + 20f ) );

			EditorGUI.DrawRect ( objectRect, skin.darkColor );
			EditorGUI.DrawRect ( objectRect.Reduce ( 1f ), skin.lightColor );
			tex = EditorGUI.ObjectField ( objectRect.Reduce ( 5f ),
			                              "",
			                              tex,
			                              typeof ( Texture2D ),
			                              false ) as Texture2D;

			GUILayout.Space ( -27f );

			GUILayout.BeginHorizontal ();

			GUILayout.Space ( 10f );

			GUILayout.BeginVertical ();
			GUILayout.Space ( 4f );

			interpretationMode = ( TextureChannelMode ) EditorGUILayout.EnumPopup ( interpretationMode,
			                                                                        GUILayout.Width ( 50f ) );

			Rect popupRect = GUILayoutUtility.GetLastRect ();
			popupRect.x += popupRect.width - 25;
			popupRect.width = 10;
			popupRect.y += 2;
			popupRect.height -= 5;
			EditorGUI.DrawRect ( popupRect, Color.black );
			EditorGUI.DrawRect ( popupRect.Reduce ( 1f ), m_ChannelsColors [ ( int ) interpretationMode ] );

			GUILayout.EndVertical ();

			if ( GUILayout.Button ( "",
			                        skin.button,
			                        GUILayout.Width ( 15f ),
			                        GUILayout.Height ( 15f ) ) )
			{
				tex = null;
			}

			EditorGUI.LabelField ( GUILayoutUtility.GetLastRect ().Reduce ( -3f ),
			                       new GUIContent ( TFToolsResources.utilityIcons [ "CrossEnabled" ] ),
			                       EditorStyles.label.WithAlignment ( TextAnchor.MiddleCenter ).WithFont ( 8, Color.white ) );

			GUILayout.Space ( 6f );

			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();

			if ( EditorGUI.EndChangeCheck () )
			{
				if ( setRGBOnModif )
					m_GChannelTexture = m_BChannelTexture = m_RChannelTexture = tex;
				
				m_PreviewTexture = GetTexture ( m_PreviewSize );
			}
		}

		Texture2D GetTexture ( Vector2 _size )
		{
			Texture2D tex = MergeChannels ( _size,
			                                m_RChannelTexture,
			                                m_GChannelTexture,
			                                m_BChannelTexture,
			                                m_AChannelTexture );

			return tex;
		}

		Texture2D MergeChannels ( Vector2 _size,
		                          Texture2D _rChannel,
		                          Texture2D _gChannel,
		                          Texture2D _bChannel,
		                          Texture2D _aChannel )
		{
			Texture2D tex = new Texture2D ( ( int ) _size.x, ( int ) _size.y );

			Vector2 rSize = _rChannel.GetSize ();
			Vector2 gSize = _gChannel.GetSize ();
			Vector2 bSize = _bChannel.GetSize ();
			Vector2 aSize = _aChannel.GetSize ();

			for ( int y = 0; y < _size.y; y++ )
				for ( int x = 0; x < _size.x; x++ )
				{
					Color col = new Color ();

					Color r = GetPixelIfPossible ( _rChannel,
					                               ( ( float ) x / _size.x ) * rSize.x,
					                               ( ( float ) y / _size.y ) * rSize.y );
					Color g = GetPixelIfPossible ( _gChannel,
					                               ( ( float ) x / _size.x ) * gSize.x,
					                               ( ( float ) y / _size.y ) * gSize.y );
					Color b = GetPixelIfPossible ( _bChannel,
					                               ( ( float ) x / _size.x ) * bSize.x,
					                               ( ( float ) y / _size.y ) * bSize.y );
					Color a = GetPixelIfPossible ( _aChannel,
					                               ( ( float ) x / _size.x ) * aSize.x,
					                               ( ( float ) y / _size.y ) * aSize.y );
					
					col.r = GetChanelValue ( r, m_RChanelMode );
					col.g = GetChanelValue ( g, m_GChanelMode );
					col.b = GetChanelValue ( b, m_BChanelMode );
					col.a = GetChanelValue ( a, m_AChanelMode );

					tex.SetPixel ( x, y, col );
				}

			tex.Apply ();

			return tex;
		}

		public static Color GetPixelIfPossible ( Texture2D tex, float x, float y )
		{
			return GetPixelIfPossible ( tex, ( int ) x, ( int ) y, Color.white );
		}

		public static Color GetPixelIfPossible ( Texture2D tex, int x, int y )
		{
			return GetPixelIfPossible ( tex, x, y, Color.white );
		}

		public static Color GetPixelIfPossible ( Texture2D tex, int x, int y, Color defaultColor )
		{
			if ( tex == null )
				return defaultColor;

			return tex.GetPixel ( x, y );
		}

		public static float GetChanelValue ( Color col, TextureChannelMode interpretationMode )
		{
			switch ( interpretationMode )
			{
				case (TextureChannelMode.r):
					return col.r;
				case (TextureChannelMode.g):
					return col.g;
				case (TextureChannelMode.b):
					return col.b;
				case (TextureChannelMode.a):
					return col.a;
				case (TextureChannelMode.GrayScale):
					return ( col.r + col.g + col.b ) / 3f;
			}

			return 0f;
		}

		void ExportTexture ()
		{
			string savePath = EditorUtility.SaveFilePanel ( "Select Export Folder", "Assets", "", "png" );

			if ( savePath == "" || savePath == " " )
				return;

			Texture2D tex = GetTexture ( m_TextureSize );

			System.IO.File.WriteAllBytes ( savePath, tex.EncodeToPNG () );
			AssetDatabase.Refresh ();
		}

		#region Getter / Setter

		GUISkinPlus skin
		{
			get
			{
				return TFGUI.guiSkinPro;
			}
		}

		#endregion
	}
}