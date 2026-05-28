using UnityEngine;
using TMPro;

public class SistemaPontos : MonoBehaviour
{
    public int comprasPontos = 0;

    public TMP_Text comprasPontosTxt;

    public void AdicionarPonto(int quantidade)
    {
        comprasPontos += quantidade;

        comprasPontosTxt.text = "Compras: " + comprasPontos;
    }
}