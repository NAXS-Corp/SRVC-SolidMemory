using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
// using System;
using System.Xml;
using System.IO;
using System.Text;
 
 
/// <summary>
/// The script modifies the materials present in the given prefab or object instance. The object is given
/// as the input and as soon as it is placed in the object field, the script processes the
/// materials present and creates a dictionary storing the materials and the new material as key
/// and value pairs respectively. In the initial case the old and new materials will be the same and
/// the they will be displayed in the corresponding labels and object fields. We can edit the entry
/// in object field for "New Material" and put a new material in its place.
/// Once we have changed the required materials then we can do the following three functions.
/// a) Save - By clicking this button we go the saveMaterialMap method which saves the material changes 
/// that we have made into an XML file which can then be used later on if we have to make the changes again at a later time
///
/// b) Load - Using this method we can specify the path of a previously saved XML file and use it to update
/// the transitions for another object. The method used is the "loadMaterialMap" method which reads data 
/// from the XML file and correspondingly updates the object fields with the values from the XML file.
///
/// c) Apply Changes - By clicking this button we call the "applyChanges" method which updates the materials
/// of the object with the new materials we have specified.
/// </summary>
public class MaterialReplacer : EditorWindow
{
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Material comparer calls the material name comparer.
	/// </summary>
	/// -----------------------------------------------------------------------
	private class MaterialComp : Comparer<Material>
	{
		public override int Compare( Material x, Material y )
		{
			return x.name.CompareTo( y.name );
		}
	}

	Material GOListTarget;
	List<GameObject> GOList = new List<GameObject>();
 
	/// <summary>
	/// Stores the material transition for all materials.
	/// </summary>
	SortedDictionary<Material, Material> mMaterialMap = new SortedDictionary<Material, Material>( new MaterialComp() );
	// Dictionary<Material, int> mGOCount = new Dictionary<Material, int>();
	// Dictionary<Material, List<GameObject>> mGOMap = new Dictionary<Material, List<GameObject>>();
	List<Renderer> MissingMatRenderer = new List<Renderer>();
	Material NewMatForMissing;
 
	/// <summary>
	/// The currently selected object.
	/// </summary>
	GameObject mObject;

	
	bool IncludeRenderer = true;
	bool IncludeSkinnedMesh = false;
 
	/// <summary>
	/// Material list scroll position.
	/// </summary>
	Vector2 mScrollPosition = new Vector2( 0, 0 );
	Vector2 mGOScrollPosition = new Vector2( 0, 0 );
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Init this instance.
	/// </summary>
	// ------------------------------------------------------------------------
	[MenuItem ("Tools/Replace Materials")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		MaterialReplacer materialWindow = (MaterialReplacer)EditorWindow.GetWindow( typeof(MaterialReplacer) );
		materialWindow.position = new Rect( 200, 200, 750, 430 );
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// OnGUI renders the editor window.
	/// </summary>
	// ------------------------------------------------------------------------
	void OnGUI()
	{
		//create an object field for the prefab/object
		GUILayout.BeginHorizontal();
		GameObject obj = (GameObject)EditorGUILayout.ObjectField( "Select or drop object:", mObject, typeof(GameObject), true );

		// if something changed
		if( obj != mObject )
		{
			// ignore model prefabs or
			if( PrefabUtility.GetPrefabType( obj ) == PrefabType.ModelPrefab )
			{
				Debug.LogError( "ProcessMaterials, Object cannot be a model file. Please create a prefab first or select an instance from scene" );
				mObject = null;
			}
			else
			{
				mObject = obj;
			}
 
			resetMaterialMap();
		}
		GUILayout.EndHorizontal();

		
		// GUILayout.BeginHorizontal();
		// IncludeRenderer = (bool)EditorGUILayout.Toggle("Renderer", IncludeRenderer);
		// IncludeSkinnedMesh = (bool)EditorGUILayout.Toggle("SkinnedMesh", IncludeSkinnedMesh);
		// GUILayout.EndHorizontal();
 
		GUILayout.Space( 7 );
 
		// Material map //
		if( mMaterialMap.Count > 0 )
		{
			// Create the Headings for displaying the material map
			GUILayout.BeginHorizontal();
			GUILayout.Label( "Current Material", GUILayout.MaxWidth( 250 ) );
			// GUILayout.Label( "Current Material", GUILayout.MaxWidth( 250 ) );
			GUILayout.Label( "Replacement" );
			GUILayout.EndHorizontal();
 
			//Create scroll view for the materials
			mScrollPosition = GUILayout.BeginScrollView( mScrollPosition );
			GUILayout.BeginVertical();
 
			// remember user change and apply it after drawing
			KeyValuePair<Material, Material> transition = new KeyValuePair<Material, Material>( null, null );
 
			foreach( KeyValuePair<Material, Material> pair in mMaterialMap )
			{
				Material mMat = pair.Key;
				GUILayout.BeginHorizontal();
				GUILayout.Label( pair.Key.name, GUILayout.MaxWidth( 250 ) );
				
				// if(GUILayout.Button( "Select", GUILayout.MaxWidth( 50 ))){
					
				if(GUILayout.Button( EditorGUIUtility.IconContent("d_Button Icon"), GUILayout.MaxWidth( 20 ), GUILayout.Height( 20 ))){
					// EditorGUIUtility.PingObject(pair.Key);
					Selection.activeObject = pair.Key;
				}
				// if(GUILayout.Button( EditorGUIUtility.IconContent("GameObject Icon", mGOCount[mMat].ToString()), GUILayout.MaxWidth( 20 ), GUILayout.Height( 20 ))){
				if(GUILayout.Button( EditorGUIUtility.IconContent("d_icon dropdown@2x"), GUILayout.MaxWidth( 20 ), GUILayout.Height( 20 ))){
					// FindReferencesTo(pair.key as Object);
					FindGOList(pair.Key);
				}
				// if(GOListTarget == mMat){
				// 	foreach(GameObject go in mGOMap[mMat])
				// 	{
				// 		GUILayout.BeginHorizontal();
				// 		EditorGUILayout.ObjectField( "", go, typeof(GameObject), true );
				// 		GUILayout.EndHorizontal();
				// 	}
				// }
				Material newValue = ( (Material)EditorGUILayout.ObjectField( "", pair.Key != pair.Value ? pair.Value : null, typeof(Material), false ) );
				GUILayout.EndHorizontal();
				
				if(GOListTarget == mMat && GOList.Count > 0){
					DisplayGOList();
				}
 
				// although this would override previous changes in the list only one change is expected per update
				if( ( newValue != null ) && ( newValue != pair.Value ) )
					transition = new KeyValuePair<Material, Material>( pair.Key, newValue );
			}
 

			// update material map with new transition
			if( transition.Key != null )
				mMaterialMap[transition.Key] = transition.Value;
 
 
			if(MissingMatRenderer.Count > 0){
				// Missing Renderers
				GUILayout.BeginHorizontal();
				GUILayout.Label( "Missing("+MissingMatRenderer.Count+")", GUILayout.MaxWidth( 250 ) );
				NewMatForMissing = ( (Material)EditorGUILayout.ObjectField( "", NewMatForMissing, typeof(Material), false ) );
				GUILayout.EndHorizontal();
			}
			

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
 
			GUILayout.BeginHorizontal();
 
			// load previously saved transitions from xml file
			// if( GUILayout.Button( "Load XML" ) )
			// 	loadMaterialMap();
 
			// // save the current material map to xml file
			// if( GUILayout.Button( "Save XML" ) )
			// 	saveMaterialMap();
 
			GUILayout.EndHorizontal();
 
			// Buttons //
			if( GUILayout.Button( "Apply Changes" ) ) // Save the material changes
			{
				applyChanges();
				resetMaterialMap();
			}
		}

		if( GUILayout.Button( "Refresh" ) ) // Save the material changes
		{
			resetMaterialMap();
		}
	}

	void ShowGOList(Material target){
		GOListTarget = target;
	}

	void DisplayGOList(){
		mGOScrollPosition = EditorGUILayout.BeginScrollView(mGOScrollPosition);
		foreach(GameObject go in GOList)
		{
			GUILayout.BeginHorizontal();
			// GUILayout.Space(250);
			EditorGUILayout.ObjectField( "", go, typeof(GameObject), true );
			GUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
	}

	void FindGOList(Material target){
		GOList.Clear();
		if(GOListTarget == target){
			return; // Toggle off
		}
		GOListTarget = target;
		if(mObject != null){
			Renderer [] renderers = mObject.GetComponentsInChildren<Renderer>();
			foreach( Renderer renderer in renderers )
				foreach( Material mat in renderer.sharedMaterials )
					if( mat == GOListTarget )
					{
						// mMaterialMap[mat] = mat;
						if(GOList.Count < 101)
							GOList.Add(renderer.gameObject);
						
					}
					else
					{
						Debug.Log( "Missing material in game object '" + renderer.name + "'" );
						MissingMatRenderer.Add(renderer);
					}
 
		}

	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Resets the material map.
	/// </summary>
	// ------------------------------------------------------------------------
	private void resetMaterialMap()
	{
		if(GOList.Count > 0)
			GOList.Clear();
		MissingMatRenderer.Clear();
		mMaterialMap.Clear();
		// mGOCount.Clear();
		// mGOMap.Clear();
 
		if( mObject != null )
		{
			GameObject instance;
			if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
				instance = (GameObject)PrefabUtility.InstantiatePrefab( mObject );
			else
				instance = mObject;
 
			Renderer [] renderers = instance.GetComponentsInChildren<Renderer>();
			foreach( Renderer renderer in renderers )
				foreach( Material mat in renderer.sharedMaterials )
					if( mat != null )
					{
						mMaterialMap[mat] = mat;
						// if(mGOCount[mat] == null)
						// 	mGOCount[mat] = 0;
						// mGOCount[mat] += 1;
						// if(mGOMap[mat].Count < 11)
						// 	mGOMap[mat].Add(renderer.gameObject);
					}
					else
					{
						Debug.Log( "Missing material in game object '" + renderer.name + "'" );
						MissingMatRenderer.Add(renderer);
					}
 
			if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
				UnityEngine.Object.DestroyImmediate( instance );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Save material map to xml file.
	/// </summary>
	// ------------------------------------------------------------------------
	private void saveMaterialMap()
	{
		string path = EditorUtility.SaveFilePanel( "Save Material Map File", "", "material_map.xml", "xml" );
		if( path != string.Empty )
		{
			StringBuilder sb = new StringBuilder().AppendLine("Saved Transitions:");
 
			//Create the xml file for storing material changes
			XmlDocument doc = new XmlDocument();
			XmlElement transitionsNode = doc.CreateElement( "Transitions" );
			doc.AppendChild( transitionsNode );
 
			// save material changes to xml
			foreach( KeyValuePair<Material,Material> pair in mMaterialMap )
			{
				if( pair.Key != pair.Value )
				{
					XmlElement materialNode = doc.CreateElement( "Material" );
 
					XmlElement sourceNode = doc.CreateElement( "Original" );
					sourceNode.InnerText = pair.Key.name;
 
					XmlElement targetNode = doc.CreateElement( "Replacement" );
					targetNode.InnerText = pair.Value.name;
 
					materialNode.AppendChild( sourceNode );
					materialNode.AppendChild( targetNode );
					transitionsNode.AppendChild( materialNode );
 
					sb.Append( "'" ).Append( sourceNode.InnerText ).Append( "' -> '" ).Append( targetNode.InnerText ).AppendLine("'");
				}
			}
			doc.Save( path );
			Debug.Log( sb.ToString() );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Load material map from xml file.
	/// </summary>
	// ------------------------------------------------------------------------
	private void loadMaterialMap()
	{
		Dictionary<string,string> transitions = new Dictionary<string, string>();
 
		string path = EditorUtility.OpenFilePanel( "Load Material Map File", "", "xml" );
		if( path != string.Empty )
		{
			XmlDocument doc = new XmlDocument();
			doc.Load( path );
 
			XmlNodeList elemList = doc.GetElementsByTagName( "Material" );
 
			foreach( XmlNode node in elemList )
			{
				if( ( node.ChildNodes.Count == 2 ) &&
					( node.ChildNodes[0].Name == "Original" ) &&
					( node.ChildNodes[1].Name == "Replacement" ) )
				{
					transitions.Add( node.ChildNodes[0].InnerText, node.ChildNodes[1].InnerText );
				}
			}
 
			mapApplicableTransitions( transitions );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// Apply material map settings to selected prefab.
	/// </summary>
	// ------------------------------------------------------------------------
	public void applyChanges()
	{
		GameObject instance;
		if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
			instance = (GameObject)PrefabUtility.InstantiatePrefab( mObject );
		else
			instance = mObject;
 
		Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
 
		// change the materials for all renderer in the Game Object
		foreach( Renderer renderer in renderers )
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for( int i = 0; i < sharedMaterials.Length; i++ )
				if( sharedMaterials[i] != null )
					sharedMaterials[i] = mMaterialMap[sharedMaterials[i]];
 
			renderer.sharedMaterials = sharedMaterials;
		}

		foreach( Renderer renderer in MissingMatRenderer){
			renderer.sharedMaterial = NewMatForMissing;
		}
 
		if( PrefabUtility.GetPrefabType( mObject ) == PrefabType.Prefab )
		{
			//Save the changes into the prefab
			PrefabUtility.ReplacePrefab( instance, mObject, ReplacePrefabOptions.ConnectToPrefab );
 
			//destroy the previously created instance
			DestroyImmediate( instance );
		}
	}
 
	// ------------------------------------------------------------------------
	/// <summary>
	/// For each transition check if the source material is used by the object and the target material exists.
	/// If both is true the transition is applied in the material map.
	/// </summary>
	// ------------------------------------------------------------------------
	private void mapApplicableTransitions( Dictionary<string,string> transitions )
	{
		// retrieve all materials in our project asset folder
		string[] materialsInProject = Directory.GetFiles( "Assets\\", "*.mat", SearchOption.AllDirectories );
 
		Dictionary<string, string> materialAssets = new Dictionary<string, string>();
		foreach( string materialFile in materialsInProject )
			materialAssets[Path.GetFileNameWithoutExtension( materialFile )] = materialFile;
 
		// retrieve all substance archives in our project asset folder
		string[] substancesInProject = Directory.GetFiles( "Assets\\", "*.sbsar", SearchOption.AllDirectories );
 
		// cache all substance instances assets found in the dictionary since we had to load them anyway
		// Dictionary<string, ProceduralMaterial> substanceAssets = new Dictionary<string, ProceduralMaterial>();
		// foreach( string substanceFile in substancesInProject )
		// {
		// 	UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath( substanceFile.Replace( "\\", "/" ) );
		// 	foreach( UnityEngine.Object obj in objects )
		// 	{
		// 		ProceduralMaterial substance = obj as ProceduralMaterial;
		// 		if( substance != null )
		// 		{
		// 			try
		// 			{
		// 				substanceAssets.Add( substance.name, substance );
		// 			}
		// 			catch( ArgumentException )
		// 			{
		// 				Debug.LogError("Multiple occurences of substance '" + substance.name + "' found which will be ignored.");
		// 			}
		// 		}
		// 	}
		// }
 
		// foreach object material check if a material transition with the material as source exist
		StringBuilder sb = new StringBuilder().AppendLine( "Mapped Transitions:" );
		foreach( Material mat in mMaterialMap.Keys )
		{
			string targetName;
			if( transitions.TryGetValue( mat.name, out targetName ) )
			{
				Material targetMaterial = null;
 
				// search for the target material
				string materialAssetPath;
				if( materialAssets.TryGetValue( targetName, out materialAssetPath ) )
				{
					targetMaterial = (Material)AssetDatabase.LoadAssetAtPath( materialAssetPath.Replace( "\\", "/" ), typeof(Material) );
				}
				else
				{
					// try to retrieve substance with the given target name
					// ProceduralMaterial substance;
					// if( substanceAssets.TryGetValue( targetName, out substance ) )
					// 	targetMaterial = substance;
				}
 
				if( targetMaterial != null )
				{
					mMaterialMap[mat] = targetMaterial;
					sb.Append( "'" ).Append( mat.name ).Append( "' -> '" ).Append( targetMaterial.name ).AppendLine( "'" );
				}
				else
					Debug.Log( "Material '" + targetName + "' specified in the transition file could not be found." );
			}
		}
		Debug.Log( sb.ToString() );
	}

	
	// Via https://answers.unity.com/questions/321615/code-to-mimic-find-references-in-scene.html
	private static void FindReferencesTo(Object to)
	{
		var referencedBy = new List<Object>();
		var allObjects = Object.FindObjectsOfType<GameObject>();
		for (int j = 0; j < allObjects.Length; j++)
		{
			var go = allObjects[j];
	
			if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
			{
				if (PrefabUtility.GetPrefabParent(go) == to)
				{
					Debug.Log(string.Format("referenced by {0}, {1}", go.name, go.GetType()), go);
					referencedBy.Add(go);
				}
			}
	
			var components = go.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				var c = components[i];
				if (!c) continue;
	
				var so = new SerializedObject(c);
				var sp = so.GetIterator();
	
				while (sp.NextVisible(true))
					if (sp.propertyType == SerializedPropertyType.ObjectReference)
					{
						if (sp.objectReferenceValue == to)
						{
							Debug.Log(string.Format("referenced by {0}, {1}", c.name, c.GetType()), c);
							referencedBy.Add(c.gameObject);
						}
					}
			}
		}
	
		if (referencedBy.Count > 0)
			Selection.objects = referencedBy.ToArray();
		else Debug.Log("no references in scene");
	}
}