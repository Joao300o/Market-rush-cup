using UnityEngine;
using TMPro;
public class ColetaDeItens : MonoBehaviour
{
    public int comprasPontos = 0;
    public TMP_Text comprasPontosTxt;
   private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Destroy(gameObject);
            comprasPontos ++;
            comprasPontosTxt.text = "Compras: " + comprasPontos;
        }
    }

}
