using UnityEngine;
using System.Collections.Generic;


public class LevelGenerator : MonoBehaviour
{
    [Header("Configurazione File")]
    public string fileName = "LevelData"; 
    public int levelToLoad = 0;
    private int currentLevelIndex;

    [Header("Riferimenti")]
    public CameraScript cameraScript;

    [Header("Prefabs Pavimento (Y = -0.6)")]
    public GameObject floorPrefab;      // ID 0
    public GameObject icePrefab;        // ID 3

    [Header("Prefabs Oggetti (Y = 0)")]
    public GameObject playerPrefab;     // ID 1
    public GameObject blockPrefab;      // ID 2
    public GameObject wallPrefab;       // ID 4
    public GameObject turretPrefab;     // ID 5
    public GameObject heavyRockPrefab;  // ID 6
    public GameObject exitPrefab;       // ID 7
    public GameObject nextLevelPortalPrefab; // ID 8

    [Header("Parametri Fisici")]
    public float floorY = -0.6f;
    public float objectY = 0f;

    private void Start()
    {
        currentLevelIndex = levelToLoad;
        GenerateLevel(currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(fileName);
        if (jsonAsset == null) return;

        LevelRoot root = JsonUtility.FromJson<LevelRoot>(jsonAsset.text);
        
        currentLevelIndex++;

        // Reset se finiscono i livelli
        if (currentLevelIndex >= root.Levels.Count)
        {
            Debug.Log("Ritorno al primo livello.");
            currentLevelIndex = 0;
        }

        GenerateLevel(currentLevelIndex);
    }

    public void GenerateLevel(int index)
    {
        ClearLevel();

        TextAsset jsonAsset = Resources.Load<TextAsset>(fileName);
        if (jsonAsset == null)
        {
            Debug.LogError("File JSON non trovato in Resources!");
            return;
        }

        LevelRoot root = JsonUtility.FromJson<LevelRoot>(jsonAsset.text);
        if (index >= root.Levels.Count) return;

        LevelConfig level = root.Levels[index];
        Debug.Log($"Generando: {level.levelName} ({root.gridWidth}x{root.gridWidth})");

        for (int i = 0; i < level.layoutData.Count; i++)
        {
            // Calcolo coordinate basato sulla larghezza definita nel JSON
            int x = i % root.gridWidth;
            int z = i / root.gridWidth;
            int id = level.layoutData[i];

            Vector3 groundPos = new Vector3(x * root.gridSize, floorY, -z * root.gridSize);
            Vector3 objectPos = new Vector3(x * root.gridSize, objectY, -z * root.gridSize);

            SpawnObject(id, groundPos, objectPos);
        }
    }

    void SpawnObject(int id, Vector3 gPos, Vector3 oPos)
    {
        // 1. Sempre il pavimento sotto
        if (id == 3) 
            Instantiate(icePrefab, gPos, Quaternion.identity, transform);
        else 
            Instantiate(floorPrefab, gPos, Quaternion.identity, transform);

        // 2. Oggetto sopra
        GameObject prefab = id switch
        {
            1 => playerPrefab,
            2 => blockPrefab,
            4 => wallPrefab,
            5 => turretPrefab,
            6 => heavyRockPrefab,
            7 => exitPrefab,
            8 => nextLevelPortalPrefab,
            _ => null
        };

        if (prefab != null)
        {
            float finalY = (id == 7 || id == 8) ? floorY + 0.05f : objectY;
            GameObject obj = Instantiate(prefab, new Vector3(oPos.x, finalY, oPos.z), Quaternion.identity, transform);

            if (id == 1 && cameraScript != null)
            {
                cameraScript.SetTarget(obj.transform);
            }
        }
    }

    public void ClearLevel()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

    [System.Serializable] public class LevelRoot
     { 
        public int gridWidth; 
        public int gridHeight; 
        public float gridSize; 
        public List<LevelConfig> Levels; 
    }
    [System.Serializable] public class LevelConfig 
    { 
        public string levelName; 
        public List<int> layoutData; 
    }