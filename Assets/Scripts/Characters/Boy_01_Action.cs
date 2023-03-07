using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boy_01_Action : MonoBehaviour
{
    abstract public class Boy_01_ActionStatus
    {
        protected Boy_01_Action mAction;
        protected string mStatusName;
        protected int mStatusCode;

        public Boy_01_ActionStatus(Boy_01_Action action, string name)
        {
            mAction = action;
            mStatusName = name;
            mStatusCode = Animator.StringToHash(name);
        }

        public string GetStatusName() { return mStatusName; }
        public int GetStatusCode() { return mStatusCode; }
        virtual public void Start()
        {
            // Debug.Log("entering status: " + mStatusName);
            mAction.mAnimator.CrossFade(mStatusCode, 0.1f, 0, 0);
        }
        virtual public void Update(float deltaTime) { }
    }

    class Boy_01_ActionStatus_Idle : Boy_01_ActionStatus
    {
        public Boy_01_ActionStatus_Idle(Boy_01_Action action)
            : base(action, "Base Layer.idle") { }

        override public void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // start to jump
                mAction.SetStatus("Base Layer.jump");
                return;
            }

            float forwarding = Input.GetAxis("Vertical");
            if (forwarding > 0.0f)
            {
                // change to move status
                mAction.SetStatus("Base Layer.move");
                return;
            }
        }
    }

    class Boy_01_ActionStatus_Move : Boy_01_ActionStatus
    {
        protected int mSpeedKey;
        protected float mSpeed;

        public Boy_01_ActionStatus_Move(Boy_01_Action action)
            : base(action, "Base Layer.move") 
        {
            mSpeedKey = Animator.StringToHash("Speed");
        }

        override public void Update(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // start to jump
                mAction.SetStatus("Base Layer.jump");
                return;
            }

            float forwarding = Input.GetAxis("Vertical");
            if (forwarding <= 0.0f)
            {
                // back to idle
                mAction.SetStatus("Base Layer.idle");
                return;
            }

            mAction.SetMovingSpeedUp(forwarding);

            float curSpeed = mAction.GetMovingSpeed();
            if (curSpeed != mSpeed)
            {
                mSpeed = curSpeed;
                mAction.mAnimator.SetFloat(mSpeedKey, mSpeed);
            }

            float turning = Input.GetAxis("Horizontal");
            mAction.SetTurningSpeedUp(turning);

            forwarding *= mAction.GetMovingSpeed() * deltaTime;
            turning *= mAction.GetTurningSpeed() * deltaTime;

            mAction.transform.Rotate(Vector3.up, turning);
            mAction.mController.Move(mAction.transform.forward * forwarding);
        }
    }

    class Boy_01_ActionStatus_Jump : Boy_01_ActionStatus
    {
        protected int mSpeedKey;
        protected float mMoveSpeed;
        protected float mJumpSpeed;
        protected bool mUpdated;

        public Boy_01_ActionStatus_Jump(Boy_01_Action action)
            : base(action, "Base Layer.jump")
        {
            mSpeedKey = Animator.StringToHash("JumpPose");
        }

        public override void Start()
        {
            base.Start();
            mUpdated = false;
            mMoveSpeed = mAction.GetMovingSpeed();
            mJumpSpeed = mAction.GetJumpingSpeed();
            mAction.mAnimator.SetFloat(mSpeedKey, 8.0f);
        }

        override public void Update(float deltaTime)
        {
            // Debug.Log(deltaTime.ToString());

            if (mUpdated && mAction.mController.isGrounded)
            {
                // ÆðÌøºóÂäµØ
                if (mMoveSpeed > 0.0f)
                {
                    mAction.SetStatus("Base Layer.move");
                }
                else
                {
                    mAction.SetStatus("Base Layer.idle");
                }
            }
            else
            {
                if (!mUpdated) { mUpdated = true; }

                var direction = mAction.transform.forward * mMoveSpeed * deltaTime;
                if (mJumpSpeed > 0.0f)
                {
                    var diffJumpSpeed = deltaTime * 9.8f;
                    var curJumpSpeed = mJumpSpeed - diffJumpSpeed;
                    if (curJumpSpeed > 0.0f)
                    {
                        direction += Vector3.up * (mJumpSpeed - (diffJumpSpeed * 0.5f)) * deltaTime;
                    }
                    mJumpSpeed = curJumpSpeed;
                }

                mAction.mController.Move(direction);
            }
        }
    }

    public float moveSpeed = 1.0f;
    public float turnSpeed = 100.0f;
    public float jumpSpeed = 100.0f;

    protected float mMoveSpeedUp = 0.0f;
    protected float mTurnSpeedUp = 0.0f;

    protected Animator mAnimator;
    protected CharacterController mController;
    protected Boy_01_ActionStatus mStatus;
    protected Dictionary<string, Boy_01_ActionStatus> mStatusStore;

    void SetMovingSpeedUp(float speedUp)
    { 
        mMoveSpeedUp = speedUp > 0.0f ? speedUp : -speedUp;
    }
    void SetTurningSpeedUp(float speedUp)
    {
        mTurnSpeedUp = speedUp > 0.0f ? speedUp : -speedUp;
    }
    float GetMovingSpeed() { return moveSpeed * mMoveSpeedUp; }
    float GetTurningSpeed() { return turnSpeed * mTurnSpeedUp; }
    float GetJumpingSpeed() { return jumpSpeed; }

    void SetStatus(string statusName)
    {
        Boy_01_ActionStatus status;
        if (!mStatusStore.TryGetValue(statusName, out status))
        {
            return;
        }

        if (mStatus.GetStatusCode() == status.GetStatusCode())
        {
            return;
        }

        mStatus = status;
        mStatus.Start();
    }

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        mController = GetComponent<CharacterController>();

        var statusIdle = new Boy_01_ActionStatus_Idle(this);
        var statusMove = new Boy_01_ActionStatus_Move(this);
        var statusJump = new Boy_01_ActionStatus_Jump(this);

        mStatusStore = new Dictionary<string, Boy_01_ActionStatus>
        {
            { statusIdle.GetStatusName(), statusIdle },
            { statusMove.GetStatusName(), statusMove },
            { statusJump.GetStatusName(), statusJump },
        };
        mStatus = statusIdle;
        mStatus.Start();
    }

    void UpdateSpeedUp(float deltaTime)
    {
        if (mMoveSpeedUp > 0.0f)
        {
            mMoveSpeedUp -= deltaTime;
        }

        if (mMoveSpeedUp < 0.0f)
        {
            mMoveSpeedUp = 0.0f;
        }

        if (mTurnSpeedUp > 0.0f)
        {
            mTurnSpeedUp -= deltaTime;
        }

        if (mTurnSpeedUp < 0.0f)
        {
            mTurnSpeedUp = 0.0f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateSpeedUp(Time.fixedDeltaTime);

        if (mStatus != null)
        {
            mStatus.Update(Time.fixedDeltaTime);
        }
    }
}
