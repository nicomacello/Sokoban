using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Block"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            TriggerGameOver();
            Destroy(gameObject);
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("<color=red>Giocatore colpito! Game Over.</color>");

        GameObject deathPanel = GameObject.FindGameObjectWithTag("Death");

        if (deathPanel != null)
        {
            deathPanel.transform.GetChild(0).gameObject.SetActive(true); 

            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("DeathPanel non trovato! Assicurati di aver creato il pannello UI e assegnato il Tag 'DeathUI'.");
            Time.timeScale = 0f;
        }
    }
}