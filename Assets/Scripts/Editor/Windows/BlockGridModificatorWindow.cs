using UnityEngine;
using UnityEditor;
using Unity.Burst.CompilerServices;

public class BlockGridModificatorWindow : EditorWindow
{
    private GameObject _objectToSpawn;

    private LevelManager _levelManager;

    [MenuItem("Window/Block Grid Modificator")]
    public static void ShowWindow()
    {
        GetWindow<BlockGridModificatorWindow>("Block Grid Modificator");
    }

    private void OnGUI()
    {


        _levelManager = GameObject.FindObjectOfType<LevelManager>();

        if (_levelManager != null)
        {
            GUILayout.Label("Predefined blocks", EditorStyles.boldLabel);
            _objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Object to Spawn", _objectToSpawn, typeof(GameObject), false);

            if (GUILayout.Button("Spawn Object"))
            {
                SpawnObjectAtMousePosition();
            }
        }
        else
        {
            Debug.LogError("Instance of LevelManager class not found.");
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            SpawnObjectAtMousePosition();
            Event.current.Use();
        }
    }

    private void SpawnObjectAtMousePosition()
    {
        if (_objectToSpawn == null) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        GameObject spawnedObject = Instantiate(_objectToSpawn);
        if (spawnedObject != null)
        {
            Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawned Object");
            spawnedObject.transform.position = ray.GetPoint(0);
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}