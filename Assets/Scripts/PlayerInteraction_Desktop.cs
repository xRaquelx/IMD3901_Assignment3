using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction_Desktop : MonoBehaviour
{
    public float interactRange = 5f;
    public Camera playerCam;

    public TextMeshProUGUI keyCounter;
    public TextMeshProUGUI interactText;

    public int keyCount = 0;
    public int desiredKeyAmount = 4;

    public UnlockDoor UnlockDoor;

    void Start()
    {
        UpdateKeyCounter(); 
        interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Key"))
            {
                interactText.gameObject.SetActive(true);
                interactText.text = "Press F to take Key";
                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    Destroy(hit.collider.gameObject);
                    keyCount++;
                    UpdateKeyCounter();
                }  
            }

            if (hit.collider.CompareTag("Door") && keyCount >= desiredKeyAmount)
            {
                interactText.gameObject.SetActive(true);
                interactText.text = "Press E to Unlock the Door";
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    UnlockDoor.OpenDoor();
                }
            }
            else if (hit.collider.CompareTag("Door") && keyCount < desiredKeyAmount)
            {
                interactText.gameObject.SetActive(true);
                interactText.text = "You do not have enough Keys";
            }
        }
        else
            interactText.gameObject.SetActive(false);
    }

    private void UpdateKeyCounter()
    {
        keyCounter.text = "Key Count: " + keyCount.ToString();
    }
}
