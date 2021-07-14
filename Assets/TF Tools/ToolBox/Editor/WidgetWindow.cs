using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WidgetWindow : EditorWindow
{
    protected List<Widget> m_Widgets = new List<Widget> ();

    public static T ShowWidgetWindow <T> () where T : WidgetWindow
    {
        T wnd = EditorWindow.GetWindow ( typeof ( T ), false ) as T;
        
        if ( wnd.windowTitle.tooltip != "" )
            wnd.titleContent = new GUIContent ( wnd.windowTitle.tooltip );
        else
            wnd.titleContent = new GUIContent ( wnd.windowTitle.text );

        return wnd;
    }

    #region GUI

    void OnGUI()
    {
        ToolBar ();

        WidgetArea ();

        BottomToolBar ();
    }

    void ToolBar()
    {
        OnToolBarGUI ();
    }

    protected virtual void OnToolBarGUI()
    {

    }

    void WidgetArea()
    {
        OnWidgetAreaGUI ();
    }

    protected virtual void OnWidgetAreaGUI()
    {

    }

    void BottomToolBar()
    {
        OnBottomToolBarGUI ();
    }

    protected virtual void OnBottomToolBarGUI()
    {

    }

    #endregion

    protected virtual void OnClickWidget ()
    {

    }

    #region Getter / Setter

    #region Virtual

    protected virtual GUIContent windowTitle
    {
        get
        {
            return new GUIContent ( "Widget Window" );
        }
    }

    #endregion

    #endregion
}