using UnityEngine;

public class StartSectionMainMenu : SectionMainMenu
{
    private Vector2 m_DownMousePos, m_UpMousePos;
    private bool m_Holding = false;
    private Vector2 startPanelPosition;

    private Vector2 swipeLimits;

    private void Awake()
    {
        swipeLimits.x = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).y;
        swipeLimits.y = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height * 0.4f)).y;
    }

    public override void Land()
    {
        base.Land();
        startPanelPosition = transform.position;
    }

    public override void Frame()
    {
        base.Frame();
        m_Holding = Input.GetMouseButton(0);

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
                Vector2 swipeVector = m_UpMousePos - m_DownMousePos;
                OnSwipe(swipeVector.normalized);
            }
        }

        // Animation
        if(m_Holding)
        {
            float y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            y = Mathf.Clamp(y, swipeLimits.x, swipeLimits.y);
            
            transform.position = new Vector2(0, Mathf.Lerp(transform.position.y, y, 10 * Time.deltaTime));
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

        if (m_UpMousePos.y > m_DownMousePos.y && ratio < 0.4f)
        {
            MainMenu.instance.SetSection(MainMenu.instance.menuSection);
        }
    }
}
