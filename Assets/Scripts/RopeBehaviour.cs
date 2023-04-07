using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//a rope consists of connected rigidbodys(segments), created from a prefab. The ropes last segment is connected to the "hook".
//This can be any rigidbody you call "reconnectHoock()" on


public class RopeBehaviour : MonoBehaviour
{
    [SerializeField]
    Transform ropeSegment = null;

    [SerializeField]
    Transform hook = null;

    Transform[] segments;

    Rigidbody2D hookBody;

    Joint2D hookJoint;


    private int segmentIndex = 0;

    private Transform createSegment()
    {

        Transform segment;

        segment = Instantiate(ropeSegment);
        segment.transform.position = hook.position;
        segment.name = "segment " + segmentIndex;

        return segment;
    }


    private void connectWithJoints(Transform segment1, Transform segment2)
    {
            Rigidbody2D parentBody = segment1.GetComponent<Rigidbody2D>();
            DistanceJoint2D joint = segment2.GetComponent<DistanceJoint2D>();

            joint.enableCollision = true;
            joint.connectedBody = parentBody;
    }

    private void reconnectHook(Transform segment)
    {
        if (segment != null)
        {
            hookJoint = segment.GetComponent<DistanceJoint2D>();

            hookJoint.connectedBody = hookBody;
        }
    }

    public void setHook(Rigidbody2D newHook)
    {
        hookBody = newHook;
        reconnectHook(segments[segmentIndex]);
    }


    private bool isTouchingHook()
    {
        if (segments[segmentIndex] != null)
        {
            CircleCollider2D segment = segments[segmentIndex].GetComponent<CircleCollider2D>();
            return hookBody.IsTouching(segment);
        }
        return false;
    }

    private void addSegment()
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


    private void removeSegment()
    {
        if (segments[segmentIndex] != null)
        {
            Rigidbody2D body = segments[segmentIndex].GetComponent<Rigidbody2D>();
            DistanceJoint2D joint = segments[segmentIndex].GetComponent<DistanceJoint2D>();

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
        hookBody = hook.GetComponent<Rigidbody2D>();
    }

    public float timer = 0.2f;
    public float createcounter = 0, deletecounter = 0;

    // Update is called once per frame
    void Update()
    {

        createcounter += Time.deltaTime;
        deletecounter += Time.deltaTime;

        if (Input.GetKey("l") && createcounter >= timer)
        {
            createcounter = 0;
            addSegment();
        }

        if (Input.GetKeyDown("k"))
        {
        }

        if (Input.GetKey("j") && deletecounter >= timer)
        {
                deletecounter = 0;
                removeSegment();
        }

        if (Input.GetMouseButton(0))
        {
         
        }
    }
}
