//using UnityEngine;
//using UnityEngine.InputSystem;

//public class ButtonInteraction : MonoBehaviour
//{
//    public Timer Timer;

//    public float range = 0.1f;
//    private float speed = 1.5f;
//    private Vector3 startPos;

//    private bool isPressed = false;
//    private bool isAnimating = false;

//    //public GameObject spawn;

//    void Start()
//    {
//        startPos = transform.localPosition;
//    }

//    void Update()
//    {
//        Vector3 target = isPressed ? startPos - Vector3.up * range : startPos;
//        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);

//        if (Vector3.Distance(transform.localPosition, target) < 0.001f)
//        {
//            if (isPressed)
//                isPressed = false;
//            else
//                isAnimating = false;
//        }
//    }

//    public void Press()
//    {
//        Timer.StopTimer();
//        isPressed = true;
//        isAnimating = true;
//    }
//}

using Unity.Netcode;
using UnityEngine;

public class ButtonInteraction : NetworkBehaviour
{
    public Timer Timer;

    public float range = 0.1f;
    private float speed = 1.5f;
    private Vector3 startPos;

    private bool isPressed = false;
    private bool isAnimating = false;

    // 0 = nobody yet
    // 1 = player 1
    // 2 = player 2
    public NetworkVariable<int> winnerPlayer = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Toggle this to replay the animation on all clients
    public NetworkVariable<bool> pressSignal = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Start()
    {
        startPos = transform.localPosition;
    }

    public override void OnNetworkSpawn()
    {
        pressSignal.OnValueChanged += OnPressSignalChanged;
        winnerPlayer.OnValueChanged += OnWinnerChanged;
    }

    public override void OnNetworkDespawn()
    {
        pressSignal.OnValueChanged -= OnPressSignalChanged;
        winnerPlayer.OnValueChanged -= OnWinnerChanged;
    }

    void Update()
    {
        if (!isAnimating) return;

        Vector3 target = isPressed ? startPos - Vector3.up * range : startPos;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, target) < 0.001f)
        {
            if (isPressed)
                isPressed = false;
            else
                isAnimating = false;
        }
    }

    private void OnPressSignalChanged(bool oldValue, bool newValue)
    {
        PlayPressAnimation();

        if (Timer != null)
            Timer.StopTimer();
    }

    private void PlayPressAnimation()
    {
        isPressed = true;
        isAnimating = true;
    }

    private void OnWinnerChanged(int oldValue, int newValue)
    {
        if (newValue == 1)
            Debug.Log("player 1 wins");
        else if (newValue == 2)
            Debug.Log("player 2 wins");
    }

    [ServerRpc(RequireOwnership = false)]
    public void PressButtonServerRpc(ServerRpcParams rpcParams = default)
    {
        // Replay button animation for everybody
        pressSignal.Value = !pressSignal.Value;

        // First person to press wins
        if (winnerPlayer.Value == 0)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;

            // Host/clientId 0 -> player 1
            // Second player -> player 2
            winnerPlayer.Value = senderId == 0 ? 1 : 2;
        }
    }
}
