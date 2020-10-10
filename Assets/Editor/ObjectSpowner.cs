using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ObjectOption 
{
    public GameObject objectToSpawn = null;
    public Transform parentTransform = null;
    public string objectKey;
}
public class ObjectSpowner : EditorWindow
{
    private Vector3 mousePos;

    public ObjectOption object1 = new ObjectOption();

    public bool objectSpawnerIsActivated = false;


    public Vector3 getMouseInScene() {
        return mousePos;
    }

    [MenuItem("Tools/ObjectSpowner Shortcut Key %1")]
    public static void ShowWindow() 
    {
        GetWindow(typeof(ObjectSpowner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Object 1", EditorStyles.boldLabel);

        object1.objectToSpawn = EditorGUILayout.ObjectField("Prefab to create", object1.objectToSpawn, typeof(GameObject), false) as GameObject;
        object1.parentTransform = EditorGUILayout.ObjectField("Prefab to create", object1.parentTransform, typeof(Transform), false) as Transform;
        object1.objectKey = EditorGUILayout.TextField("Aligned key", object1.objectKey);

        objectSpawnerIsActivated = GUILayout.Toggle(objectSpawnerIsActivated, "Active");

        if (objectSpawnerIsActivated) {
            CreateAnObjectAtCursor();
        }

        if (GUILayout.Button("Create")){
            CreateAnObjectAtCursor();
        }

    }

    private void CreateAnObjectAtCursor() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Default")))
        {

            if (Input.GetKey(KeyCode.Mouse0))
            {
                GameObject obj = Instantiate(object1.objectToSpawn, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
                obj.transform.parent = object1.parentTransform;
            }

        }
    }

    
}
