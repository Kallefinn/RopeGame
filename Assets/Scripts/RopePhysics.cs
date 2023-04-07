using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePhysics : MonoBehaviour
{

    Transform player;
    SpriteRenderer sprite;


    private int positionInArray;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
    }

    void OnMouseOver()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
