using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject pontosUi;
    public PlayerMove playerMove;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerMove.canMove = true;

            menuUI.SetActive(false);

            pontosUi.SetActive(true);

        }
    }
}