//using TMPro;
//using UnityEngine;
//using System.Collections;

//public class UnlockDoor : MonoBehaviour
//{
//    public int requiredKeys = 4;

//    public GameObject door;
//    public float openSpeed = 2f;
//    public Transform openedPosition;

//    public TextMeshProUGUI doorText;

//    private Coroutine openRoutine;
//    private bool doorOpened = false;

//    void Start()
//    {
//        doorText.gameObject.SetActive(false);
//    }

//    public void OpenDoor()
//    {
//        doorText.gameObject.SetActive(true);
//        doorText.text = "Door is Opening";
//        if (openRoutine != null)
//            StopCoroutine(openRoutine);
//        openRoutine = StartCoroutine(SlideDoorToOpen());
//    }

//    IEnumerator SlideDoorToOpen()
//    {
//        while (Vector3.Distance(door.transform.position, openedPosition.position) > 0.01f)
//        {
//            door.transform.position = Vector3.MoveTowards(door.transform.position, openedPosition.position, openSpeed * Time.deltaTime);

//            yield return null;
//        }
//        door.transform.position = openedPosition.position;
//        doorOpened = true;
//        doorText.gameObject.SetActive(false);
//    }
//}

using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class UnlockDoor : NetworkBehaviour
{
    public int requiredKeys = 4;

    public GameObject door;
    public float openSpeed = 2f;
    public Transform openedPosition;

    public TextMeshProUGUI doorText;

    private Coroutine openRoutine;

    public NetworkVariable<bool> doorOpened = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Start()
    {
        if (doorText != null)
            doorText.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        doorOpened.OnValueChanged += OnDoorOpenedChanged;

        if (doorOpened.Value)
        {
            if (door != null && openedPosition != null)
                door.transform.position = openedPosition.position;
        }
    }

    public override void OnNetworkDespawn()
    {
        doorOpened.OnValueChanged -= OnDoorOpenedChanged;
    }

    private void OnDoorOpenedChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            if (doorText != null)
            {
                doorText.gameObject.SetActive(true);
                doorText.text = "Door is Opening";
            }

            if (openRoutine != null)
                StopCoroutine(openRoutine);

            openRoutine = StartCoroutine(SlideDoorToOpen());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryOpenDoorServerRpc()
    {
        if (doorOpened.Value) return;

        doorOpened.Value = true;
    }

    IEnumerator SlideDoorToOpen()
    {
        while (Vector3.Distance(door.transform.position, openedPosition.position) > 0.01f)
        {
            door.transform.position = Vector3.MoveTowards(
                door.transform.position,
                openedPosition.position,
                openSpeed * Time.deltaTime
            );

            yield return null;
        }

        door.transform.position = openedPosition.position;

        if (doorText != null)
            doorText.gameObject.SetActive(false);
    }
}
