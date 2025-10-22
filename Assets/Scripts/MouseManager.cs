using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }

    private void Awake()
    {
        SetInstance();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ChangeMouseVisibility()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public Vector3 GetMousePositionInWorld()
    {
        float distanceFromCamera = 1.7f; //TO BE CHANGED according to the real table and camera
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = distanceFromCamera;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return worldPos;
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}
