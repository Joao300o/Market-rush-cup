using UnityEngine;

public class BananaEscorrega : MonoBehaviour
{
    public GameObject banana;
    public Transform bananaTransform;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(banana, bananaTransform.position,bananaTransform.rotation);
            Debug.Log("funciona");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
