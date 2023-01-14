using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    int spawnInterval = 100;
    public float timer;
    private new Rigidbody2D rigidbody2D;
    float timerMultiplier = 20;
    float scoreMultiplier = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        this.GetComponent<Rigidbody2D>().AddForce(transform.up * timer);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSystem.instance.isPaused)
        {
            rigidbody2D.Sleep();
        }
        else
        {
            rigidbody2D.WakeUp();
        }

        if (rigidbody2D.velocity.magnitude == 0 && !GameSystem.instance.isPaused)
        {
            timer -= timerMultiplier * Time.deltaTime;
        }

        this.gameObject.transform.GetComponent<Renderer>().material.color = new Color(timer / 100, timer / 100, 1.0f);

        if (timer < 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            GameSystem.instance.updateScore(timer * scoreMultiplier);
            coll.gameObject.GetComponent<PlayerController>().GetComponent<Rigidbody2D>().AddForce(transform.up * timer);

            Destroy(this.gameObject);
        }
    }
}
