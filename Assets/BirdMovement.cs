using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public float speed = 25.0f;
    public float rotationSpeed = 90;
    public float force = 700f;

    Rigidbody rb;
    Transform t;
    bool inFlight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        inFlight = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Code derived from CS 426 Assignment One

        if (inFlight)
            rb.linearVelocity += this.transform.forward * speed * Time.deltaTime; //constant movement speed
        else
            rb.linearVelocity = this.transform.forward * 0; //placeholder for landing
        // Time.deltaTime represents the time that passed since the last frame
        //the multiplication below ensures that GameObject moves constant speed every frame
        if (Input.GetKey(KeyCode.W))
            inFlight = true; //placeholder for taking off, takes a moment to work fsr
            //rb.linearVelocity += this.transform.forward * speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            inFlight = false; //placeholder for landing
            //rb.linearVelocity -= this.transform.forward * speed * Time.deltaTime;

        // Quaternion returns a rotation that rotates x degrees around the x axis and so on
        if (Input.GetKey(KeyCode.D))
            t.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
        else if (Input.GetKey(KeyCode.A))
            t.rotation *= Quaternion.Euler(0, - rotationSpeed * Time.deltaTime, 0);
    }
}
