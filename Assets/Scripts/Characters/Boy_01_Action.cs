using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boy_01_Action : MonoBehaviour
{
    private CharacterController mController;
    public float forwardSpeed = 1.0f;
    public float turnSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        mController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mController == null) {
            // no controller found
            return;
        }

        float forwarding = Input.GetAxis("Vertical");
        float turning = Input.GetAxis("Horizontal");
        forwarding *= forwardSpeed * Time.fixedDeltaTime;
        turning *= turnSpeed * Time.fixedDeltaTime;
        if (forwarding < 0.0f) {
            turning = -turning;
        }

        // rotate model
        transform.Rotate(Vector3.up, turning);

        // move forward
        mController.Move(transform.forward * forwarding);
    }
}
