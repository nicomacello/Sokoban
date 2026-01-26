using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem; 

[System.Serializable]
public class LevelRoot {
    public int gridWidth;
    public int gridHeight;
    public float gridSize;
    public List<LevelConfig> Levels;
}

[System.Serializable]
public class LevelConfig {
    public string levelName;
    public List<int> layoutData;
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Configurazione File")]
    public string fileName = "LevelData"; 
    public int levelToLoad = 0;
    private int currentLevelIndex;

    [Header("Riferimenti")]
    public CameraScript cameraScript;

    [Header("Prefabs Pavimento (Y = -0.6)")]
    public GameObject floorPrefab;      
    public GameObject icePrefab;        

    [Header("Prefabs Oggetti (Y = 0)")]
    public GameObject playerPrefab;     
    public GameObject blockPrefab;      
    public GameObject wallPrefab;       
    public GameObject turretPrefab;     
    public GameObject heavyRockPrefab;  
    public GameObject exitPrefab;       
    public GameObject nextLevelPortalPrefab;
    public GameObject objectivePrefab;


    [Header("Parametri Fisici")]
    public float floorY = -0.6f;
    public float objectY = 0f;

    private int debugWidth;
    private int debugHeight;
    private float debugSize;

    private void Start()
    {
        currentLevelIndex = levelToLoad;
        GenerateLevel(currentLevelIndex);
    }

    private void Update()
    {
        // Reset rapido (R)
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetLevel();
        }

        // Chiusura gioco (ESC)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            QuitGame();
        }
    }

    // --- FUNZIONI DI NAVIGAZIONE ---

    public void ResetLevel()
    {
        GenerateLevel(currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(fileName);
        if (jsonAsset == null) return;

        LevelRoot root = JsonUtility.FromJson<LevelRoot>(jsonAsset.text);
        
        // Incrementa l'indice e torna a 0 se superi il numero di livelli
        currentLevelIndex++;
        if (currentLevelIndex >= root.Levels.Count)
        {
            currentLevelIndex = 0;
        }

        GenerateLevel(currentLevelIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // --- LOGICA DI GENERAZIONE ---

    public void GenerateLevel(int index)
    {
        ClearLevel();

        TextAsset jsonAsset = Resources.Load<TextAsset>(fileName);
        if (jsonAsset == null) return;

        LevelRoot root = JsonUtility.FromJson<LevelRoot>(jsonAsset.text);
        if (root == null || index >= root.Levels.Count) return;

        // Aggiorna l'indice corrente per il Reset
        currentLevelIndex = index;
        LevelConfig level = root.Levels[index];

        debugWidth = root.gridWidth;
        debugHeight = root.gridHeight;
        debugSize = root.gridSize;

        for (int i = 0; i < level.layoutData.Count; i++)
        {
            int x = i % root.gridWidth;
            int z = i / root.gridWidth;
            int id = level.layoutData[i];

            Vector3 groundPos = new Vector3(x * root.gridSize, floorY, -z * root.gridSize);
            Vector3 objectPos = new Vector3(x * root.gridSize, objectY, -z * root.gridSize);

            SpawnObject(id, groundPos, objectPos);
        }
        
        Debug.Log($"Livello caricato: {level.levelName} (Indice: {currentLevelIndex})");
    }

    void SpawnObject(int id, Vector3 gPos, Vector3 oPos)
    {
        // 1. Pavimento
        if (id == 3) 
            Instantiate(icePrefab, gPos, Quaternion.identity, transform);
        else 
            Instantiate(floorPrefab, gPos, Quaternion.identity, transform);

        // 2. Oggetto
        GameObject prefab = id switch
        {
            1 => playerPrefab,
            2 => blockPrefab,
            4 => wallPrefab,
            5 => turretPrefab,
            6 => heavyRockPrefab,
            7 => exitPrefab,
            8 => nextLevelPortalPrefab,
            9 => objectivePrefab,
            _ => null
        };

        if (prefab != null)
        {
            // Rotazione torretta -90 su Y
            Quaternion rotation = (id == 5) ? Quaternion.Euler(0, 90f, 0) : Quaternion.identity;
            
            float finalY = (id == 7 || id == 8) ? floorY + 0.05f : objectY;
            GameObject obj = Instantiate(prefab, new Vector3(oPos.x, finalY, oPos.z), rotation, transform);

            if (id == 1 && cameraScript != null)
                cameraScript.SetTarget(obj.transform);
        }
    }

    public void ClearLevel()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (debugWidth <= 0) return;
        Gizmos.color = Color.yellow;
        for (int x = 0; x < debugWidth; x++)
        {
            for (int z = 0; z < debugHeight; z++)
            {
                Vector3 pos = new Vector3(x * debugSize, floorY, -z * debugSize);
                Gizmos.DrawWireCube(pos, new Vector3(debugSize, 0.1f, debugSize));
            }
        }
    }
}

