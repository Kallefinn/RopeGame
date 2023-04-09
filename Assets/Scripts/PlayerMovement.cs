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


    [SerializeField,Range(0,100)]
    float movementSpeed;

    [SerializeField,Range(0,100)]
    float jumpHeight;

    [SerializeField, Range(0, 100)]
    float drag;


    Rigidbody2D body, mouseBody;
    RopeBehaviour ropeControll;


    // Start is called before the first frame update

    private void Awake()
    {
        body = this.GetComponent<Rigidbody2D>();
        mouseBody = mouseHitbox.GetComponent<Rigidbody2D>();
        RopeObject = Instantiate(rope);
        ropeControll = RopeObject.GetComponent<RopeBehaviour>();
        ropeControll.setHook(mouseHitbox);
    }



    void Start()
    {

    }

   

    void changeHookTo(Transform newHook)
    {
        RopeObject.GetComponent<RopeBehaviour>().setHook(newHook);
    }


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

        if (Input.GetKey("w"))
        {
            body.AddForce(jumpHeight * Vector2.up);
        }
    }

   void updateMouse()
    {
        mouseBody.AddForce(Input.mousePosition.normalized * mouseBody.mass);

        if (Input.GetKeyDown("p"))
        {
            changeHookTo(this.transform);
        }
        if (Input.GetKeyDown("o"))
        {
            changeHookTo(mouseHitbox);
        }
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
        }

        if (Input.GetKey("j") && deletecounter >= timer)
        {
            deletecounter = 0;
            ropeControll.removeLastSegment();
        }

        if (Input.GetMouseButton(0))
        {

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
