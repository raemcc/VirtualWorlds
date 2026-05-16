using UnityEngine;

public class Curtain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void hoverEnter()
    {
        Debug.Log("hover entered");
    }

    // Update is called once per frame
    public void selected()
    {
        Debug.Log("selected");
    }

    private Animator animator;
    private bool isOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleCurtain()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }
}
