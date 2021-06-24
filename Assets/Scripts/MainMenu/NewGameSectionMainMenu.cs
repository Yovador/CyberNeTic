using UnityEngine;
using UnityEngine.UI;

public class NewGameSectionMainMenu : MonoBehaviour
{
    public Button continueButton;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenPanel()
    {
        // Open animation
        animator.SetBool("IsOpen", true);
        
        // Show "Continue button" if a save exists
        continueButton.gameObject.SetActive(SaveManager.SaveExists());
    }

    public void ClosePanel()
    {
        // Close animation
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
