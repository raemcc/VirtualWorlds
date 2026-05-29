using UnityEngine;

public class Door : MonoBehaviour
{
    public void hoverEnter()
    {
        Debug.Log("hover entered");
    }

    private Animator animator;
        private bool isOpen = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void ToggleDoor()
        {
            isOpen = !isOpen;
            animator.SetBool("isOpen", isOpen);
        }
}
