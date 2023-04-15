using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Player Movement is achieved by Applying Forces to a rigidbody, updates on "position" should be avoided


public class PlayerMovement : MonoBehaviour
{

    public enum State{
        Moving, Hanging, Floating
    }

    State activeState = State.Moving;

    [SerializeField]
    Transform mouseHitbox = null;

    [SerializeField]
    GameObject rope = null;

    GameObject RopeObject;

    [SerializeField,Range(0,1000)]
    float movementSpeed;

    [SerializeField,Range(0,1000)]
    float jumpHeight;

    [SerializeField, Range(0, 100)]
    float drag;


    Rigidbody2D body;
    RopeBehaviour ropeControll;


    // Start is called before the first frame update
    void Start()
    {

    }


    void Awake()
    {
        body = this.GetComponent<Rigidbody2D>();
        createNewRope();
       
    }


    void createNewRope()
    {
        RopeObject = Instantiate(rope);
        ropeControll = RopeObject.GetComponent<RopeBehaviour>();
        ropeControll.setHook(this.transform);
        ropeControll.setHand(mouseHitbox);
    }

    void changeHookTo(Transform newHook)
    {
        ropeControll.setHook(newHook);
    }

    void updateMouse()
    {
        mouseHitbox.position = this.transform.position + new Vector3(this.transform.localScale.x + 0.5f, 0, 0);
    }

    [SerializeField, Range(0, 10)]
    float accelerationTimer = 1;
    float accelerationRightCounter = 1;
    float accelerationLeftCounter = 1;

    [SerializeField, Range(0, 10)]
    float decellerateTimer = 1;
    float decellerateRightCounter = 1;
    float decellerateLeftCounter = 1;

    [SerializeField, Range(0, 1000)]
    float accelerationSpeed = 600;

    void walking()
    {

        if (Input.GetKeyDown("d"))
        {
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, 0, this.transform.rotation.z);
            mouseHitbox.position = new Vector3(mouseHitbox.position.x * -1, mouseHitbox.position.y, mouseHitbox.position.z);
            accelerationRightCounter = 0;
        }
        if (Input.GetKey("d"))
        {
            body.AddForce(Vector2.right * drag * body.velocity);
        }
        if (Input.GetKeyUp("d"))
        {
            body.AddForce(accelerationSpeed * Vector2.left);
        }


        if (Input.GetKeyDown("a"))
        {
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, 180, this.transform.rotation.z);
            mouseHitbox.position = new Vector3(mouseHitbox.position.x * -1, mouseHitbox.position.y, mouseHitbox.position.z);
            accelerationLeftCounter = 0;
        }

        if (Input.GetKey("a"))
        {
            body.AddForce(Vector2.left * drag * body.velocity);
        }
        if (Input.GetKeyUp("a"))
        {
            body.AddForce(accelerationSpeed * Vector2.right);
        }
    }


    void applyInputForces()
    {
        if (accelerationRightCounter < accelerationTimer)
        {
            body.AddForce(accelerationSpeed * Vector2.right);
        }
        if (accelerationLeftCounter < accelerationTimer)
        {
            body.AddForce(accelerationSpeed * Vector2.left);
        }

        if (decellerateRightCounter < decellerateTimer)
        {
            body.AddForce(accelerationSpeed * Vector2.left);
        }
        if (decellerateLeftCounter < decellerateTimer)
        {
            body.AddForce(accelerationSpeed * Vector2.right);
        }
    }

    void updatePlayerMovement()
    {
        body.AddForce(body.velocity * -drag);

        if (activeState == State.Moving)
        {
            walking();

            if (Input.GetKey("s"))
            {
                body.AddForce(movementSpeed * Vector2.down);
            }

            if (Input.GetKey("w"))
            {
                body.AddForce(jumpHeight * Vector2.up);
            }
        }

        applyInputForces();

        accelerationRightCounter += Time.deltaTime;
        decellerateRightCounter += Time.deltaTime;

        accelerationLeftCounter += Time.deltaTime;
        decellerateLeftCounter += Time.deltaTime;

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
        ropeUpdates();
        updateMouse();

    }
}
