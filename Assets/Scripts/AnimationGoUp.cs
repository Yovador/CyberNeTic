// //Attach this script to a GameObject with an Animator component attached.
// //Apply these parameters to your transitions between states

// //This script allows you to trigger an Animator parameter and reset the other that could possibly still be active. Press the up and down arrow keys to do this.
// // m_Animator.SetTrigger("GoUp");
// // m_Animator.ResetTrigger("GoUp");

// using UnityEngine;

// public class AnimationGoUp : MonoBehaviour
// {
//     Animator m_Animator;

//     void Start()
//     {
//         //Get the Animator attached to the GameObject you are intending to animate.
//         m_Animator = gameObject.GetComponent<Animator>();
//     }

//     void Update()
//     {
//         //Press the up arrow button to reset the trigger and set another one
//         if (Input.GetKey(KeyCode.UpArrow))
//         {
//             m_Animator.SetTrigger("GoUp");
//         }

//     }

//     MoveFlux(medium.spaceBetweenMessages + rectTransform.sizeDelta.y );

//         private void MoveFlux(float value)
//     {
//         //Monter des messages sans animation **TEMPORAIRE**
//         Vector2 newPos = new Vector2(conversationFlux.transform.position.x, conversationFlux.transform.position.y +  value);
//         conversationFlux.transform.position = newPos;
//         //**TEMPORAIRE**
//     }
// }