using UnityEngine;

using UnityEditor;

using TFTools.Extensions;

using TFToolsEditor.Extensions;
using TFToolsEditor.CustomGUI;

public class UtilityWindow : MyEditorWindow
{
	SerializedObject m_SObj;

	public static void ShowToolboxWindow <T> () where T : UtilityWindow
	{
		T wnd = EditorWindow.GetWindow ( typeof ( T ), true ) as T;

		wnd.minSize = wnd.windowSize;
		wnd.maxSize = wnd.minSize;

		if ( wnd.windowTitle.tooltip != "" )
			wnd.titleContent = new GUIContent ( wnd.windowTitle.tooltip );
		else
			wnd.titleContent = new GUIContent ( wnd.windowTitle.text );
	}

	void OnGUI ()
	{
		sObj.Update ();
		
		EditorGUI.DrawRect ( fullRect, skin.lightColor );

		GUILayout.Space ( 10f );

		#region Title

		GUILayout.BeginHorizontal ();
		{
			GUILayout.Space ( 25f );

			TFGUILayout.Label ( windowTitle.image as Texture2D,
			                    GUILayout.Width ( 22f ),
			                    GUILayout.Height ( 22f ) );

			TFGUILayout.Label ( windowTitle.text,
			                    TFStyles.label.WithFont ( 15, Color.white ),
			                    GUILayout.Height ( 22f ) );
		}
		GUILayout.EndHorizontal ();

		#endregion

		GUILayout.Space ( 5f );

		DrawSeparator ( 30f );

		#region Background

		Rect subRect = fullRect;
		subRect.x += 15f;
		subRect.width -= 30f;
		subRect.y += 50f;
		subRect.height -= 65f;

		EditorGUI.DrawRect ( subRect, skin.darkColor );
		EditorGUI.DrawRect ( subRect.Reduce ( 1f ), skin.color );

		#endregion

		GUILayout.BeginArea ( subRect );
		{
			GUILayout.BeginHorizontal ();
			{
				GUILayout.Space ( 5f );
				GUILayout.BeginVertical ();
				{
					GUILayout.Space ( 5f );
					OnHeaderGUI ();
					OnBodyGUI ();
					GUILayout.FlexibleSpace ();
					OnFooterGUI ();
					GUILayout.Space ( 10f );
				}
				GUILayout.EndVertical ();
				GUILayout.Space ( 5f );
			}
			GUILayout.EndHorizontal ();
		}
		GUILayout.EndArea ();
		sObj.ApplyModifiedProperties ();

		Repaint ();
	}

	public virtual void OnHeaderGUI ()
	{
	}

	public virtual void OnBodyGUI ()
	{
	}

	public virtual void OnFooterGUI ()
	{
	}

	#region Getter / Setter

	protected SerializedObject sObj
	{
		get
		{
			if ( this.m_SObj == null )
				this.m_SObj = new SerializedObject ( this );

			return  this.m_SObj;
		}
	}

	#region Virtual

	protected virtual GUIContent windowTitle
	{
		get
		{
			return new GUIContent ( "ToolBox Window" );
		}
	}

	protected virtual Vector2 windowSize
	{
		get
		{
			return new Vector2 ( 350f, 500f );
		}
	}

	#endregion

	#endregion
}