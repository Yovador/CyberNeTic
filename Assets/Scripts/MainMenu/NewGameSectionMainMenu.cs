using UnityEngine;

public class NewGameSectionMainMenu : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenPanel()
    {
        animator.SetBool("IsOpen", true);
    }

    public void ClosePanel()
    {
        animator.SetBool("IsOpen", false);
    }

    public void Print (string message)
    {
        Debug.Log(message);
    }

    public void ContinueGame()
    {
        GameManager.instance.ContinueGame();
    }

    public void StartNewGame()
    {
        GameManager.instance.StartNewGame();
    }
}
