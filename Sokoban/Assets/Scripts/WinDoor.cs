using UnityEngine;

public class WinDoor : MonoBehaviour
{
    [SerializeField]private GameObject winPanel;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Debug.Log("Vittoria Raggiunta!");
    }
}
