using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePhysics : MonoBehaviour
{

    Transform player;
    MeshRenderer sprite;

    Transform hands;

    Collider2D hand;
    Rigidbody2D handbody;
    Rigidbody2D body;

    private int positionInArray;

    public void setHand(Transform newHand)
    {
        hands = newHand;
        hand = hands.GetComponent<Collider2D>();
        handbody = hands.GetComponent<Rigidbody2D>();

    }
    public void setpositionInArray(int i)
    {
        positionInArray = i;
    }

    public int getpositionInArray()
    {
        return positionInArray;
    }

    private void Awake()
    {
        body = this.GetComponent<Rigidbody2D>();
        sprite = this.GetComponent<MeshRenderer>();

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnMouseOver()
    {

    }

    private bool attached = false;
    DistanceJoint2D mouseJoint;

    // Update is called once per frame
    void Update()
    {
        if (body.IsTouching(hand) && Input.GetMouseButton(0))
        {
            Debug.Log(positionInArray);
            mouseJoint = this.gameObject.AddComponent<DistanceJoint2D>();
            mouseJoint.enableCollision = true;
            mouseJoint.autoConfigureDistance = false;
            mouseJoint.maxDistanceOnly = true;
            mouseJoint.connectedBody = handbody;
            attached = true;
        }

        if (Input.GetMouseButtonUp(0) && attached == true)
        {
            Destroy(mouseJoint);
            attached = false;
        }
    }
}
