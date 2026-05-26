using UnityEngine;

public class ColetaDeItens : MonoBehaviour
{
   private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
