using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelRoot
{
    public int gridWidth;
    public int gridHeight;
    public float gridSize;
    public List<LevelConfig> Levels;
}

[System.Serializable]
public class LevelConfig
{
    public string levelName;
    public List<int> layoutData;
}

public class LevelGenerator : MonoBehaviour
{
    
    public string fileName = "LevelData";
    public int levelToLoad = 0;

    public CameraScript cameraScript; 
    public GameObject floorPrefab;      // ID 0
    public GameObject icePrefab;        // ID 3

    public GameObject playerPrefab;     // ID 1
    public GameObject blockPrefab;      // ID 2
    public GameObject wallPrefab;       // ID 4
    public GameObject turretPrefab;     // ID 5
    public GameObject heavyRockPrefab;  // ID 6
    public GameObject exitPrefab;       // ID 7

    public float floorY = -0.6f;
    public float objectY = 0f;

    private void Start()
    {
        GenerateLevel(levelToLoad);
    }

    public void GenerateLevel(int index)
    {
        ClearLevel();

        TextAsset jsonAsset = Resources.Load<TextAsset>(fileName);
        if (jsonAsset == null)
        {
            Debug.LogError($"[LevelGenerator] File '{fileName}' non trovato nella cartella Assets/Resources!");
            return;
        }

        LevelRoot root = JsonUtility.FromJson<LevelRoot>(jsonAsset.text);
        if (root == null || index >= root.Levels.Count)
        {
            Debug.LogError("[LevelGenerator] Struttura JSON non valida o indice livello inesistente.");
            return;
        }

        LevelConfig currentLevel = root.Levels[index];

        for (int i = 0; i < currentLevel.layoutData.Count; i++)
        {
            int x = i % root.gridWidth;
            int z = i / root.gridWidth;
            int id = currentLevel.layoutData[i];

            Vector3 groundPos = new Vector3(x * root.gridSize, floorY, -z * root.gridSize);
            Vector3 objectPos = new Vector3(x * root.gridSize, objectY, -z * root.gridSize);

            SpawnLogic(id, groundPos, objectPos);
        }
    }

    void SpawnLogic(int id, Vector3 groundPos, Vector3 objectPos)
    {
        if (id == 3)
            Instantiate(icePrefab, groundPos, Quaternion.identity, transform);
        else
            Instantiate(floorPrefab, groundPos, Quaternion.identity, transform);

        GameObject prefabToSpawn = id switch
        {
            1 => playerPrefab,
            2 => blockPrefab,
            4 => wallPrefab,
            5 => turretPrefab,
            6 => heavyRockPrefab,
            7 => exitPrefab,
            _ => null
        };

        if (prefabToSpawn != null)
        {
            float spawnY = (id == 7) ? floorY + 0.05f : objectY;
            Vector3 finalPos = new Vector3(objectPos.x, spawnY, objectPos.z);
            
            GameObject spawnedObj = Instantiate(prefabToSpawn, finalPos, Quaternion.identity, transform);

            if (id == 1)
            {
                SetupCamera(spawnedObj.transform);
            }
        }
    }

    void SetupCamera(Transform playerTransform)
    {
        if (cameraScript != null)
        {
            cameraScript.SetTarget(playerTransform);
        }
        else
        {
            CameraScript foundCam = Object.FindFirstObjectByType<CameraScript>();
            if (foundCam != null) foundCam.SetTarget(playerTransform);
        }
    }

    public void ClearLevel()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

