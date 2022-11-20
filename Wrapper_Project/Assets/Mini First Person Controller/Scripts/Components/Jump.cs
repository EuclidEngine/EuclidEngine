using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody rigidbody;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;


    void Reset()
    {
        // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Awake()
    {
        // Get rigidbody.
        rigidbody = GetComponent<Rigidbody>();
    }

    public void changeJumpForce(float new_jump_force)
    {
        jumpStrength = new_jump_force;
    }

    void LateUpdate()
    {
        // Jump when the Jump button is pressed and we are on the ground.
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            float forceScale = Mathf.Clamp(rigidbody.gameObject.transform.localScale.y * 0.5f, 1f, Mathf.Infinity);
            rigidbody.AddForce(Vector3.up * 100 * jumpStrength * forceScale);
            Jumped?.Invoke();
        }
    }
}
