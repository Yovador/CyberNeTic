using UnityEngine;

public abstract class SectionMainMenu : MonoBehaviour
{
    private bool m_isCurrent = false;

    public virtual void Land()
    {
        m_isCurrent = true;
        gameObject.SetActive(true);
    }

    public virtual void Frame()
    {
    }

    public virtual void Exit()
    {
        m_isCurrent = false;
        gameObject.SetActive(false);
    }
}
