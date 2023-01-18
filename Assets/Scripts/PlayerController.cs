using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PointerController pointer;
    public ShipEngineController engineLeft;
    public ShipEngineController engineRight;
    
    public bool isUncontrollable = false;
    private new Rigidbody2D rigidbody2D;
    private new Transform transform;
    //[HideInInspector] public Animator anim;
    private float movementSpeed = 12.0f;
    private float maxSpeed = 20f;//Replace with your max speed
    private Vector3 pos;

    private bool touchLeft = false;
    private bool touchRight = false;

    [HideInInspector]public float fuelMax = 100.0f;
    [HideInInspector]public float fuel = 100.0f;
    private float fuelWaste = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        transform = this.GetComponent<Transform>();
        //anim = this.GetComponent<Animator>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameSystem.instance.isPaused)
        {
            rigidbody2D.Sleep();
        }
        else
        {
            rigidbody2D.WakeUp();
        }

        if (fuel > fuelMax)
        {
            fuel = fuelMax;
        }
        pos = Camera.main.WorldToViewportPoint(this.transform.position);

        if (pos.x < 0.0f)
        {
            this.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1, pos.y, pos.z));
        }

        if (pos.x > 1.0f)
        {
            this.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0, pos.y, pos.z));
        }
        if (1.0f < pos.y)
        {
            rigidbody2D.velocity = new Vector2(pos.x, 0);
            this.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(pos.x, 1.0f, pos.z));
        }
        else
        {
            pointer.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (!isUncontrollable && fuel > 0)
        {
            engineRight.GetComponent<SpriteRenderer>().enabled = false;
            engineLeft.GetComponent<SpriteRenderer>().enabled = false;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                moveLeft();

                engineRight.GetComponent<SpriteRenderer>().enabled = true;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                moveRight();

                engineLeft.GetComponent<SpriteRenderer>().enabled = true;
            }

            touchLeft = false;
            touchRight = false;
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).position.x < Screen.width / 2)
                    {
                        touchLeft = true;
                    }
                    if (Input.GetTouch(i).position.x > Screen.width / 2)
                    {
                        touchRight = true;
                    }
                }

                if (touchLeft)
                {
                    moveLeft();
                    engineRight.GetComponent<SpriteRenderer>().enabled = true;
                }
                
                if (touchRight)
                {
                    moveRight();
                    engineLeft.GetComponent<SpriteRenderer>().enabled = true;
                }
            }

            if (fuel < 0)
            {
                isUncontrollable = true;

                fuel = 0;
            }

            GameSystem.instance.updateFuel(fuel);
        }
        else
        {
            engineLeft.GetComponent<SpriteRenderer>().enabled = false;
            engineRight.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (rigidbody2D.velocity.magnitude > maxSpeed)
        {
            rigidbody2D.velocity = rigidbody2D.velocity.normalized * maxSpeed;
        }
    }
    void moveLeft()
    {
        fuel -= Time.deltaTime * fuelWaste;

        rigidbody2D.AddForce(transform.up * movementSpeed);
        rigidbody2D.AddForce(-transform.right * movementSpeed / 2);
    }
    void moveRight()
    {
        fuel -= Time.deltaTime * fuelWaste;

        rigidbody2D.AddForce(transform.up * movementSpeed);
        rigidbody2D.AddForce(transform.right * movementSpeed / 2);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Floor")
        {
            isUncontrollable = true;

            this.gameObject.transform.GetComponent<Renderer>().material.color = Color.red;

            GameSystem.instance.Finish();
        }
    }
}
