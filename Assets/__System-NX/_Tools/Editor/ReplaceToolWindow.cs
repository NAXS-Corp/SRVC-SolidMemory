// using System.Linq;
// using UnityEngine;
// using UnityEditor;
// /// <summary>
// /// Replace tool window.
// /// </summary>
// public class ReplaceToolWindow : EditorWindow
// {
    
//     /// <summary>
//     /// Drawing GUI in window.
//     /// </summary>
//     private void OnGUI()
//     {
//         ...
//         // Replace button.
//         if (GUILayout.Button("Replace"))
//         {
//             // Check if replace object is assigned.
//             if (!replaceObjectField.objectReferenceValue)
//             {
//                 Debug.LogErrorFormat("[Replace Tool] {0}", "Missing prefab to replace with!");
//                 return;
//             }
//             // Check if there are objects to replace.
//             if (data.objectsToReplace.Length == 0)
//             {
//                 Debug.LogErrorFormat("[Replace Tool] {0}", "Missing objects to replace!");
//                 return;
//             }
//             ReplaceSelectedObjects(data.objectsToReplace, data.replaceObject);
//         }
//         ...
//     }
//     ...
//     /// <summary>
//     /// Replaces game objects with provided replace object.
//     /// </summary>
//     /// <param name="objectToReplace">Game Objects to replace.</param>
//     /// <param name="replaceObject">Replace object.</param>
//     private void ReplaceSelectedObjects(GameObject[] objectToReplace, GameObject replaceObject)
//     {
//         Debug.Log("[Replace Tool] Replace process");
//         // Loop through object to replace.
//         for (int i = 0; i < objectToReplace.Length; i++)
//         {
//             var go = objectToReplace[i];
//             // Register current game object to Undo action in editor.
//             Undo.RegisterCompleteObjectUndo(go, "Saving game object state");
//             // Creating replace object as the same position and same parent.
//             var inst = Instantiate(replaceObject, go.transform.position, go.transform.rotation, go.transform.parent);
//             inst.transform.localScale = go.transform.localScale;
//             // Register object creation for Undo action in editor.
//             Undo.RegisterCreatedObjectUndo(inst, "Replacement creation.");
//             // Changing parent for all children of current game object.
//             foreach (Transform child in go.transform)
//             {
//                 // Saving action for Undo action in editor.
//                 Undo.SetTransformParent(child, inst.transform, "Parent Change");
//             }
//             // Destroying current game object with save for Undo action in editor.
//             Undo.DestroyObjectImmediate(go);
//         }
//         Debug.LogFormat("[Replace Tool] {0} objects replaced on scene with {1}", objectToReplace.Length, replaceObject.name);
//     }
// }