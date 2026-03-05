using TMPro;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public int requiredKeys = 4;

    public GameObject door;
    public float openSpeed = 2f;
    public Transform openedPosition;

    public GameObject finishWall;

    private bool doorOpened = false;
    private bool playerFinished = false;

    public TextMeshProUGUI doorText;

    void Start()
    {
        doorText.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OpenDoor()
    {
        doorText.gameObject.SetActive(true);
        doorText.text = "Door is Opening";
        //float newX = Mathf.MoveTowards(door.transform.position.x, openedPosition.position.x, openSpeed * Time.deltaTime);
        //door.transform.position = new Vector3(newX, door.transform.position.y, door.transform.position.z);
        //if (Mathf.Abs(door.transform.position.x - openedPosition.position.x) < 0.1f)
        //{
        //    door.transform.position = new Vector3(openedPosition.position.x, door.transform.position.y, door.transform.position.z);
        //    doorOpened = true;
        //    doorText.gameObject.SetActive(false);
        //}

        //if (current door position is at the open door position)
        //{
        //    doorOpened = true;
        //    doorText.gameObject.SetActive(false);
        //}
    }
}
