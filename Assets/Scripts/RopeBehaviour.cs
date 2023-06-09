using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//a rope consists of connected rigidbodys(segments), created from a prefab. The ropes last segment is connected to the "hook".
//This can be any rigidbody you call "reconnectHoock()" on


public class RopeBehaviour : MonoBehaviour
{
    [SerializeField]
    Transform ropeSegment = null;

    Transform hook = null;

    Transform hand;

    Transform[] segments;

    Rigidbody2D hookBody;

    DistanceJoint2D hookJoint;

    private int segmentIndex = 0;


    public void setHand(Transform newHand)
    {
        hand = newHand;
    }

    public Transform createSegment()
    {

        Transform segment;

        segment = Instantiate(ropeSegment);
        segment.transform.position = hook.position;
        segment.name = "segment " + segmentIndex;
        segment.GetComponent<RopePhysics>().setHand(hand);

        return segment;
    }

    public void connectWithJoints(Transform segment1, Transform segment2)
    {
        var Body = segment1.GetComponent<Rigidbody2D>();
        var joint = segment2.GetComponent<DistanceJoint2D>();

        joint.enableCollision = true;
        joint.connectedBody = Body;
    }

    public void cutJoints(Transform segment)
    {
        Destroy(segment.GetComponent<DistanceJoint2D>());
    }

    public void reconnectHook(Transform segment)
    {
        if (segment != null)
        {
            hookJoint = segment.GetComponent<DistanceJoint2D>();

            hookJoint.connectedBody = hookBody;
        }
    }

    public void disconnectHook()
    {
        Destroy(hookJoint);
    }

    public void setHook(Transform newHook)
    {
        hook = newHook;
        hookBody = hook.GetComponent<Rigidbody2D>();
        reconnectHook(segments[segmentIndex]);
    }

    public void addSegment()
    {
        int index = ++segmentIndex;
        var segment = segments[index] = createSegment();
        segment.GetComponent<RopePhysics>().setpositionInArray(index);

        reconnectHook(segment);

        if (segments[index - 1] != null)
        {
            connectWithJoints(segments[index], segments[index - 1]);
        }

    }


    public void removeLastSegment()
    {

        if (segments[segmentIndex] != null && Vector2.Distance(hook.position, segments[segmentIndex].position) <= hook.localScale.x + 0.08)
        {
            Destroy(segments[segmentIndex].gameObject);
            segmentIndex--;
            reconnectHook(segments[segmentIndex]);
        }
    }

    private void Awake()
    {
        segments = new Transform[1000];
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
