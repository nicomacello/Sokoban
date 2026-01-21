using UnityEngine;
using System.Collections.Generic;

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

public class LevelGenerator : MonoBehaviour {
    public string fileName = "LevelData"; 
    public int levelToLoad = 0;

    public GameObject floorPrefab;      // ID 0
    public GameObject playerPrefab;     // ID 1
    public GameObject blockPrefab;      // ID 2
    public GameObject icePrefab;        // ID 3
    public GameObject wallPrefab;       // ID 4
    public GameObject turretPrefab;     // ID 5

    private void Start() {
        GenerateLevel(levelToLoad);
    }

    public void GenerateLevel(int index) {
        ClearLevel();

        TextAsset jsonAsset = Resources.Load<TextAsset>(fileName);
        
        if (jsonAsset == null) {
            Debug.LogError("Errore: Il file " + fileName + " non è stato trovato nella cartella Resources!");
            return;
        }

        LevelRoot root = JsonUtility.FromJson<LevelRoot>(jsonAsset.text);
        
        if (index >= root.Levels.Count) {
            Debug.LogWarning("Livello non trovato nel JSON!");
            return;
        }

        LevelConfig currentLevel = root.Levels[index];

        for (int i = 0; i < currentLevel.layoutData.Count; i++) {
            int x = i % root.gridWidth;
            int z = i / root.gridWidth;
            
            int id = currentLevel.layoutData[i];

            Vector3 position = new Vector3(x * root.gridSize, 0, -z * root.gridSize);

            SpawnById(id, position);
        }
    }

    void SpawnById(int id, Vector3 pos) {
        if (id != 3 && id != 4) {
            Instantiate(floorPrefab, pos, Quaternion.identity, transform);
        }

        GameObject prefab = null;
        switch (id) {
            case 1: prefab = playerPrefab; break;
            case 2: prefab = blockPrefab; break;
            case 3: prefab = icePrefab; break;
            case 4: prefab = wallPrefab; break;
            case 5: prefab = turretPrefab; break;
        }

        if (prefab != null) {
            float y = (id == 3 || id == 4) ? 0f : 0.5f;
            Vector3 finalPos = new Vector3(pos.x, y, pos.z);
            Instantiate(prefab, finalPos, Quaternion.identity, transform);
        }
    }

    public void ClearLevel() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }
}