using UnityEngine;
using System.Collections;

public class BananaEscorregou : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();

            if (player != null)
            {
                StartCoroutine(player.Spin());
            }

            Destroy(gameObject);
        }
    }
}