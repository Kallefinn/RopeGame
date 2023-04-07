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


    [SerializeField,Range(0,100)]
    float movementSpeed;

    [SerializeField,Range(0,100)]
    float jumpHeight;

    [SerializeField, Range(0, 100)]
    float drag;


    Rigidbody2D body, mouseBody;


    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody2D>();
        mouseBody = mouseHitbox.GetComponent<Rigidbody2D>();

    }

    
    void changeHookTo(Rigidbody2D newHook)
    {
        rope.GetComponent<RopeBehaviour>().setHook(newHook);
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
            changeHookTo(body);
        }
        if (Input.GetKeyDown("o"))
        {
            changeHookTo(mouseBody);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerMovement();
        updateMouse();

    }
}
