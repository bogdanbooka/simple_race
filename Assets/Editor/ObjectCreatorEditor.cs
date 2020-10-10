using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectCreator))]
public class ObjectCreatorEditor : Editor
{
    // Start is called before the first frame update

    private ObjectCreator myScript = null;

    public override void OnInspectorGUI()
    {
        if (myScript == null)
        {
            myScript = (ObjectCreator)target;
        }

        DrawDefaultInspector();
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.

        GUILayout.Label("Editor settings", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);

        if (myScript != null)
        {
            for (int i = 0; i < myScript.prefabs.Length; ++i) {
                GameObject o = myScript.prefabs[i];
                if (o == null) 
                {
                    continue;
                }
                
                EditorGUILayout.ObjectField($"Object {i+1}", AssetPreview.GetAssetPreview(o), typeof(Texture2D), false);
            }
        }
        
        EditorGUI.EndDisabledGroup();
    }

    private void OnSceneGUI()
    {
        if (myScript == null) 
        {
            myScript = (ObjectCreator)target; 
        }

        if (Event.current.type == EventType.KeyUp && Event.current.alt) {

            Debug.Log($" lol {Event.current.keyCode.ToString()}");

            string keyKodeString = Event.current.keyCode.ToString();

            int activePrefabIndex = 0;

            if (keyKodeString.StartsWith("Alpha")) 
            {
                int.TryParse(keyKodeString.Remove(0, 5), out activePrefabIndex);

                activePrefabIndex += -1;
            }


            if (activePrefabIndex < 0)
            {
                activePrefabIndex = 0;
            }
            if (activePrefabIndex >= myScript.prefabs.Length) 
            {
                activePrefabIndex = myScript.prefabs.Length - 1;
            }

            //EditorUtility.SetDirty(target);

            SpawnObjectAtCursor(activePrefabIndex, Event.current.mousePosition);

            Debug.Log($"activePrefabIndex was updated: {activePrefabIndex}");
        }

    }

    private void SpawnObjectAtCursor(int prefabIndex, Vector2 mousePos) 
    {
        ObjectCreator myScript = (ObjectCreator)target;

        Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePos);

        RaycastHit hitInfo;

        if (myScript.gameObject.scene.GetPhysicsScene().Raycast(worldRay.origin, worldRay.direction, out hitInfo, 10000, LayerMask.GetMask("Default")))
        {
            GameObject prefab_instance = PrefabUtility.InstantiatePrefab(myScript.prefabs[prefabIndex]) as GameObject;
            prefab_instance.transform.position = hitInfo.point;
            prefab_instance.transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            prefab_instance.transform.parent = myScript.parentTransform;

            EditorUtility.SetDirty(prefab_instance);

        }
    }
}
