// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
 
// public class PrefabReplace : EditorWindow
// {
//     [SerializeField] private GameObject prefab;
//     private bool selectionChanged;
//     private string objectsToSearch = "";
//     private List<GameObject> foundObjects = new List<GameObject>();
//     private GUIStyle guiStyle = new GUIStyle(); //create a new variable
//     private int count = 0;
//     private bool addFoundObjects;
//     private bool keepNames = true;
 
//     [MenuItem("Tools/Prefab Replace")]
//     static void CreateReplaceWithPrefab()
//     {
//         int width = 340;
//         int height = 370;
 
//         int x = (Screen.currentResolution.width - width) / 2;
//         int y = (Screen.currentResolution.height - height) / 2;
       
//         GetWindow<PrefabReplace>().position = new Rect(x, y, width, height);
//     }
 
//     private void OnGUI()
//     {
//         guiStyle.fontSize = 15; //change the font size
//         Searching();
//         GUILayout.Space(10);
//         Replacing();
//         GUILayout.Space(50);
//         Settings();
//     }
 
//     private void Searching()
//     {
//         //GUI.Label(new Rect(10, 15, 150, 20), "Search by name", guiStyle);
//         objectsToSearch = GUI.TextField(new Rect(90, 35, 150, 20), objectsToSearch, 25);
 
//         if (objectsToSearch != "")
//         {
//             GUI.enabled = true;
//         }
//         else
//         {
//             GUI.enabled = false;
//             count = 0;
//         }
//         GUILayout.Space(15);
//         if (GUILayout.Button("Search"))
//         {
//             foundObjects = new List<GameObject>();
//             count = 0;
 
//             foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
//             {
//                 if (gameObj.name == objectsToSearch)//.Contains(objectsToSearch))// == objectsToSearch)
//                 {
//                     count += 1;
//                     foundObjects.Add(gameObj);
//                     foreach (Transform child in gameObj.transform)
//                     {
//                         count += 1;
//                         foundObjects.Add(child.gameObject);
//                     }
//                 }
//             }
 
//             if(foundObjects.Count == 0)
//             {
//                 count = 0;
//             }
//         }
 
//         GUI.enabled = true;
//         EditorGUI.LabelField(new Rect(90, 65, 210, 15), "Number of found objects and childs");
//         GUI.TextField(new Rect(90, 80, 60, 15), count.ToString(), 25);
 
//         GUILayout.Space(100);
//         if (count > 0)
//         {
//             GUI.enabled = true;
//         }
//         else
//         {
//             GUI.enabled = false;
//         }
//         if (GUILayout.Button("Replace found objects"))
//         {
//             if (prefab != null)      
//             {
//                 InstantiatePrefab(foundObjects);
//             }
//         }
 
//         GUI.enabled = true;
//     }
 
//     private void Replacing()
//     {
//         GUILayout.Space(20);
//         GUILayout.BeginVertical(GUI.skin.box);
//         GUILayout.Label("Replacing");
//         GUILayout.Space(20);
 
//         prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
 
//         var selection = Selection.objects.OfType<GameObject>().ToList();
 
//         if (selectionChanged)
//         {
//             if (selection.Count == 0)
//                 GUI.enabled = false;
 
//             for (var i = selection.Count - 1; i >= 0; --i)
//             {
//                 var selectedObject = selection[i];
//                 if (prefab != null && selection.Count > 0 &&
//                     selectedObject.scene.name != null
//                     && prefab != PrefabUtility
//                     .GetCorrespondingObjectFromSource(selectedObject))
//                 {
//                     GUI.enabled = true;
//                 }
//                 else
//                 {
//                     GUI.enabled = false;
//                 }
//             }
//         }
//         else
//         {
//             GUI.enabled = false;
//         }
 
//         if (GUILayout.Button("Replace"))
//         {
//             InstantiatePrefab(selection);
//             selectionChanged = false;
//         }
 
//         GUILayout.Space(10);
//         GUI.enabled = true;
//         EditorGUILayout.LabelField("Selection count: " + Selection.objects.OfType<GameObject>().Count());
 
//         GUILayout.EndVertical();
//     }
 
//     private void Settings()
//     {
//         keepNames = GUILayout.Toggle(keepNames, "Keep Names");
//     }
 
//     private void OnInspectorUpdate()
//     {
//         Repaint();
//     }
 
//     private void OnSelectionChange()
//     {
//         selectionChanged = true;
//     }
 
//     private void InstantiatePrefab(List<GameObject> selection)
//     {
//         if (prefab != null && selection.Count > 0)
//         {
//             for (var i = selection.Count - 1; i >= 0; --i)
//             {
//                 var selected = selection[i];
//                 Component[] components = selected.GetComponents(typeof(MonoBehaviour));
//                 if (components.Length == 0)
//                 {
//                     SceneManager.SetActiveScene(SceneManager.GetSceneByName(selected.scene.name));
 
//                     var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
//                     GameObject newObject;
 
//                     if (prefabType == PrefabAssetType.Regular)
//                     {
//                         newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
//                     }
//                     else
//                     {
//                         newObject = Instantiate(prefab);
 
//                         if (keepNames == false)
//                         {
//                             newObject.name = prefab.name;
//                         }
//                     }
//                     if (newObject == null)
//                     {
//                         Debug.LogError("Error instantiating prefab");
//                         break;
//                     }
 
//                     Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
//                     newObject.transform.parent = selected.transform.parent;
//                     newObject.transform.localPosition = selected.transform.localPosition;
//                     newObject.transform.localRotation = selected.transform.localRotation;
//                     newObject.transform.localScale = selected.transform.localScale;
//                     newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
//                     if (keepNames == true)
//                     {
//                         newObject.name = selected.name;
//                     }
//                     Undo.DestroyObjectImmediate(selected);
//                 }
//             }
//         }
//     }
// }
 