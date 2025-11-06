using AIBot;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class AIInputFeeder : MonoBehaviour
{
    private SharedVector2 moveDir;
    public PlayerRoot PlayerRoot;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
    }

    void Start()
    {
        if (PlayerRoot == null || PlayerRoot.BotController == null)
        {
            Debug.LogError("PlayerRoot hoặc BotController chưa được tìm thấy.");
            return;
        }
        SharedVariable sharedVar = GlobalVariables.Instance.GetVariable("MoveDirection");
        if (sharedVar != null)
        {
            moveDir = sharedVar as SharedVector2;
        }
        if (moveDir == null)
        {
            return;
        }
        Debug.Log("================ Global Variable Setup ================");
        Debug.Log($"SharedVariable Found: {moveDir.Name}");
        Debug.Log($"Initial MoveDirection Value: {moveDir.Value}");
        Debug.Log("=======================================================");
    }

    void Update()
    {
        if (PlayerRoot.PlayerAssetsInputs == null || moveDir == null)
            return;

        Vector2 currentMoveInput = moveDir.Value;
        if (currentMoveInput != Vector2.zero)
        {
            PlayerRoot.PlayerAssetsInputs.MoveInput(currentMoveInput);
        }
    }
}