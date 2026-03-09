using UnityEngine;
using Unity.Netcode;
using TMPro;


public class AssignScripts : NetworkBehaviour
{
    public static AssignScripts assigner;

    public NetworkObject player1;
    public NetworkObject player2;

    public GameObject playerPC;

    public GameObject UnlockDoor;

    public GameObject door;
    public Transform openedPosition;
    public TextMeshProUGUI interactText;

    private void Awake()
    {
        if (assigner == null)
            assigner = this;
    }

    public override void OnNetworkSpawn()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj.OwnerClientId == 0)
            player1 = netObj;
        else if (netObj.OwnerClientId == 1)
            player2 = netObj;
    }

    void Update()
    {
        if (playerPC != null) 
        {
            playerPC.GetComponent<UnlockDoor>().door = door;
            playerPC.GetComponent<UnlockDoor>().openedPosition = openedPosition;
            //door.GetComponent<UnlockDoor>().doorText = playerPC.messagetext;
        }
    }
}
