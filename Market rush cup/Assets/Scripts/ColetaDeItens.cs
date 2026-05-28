using UnityEngine;

public class ColetaDeItens : MonoBehaviour
{
    public int quantidade = 1;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            SistemaPontos pontuacao =
                col.GetComponent<SistemaPontos>();

            if (pontuacao != null)
            {
                pontuacao.AdicionarPonto(quantidade);
            }
        }
    }
}