using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Player Movement is achieved by Applying Forces to a rigidbody, updates on "position" should be avoided


public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    Transform mouseHitbox = null;

    [SerializeField]
    GameObject rope = null;

    [SerializeField]
    GameObject RopeObject;


    [SerializeField,Range(0,1000)]
    float movementSpeed;

    [SerializeField,Range(0,1000)]
    float jumpHeight;

    [SerializeField, Range(0, 100)]
    float drag;


    Rigidbody2D body, mouseBody;
    RopeBehaviour ropeControll;


    // Start is called before the first frame update
    void Start()
    {

    }


    void Awake()
    {
        body = this.GetComponent<Rigidbody2D>();
        mouseBody = mouseHitbox.GetComponent<Rigidbody2D>();
        createNewRope();
    }


    void createNewRope()
    {
        RopeObject = Instantiate(rope);
        ropeControll = RopeObject.GetComponent<RopeBehaviour>();
        ropeControll.setHook(mouseHitbox);
    }

    void changeHookTo(Transform newHook)
    {
        ropeControll.setHook(newHook);
    }


    bool jumping = false;

    [SerializeField,Range(0,10)]
    float jumpingtimer = 2;
    
    float jumptimer = 0;

    void updatePlayerMovement()
    {
        body.AddForce(body.velocity * -drag);

        if (Input.GetKey("d"))
        {
            body.AddForce(movementSpeed * Vector2.right);
        }

        if (Input.GetKey("a"))
        {
            body.AddForce(movementSpeed * Vector2.left);
        }

        if (Input.GetKey("s"))
        {
            body.AddForce(movementSpeed * Vector2.down);
        }

        if (Input.GetKeyDown("w")&& jumping == false)
        {
            jumping = true;
        }

        if (jumptimer > jumpingtimer)
        {
            jumping = false;
            jumptimer = 0;
        }

        if (jumping)
        {
            body.AddForce(jumpHeight * Vector2.up);
        }

        jumptimer += Time.deltaTime;
    }

   void updateMouse()
    {
        mouseBody.AddForce(Input.mousePosition.normalized * mouseBody.mass * 2);

    }

    public float timer = 0.2f;
    public float createcounter = 0, deletecounter = 0;

    void ropeUpdates()
    {
        createcounter += Time.deltaTime;
        deletecounter += Time.deltaTime;

        if (Input.GetKey("l") && createcounter >= timer)
        {
            createcounter = 0;
            ropeControll.addSegment();
        }

        if (Input.GetKeyDown("k"))
        {
            ropeControll.disconnectHook();
            createNewRope();
        }

        if (Input.GetKey("j") && deletecounter >= timer)
        {
            deletecounter = 0;
            ropeControll.removeLastSegment();
        }

        if (Input.GetKeyDown("p"))
        {
            changeHookTo(this.transform);
        }
        if (Input.GetKeyDown("o"))
        {
            changeHookTo(mouseHitbox);
        }

        if(deletecounter > 100) 
        {
            deletecounter = 0;
        }
        if(createcounter > 100)
        {
            createcounter = 0;
        }


    }


    // Update is called once per frame
    void Update()
    {
        updatePlayerMovement();
        updateMouse();
        ropeUpdates();

    }
}
