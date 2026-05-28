using UnityEngine;
using TMPro;

public class WIn : MonoBehaviour
{
    public GameObject canvaWin;

    public TMP_Text textDeQNTDePontos;

    public SistemaPontos playerPontuacao;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canvaWin.SetActive(true);

            textDeQNTDePontos.text =
                "Compras realizadas: " 
                +  playerPontuacao.comprasPontos;
                Time.timeScale = 0f;
        }
    }
}
