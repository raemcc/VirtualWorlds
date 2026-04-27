using UnityEngine;

public class LavaBlobMover : MonoBehaviour
{
    public float bottomY = 0f;
    public float topY = 1f;
    public float speed = 0.3f;
    public float offset = 0f;

    private bool isAnimating = false;

    void Update()
    {
        if (isAnimating)
        {
            float t = (Mathf.Sin((Time.time * speed) + offset) + 1f) / 2f;
            float newY = Mathf.Lerp(bottomY, topY, t);
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                newY, 
                transform.localPosition.z
            );
        }
    }

    public void StartMoving()
    {
        isAnimating = true;
    }
}