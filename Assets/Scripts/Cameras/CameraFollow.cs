using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;
    public Vector3 targetOffset = new Vector3(0, 2, 0);
    public float maxDistance = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targetObject != null) {
            var targetPostion = (
                targetObject.transform.position +
                targetOffset
            );
            var curDistance = Vector3.Distance(
                transform.position,
                targetPostion
            );
            if (curDistance > maxDistance) {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPostion,
                    curDistance - maxDistance
                );
            }
            transform.LookAt(targetObject.transform);
        }
    }
}
