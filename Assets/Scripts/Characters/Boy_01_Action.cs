using System.Collections;
using System.Collections.Generic;
using UnityEngine;


abstract class Boy_01_ActionStatus
{
    protected Boy_01_Action mAction;

    public Boy_01_ActionStatus(Boy_01_Action action)
    {
        this.mAction = action;
    }

    abstract void FixedUpdate();
}

class Boy_01_ActionStatus_Idle : Boy_01_ActionStatus
{
    void FixedUpdate()
    {
        float forwarding = Input.GetAxis("Vertical");
        // float turning = Input.GetAxis("Horizontal");
        if (forwarding != 0.0f)
        {
            // change to walk status
            mAction.SetStatus()
        }
    }
}




public class Boy_01_Action : MonoBehaviour
{
    protected Animator mAnimator;
    protected CharacterController mController;
    protected Boy_01_ActionStatus mStatus;
    protected Dictionary<int, Boy_01_ActionStatus> mStatusStore;

    public float forwardSpeed = 1.0f;
    public float turnSpeed = 100.0f;

    private static readonly int mStatusCodeIdle = Animator.StringToHash("Base Layer.idle");
    private static readonly int mStatusCodeMove = Animator.StringToHash("Base Layer.move");
    private static readonly int mStatusCodeJump = Animator.StringToHash("Base Layer.jump");
    private static readonly int mStatusCodeDamage = Animator.StringToHash("Base Layer.damage");
    private static readonly int mStatusCodeDown = Animator.StringToHash("Base Layer.down");
    private static readonly int mStatusCodeFaint = Animator.StringToHash("Base Layer.faint");
    private static readonly int mStatusCodeStandupFaint = Animator.StringToHash("Base Layer.standup_faint");

    private static readonly int JumpTag = Animator.StringToHash("Jump");
    private static readonly int DamageTag = Animator.StringToHash("Damage");
    private static readonly int FaintTag = Animator.StringToHash("Faint");

    private static readonly int SpeedParameter = Animator.StringToHash("Speed");
    private static readonly int JumpPoseParameter = Animator.StringToHash("JumpPose");

    void SetStatus(int statusCode)
    {
        mStatusStore.TryGetValue(statusCode, out mStatus);
    }

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        mController = GetComponent<CharacterController>();
        mStatusStore = new Dictionary<int, Boy_01_ActionStatus>
        {
            (Animator.StringToHash("Base Layer.idle"), new Boy_01_ActionStatus()),
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float forwarding = Input.GetAxis("Vertical");
        float turning = Input.GetAxis("Horizontal");
        forwarding *= forwardSpeed * Time.fixedDeltaTime;
        turning *= turnSpeed * Time.fixedDeltaTime;
        if (forwarding < 0.0f) {
            turning = -turning;
        }

        // rotate model
        transform.Rotate(Vector3.up, turning);

        if (mController != null)
        {
            // move forward
            mController.Move(transform.forward * forwarding);
        }
    }
}
