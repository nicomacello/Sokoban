using UnityEngine;

public class NextLevelDoor : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; 
            
            Debug.Log("Passaggio al livello successivo attivato!");

            LevelGenerator generator = FindFirstObjectByType<LevelGenerator>();
            if (generator != null)
            {
                generator.LoadNextLevel();
            }
        }
    }
}
