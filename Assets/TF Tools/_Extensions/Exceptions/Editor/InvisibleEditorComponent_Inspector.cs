using UnityEngine;
using UnityEditor;

[CustomEditor ( typeof ( InvisibleEditorComponent ), true )]
public class InvisibleEditorComponent_Inspector : Editor
{
	public override void OnInspectorGUI ()
	{
		if ( PrefabUtility.GetPrefabAssetType ( target ) == PrefabAssetType.Model )
			return;

		if ( target.hideFlags != ( HideFlags.HideInInspector | HideFlags.NotEditable | HideFlags.DontUnloadUnusedAsset ) )
			target.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable | HideFlags.DontUnloadUnusedAsset;
	}
}