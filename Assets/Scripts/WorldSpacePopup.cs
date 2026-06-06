using UnityEngine;

public class WorldSpacePopup : MonoBehaviour
{
    public GameObject popupPanel;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void Show()
    {
        if (popupPanel != null)
            popupPanel.SetActive(true);
    }

    public void Hide()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void Toggle()
    {
        if (popupPanel != null)
            popupPanel.SetActive(!popupPanel.activeSelf);
    }
}
