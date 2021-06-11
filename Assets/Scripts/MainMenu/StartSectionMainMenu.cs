using UnityEngine;

public class StartSectionMainMenu : SectionMainMenu
{
    private Vector2 m_DownMousePos, m_UpMousePos;
    private bool m_Holding = false;
    private Vector2 startPanelPosition;

    public override void Land()
    {
        base.Land();
        startPanelPosition = transform.position;
    }

    public override void Frame()
    {
        base.Frame();

        m_Holding = Input.GetMouseButton(0);
        Vector2 swipeVector = (m_UpMousePos - (Vector2)Input.mousePosition);

        // Mouse down
        if (Input.GetMouseButtonDown(0))
        {
            m_DownMousePos = Input.mousePosition;
        }

        // Mouse up
        else if (Input.GetMouseButtonUp(0))
        {
            m_UpMousePos = Input.mousePosition;

            // Swipe?
            float dist = Vector2.Distance(m_DownMousePos, m_UpMousePos);
            if(dist > MainMenu.minSwipeDistance)
            {
                OnSwipe(swipeVector.normalized);
            }

            transform.position = startPanelPosition;
        }

        // Animation
        if(m_Holding)
        {
            transform.position = startPanelPosition + new Vector2(0, Mathf.Clamp(-swipeVector.y * 0.005f, 0f, 4f));
        }else
        {
            transform.position = startPanelPosition;
        }

    }

    private void OnDrawGizmos()
    {
        if(m_Holding)
        {
            Gizmos.color = Color.red;

            Vector2 from = Camera.main.ScreenToWorldPoint(m_DownMousePos);
            Vector2 to = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Gizmos.DrawLine(from, to);
        }
    }

    private void OnSwipe (Vector2 dir)
    {
        float ratio = dir.x / dir.y;

        if (m_UpMousePos.y > m_DownMousePos.y && ratio < 0.35f)
        {
            MainMenu.instance.SetSection(MainMenu.instance.menuSection);
        }
    }
}
