//using TMPro;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerInteraction_Desktop : MonoBehaviour
//{
//    public float interactRange = 5f;
//    public Camera playerCam;

//    public TextMeshProUGUI keyCounter;
//    public TextMeshProUGUI interactText;

//    public int keyCount = 0;
//    public int desiredKeyAmount = 4;

//    public UnlockDoor UnlockDoor;

//    void Start()
//    {
//        UpdateKeyCounter(); 
//        interactText.gameObject.SetActive(false);
//    }

//    void Update()
//    {
//        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, interactRange))
//        {
//            if (hit.collider.CompareTag("Key"))
//            {
//                interactText.gameObject.SetActive(true);
//                interactText.text = "Press F to take Key";
//                if (Keyboard.current.fKey.wasPressedThisFrame)
//                {
//                    Destroy(hit.collider.gameObject);
//                    keyCount++;
//                    UpdateKeyCounter();
//                }  
//            }

//            if (hit.collider.CompareTag("Door") && keyCount >= desiredKeyAmount)
//            {
//                interactText.gameObject.SetActive(true);
//                interactText.text = "Press E to Unlock the Door";
//                if (Keyboard.current.eKey.wasPressedThisFrame)
//                {
//                    UnlockDoor.OpenDoor();
//                }
//            }
//            else if (hit.collider.CompareTag("Door") && keyCount < desiredKeyAmount)
//            {
//                interactText.gameObject.SetActive(true);
//                interactText.text = "You do not have enough Keys";
//            }

//            if (hit.collider.CompareTag("Button"))
//            {
//                interactText.gameObject.SetActive(true);
//                interactText.text = "Press E to Finish the Game";
//                if (Keyboard.current.eKey.wasPressedThisFrame)
//                {
//                    ButtonInteraction button = hit.collider.GetComponent<ButtonInteraction>();
//                    button.Press();
//                }
//            }
//        }
//        else
//            interactText.gameObject.SetActive(false);
//    }

//    private void UpdateKeyCounter()
//    {
//        keyCounter.text = "Key Count: " + keyCount.ToString();
//    }
//}

using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction_Desktop : NetworkBehaviour
{
    public float interactRange = 5f;
    public Camera playerCam;

    public TextMeshProUGUI interactText;

    // Shared key count for BOTH players
    public NetworkVariable<int> sharedKeyCount = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> sharedKeyCount2;

    public int desiredKeyAmount = 8;

    public UnlockDoor unlockDoor;

    void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        //UpdateKeyCounter(sharedKeyCount.Value);
        interactText.gameObject.SetActive(false);
    }

    //public override void OnNetworkSpawn()
    //{
    //    sharedKeyCount.OnValueChanged += OnSharedKeyCountChanged;
    //}

    //public override void OnNetworkDespawn()
    //{
    //    sharedKeyCount.OnValueChanged -= OnSharedKeyCountChanged;
    //}

    //private void OnSharedKeyCountChanged(int oldValue, int newValue)
    //{
    //    UpdateKeyCounter(newValue);
    //}


  
    void Update()
    {
        if (!IsOwner) return;
        if (playerCam == null) return;

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
                    NetworkObject netObj = hit.collider.GetComponent<NetworkObject>();
                    if (netObj != null)
                    {
                        PickUpKeyServerRpc(netObj.NetworkObjectId);
                        PickUpKeyClientRpc(netObj.NetworkObjectId);
                    }
                }
                return;
            }

            if (hit.collider.CompareTag("Door"))
            {
                interactText.gameObject.SetActive(true);
                if (sharedKeyCount.Value >= desiredKeyAmount)
                {
                    interactText.text = "Press E to Unlock the Door";
                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        unlockDoor.TryOpenDoorServerRpc();
                        Debug.Log("Open");
                    }
                }
                else
                    interactText.text = "You do not have enough Keys";
                return;
            }

            if (hit.collider.CompareTag("Button"))
            {
                interactText.gameObject.SetActive(true);
                interactText.text = "Press E to Finish the Game";
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    ButtonInteraction button = hit.collider.GetComponent<ButtonInteraction>();
                    if (button != null)
                        button.PressButtonServerRpc();
                }
                return;
            }
            interactText.gameObject.SetActive(false);
        }
        else
            interactText.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpKeyServerRpc(ulong keyNetworkObjectId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(keyNetworkObjectId, out NetworkObject keyObject))
            return;

        if (!keyObject.IsSpawned) 
            return;

        sharedKeyCount.Value++;

        keyObject.Despawn(true);
    }

    [ClientRpc]
    private void PickUpKeyClientRpc(ulong keyNetworkObjectId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(keyNetworkObjectId, out NetworkObject keyObject))
            return;

        sharedKeyCount.Value++;

        keyObject.Despawn(true);
    }

    //private void UpdateKeyCounter(int count)
    //{
    //    if (keyCounter != null)
    //        keyCounter.text = "Key Count: " + count.ToString();
    //}
}
