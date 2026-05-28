using UnityEngine;

public class BoostPickup : MonoBehaviour
{
    [Header("Quanto tempo de boost dá")]
    public float boostAmount = 4f;

    public float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBoost boostSystem = other.GetComponent<PlayerBoost>();

            if (boostSystem != null)
            {
                boostSystem.AddBoost(boostAmount);
                
                // Feedback visual
                Destroy(gameObject);
            }
        }
    }
}