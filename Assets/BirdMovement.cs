using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public float speed = 25.0f;
    public float vertSpeed = 525.0f;
    public float rotationSpeed = 90;
    public float force = 700f;
    public Vector3 up = new Vector3(0f, 100f, 0f);

    Rigidbody rb;
    Transform t;
    public Animator anim;
    bool inFlight;
    public AudioClip cry;
    public AudioClip flight;
    AudioSource audio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        //anim = GetComponent<Animator>();
        inFlight = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Code derived from CS 426 Assignment One
        // Time.deltaTime represents the time that passed since the last frame
        //the multiplication below ensures that GameObject moves constant speed every frame
        if (Input.GetKey(KeyCode.W)) {
            if(inFlight == false){
                audio.clip = flight;
                audio.Play();
            }
            inFlight = true; //placeholder for taking off, takes a moment to work fsr
            //rb.linearVelocity += this.transform.forward * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S)) {
            inFlight = false; //placeholder for landing
            //rb.linearVelocity -= this.transform.forward * speed * Time.deltaTime;
        }
        if (inFlight){
            rb.linearVelocity = this.transform.forward * speed * Time.deltaTime; //constant movement speed
            if(this.transform.position.y < 10.8 && rb.linearVelocity.y <= 0){
                rb.linearVelocity += this.transform.up * vertSpeed * Time.deltaTime; //move up if y is below cruising altitude
            } else if(rb.linearVelocity.y > 0){
                rb.linearVelocity = this.transform.forward * speed * Time.deltaTime;
            }
        } else{
            rb.linearVelocity = this.transform.forward * 0; //placeholder for landing
            if(this.transform.position.y > -0.1)
                rb.linearVelocity = this.transform.up * vertSpeed * Time.deltaTime * (-1); //move down if y is above the ground
        }

        // Quaternion returns a rotation that rotates x degrees around the x axis and so on
        if (Input.GetKey(KeyCode.D)){
            t.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
            anim.SetBool("Left", false);
            anim.SetBool("Right", true);
        } else {
            if (Input.GetKey(KeyCode.A)){
                t.rotation *= Quaternion.Euler(0, - rotationSpeed * Time.deltaTime, 0);
                anim.SetBool("Left", true);
                anim.SetBool("Right", false);
            } else
                anim.SetBool("Left", false);
                anim.SetBool("Right", false);
                //t.rotation = Quaternion.Euler(0, t.rotation.y, 0);
        }

        if(Input.GetKey(KeyCode.E)){ //press e to cry - more functionality to manipulate soldier AI goes here later
            audio.clip = cry;
            audio.Play();
        }

        //todo - peck logic, if there is a corpse nearby and landed
        anim.SetBool("Flight", inFlight);

    }
}
