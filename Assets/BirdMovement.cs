using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BirdMovement : MonoBehaviour
{
    public float speed = 25.0f;
    public float vertSpeed = 525.0f;
    public float rotationSpeed = 90;
    public float force = 700f;
    public Vector3 up = new Vector3(0f, 100f, 0f);
    public Transform Soldier;

    Rigidbody rb;
    Transform t;
    public Animator anim;
    bool inFlight;
    public AudioClip cry;
    public AudioClip flight;
    AudioSource audio;

    public TMP_Text scoretext;
    public static float score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        //anim = GetComponent<Animator>();
        inFlight = true;
        score = 0;
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
            anim.SetBool("Peck", false);
            rb.linearVelocity = this.transform.forward * speed * Time.deltaTime; //constant movement speed
            if(this.transform.position.y < 10.8 && rb.linearVelocity.y <= 0){
                rb.linearVelocity += this.transform.up * vertSpeed * Time.deltaTime; //move up if y is below cruising altitude
            } else if(rb.linearVelocity.y > 0){
                rb.linearVelocity = this.transform.forward * speed * Time.deltaTime;
            }
        } else{
            rb.linearVelocity = this.transform.forward * 0; //placeholder for landing
            if(this.transform.position.y > 0.3)
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

        anim.SetBool("Flight", (inFlight || this.transform.position.y > 3.2)); //does not initiate landing animation until near ground

        if(!anim.GetBool("Flight") && Input.GetKey(KeyCode.Q)){
            anim.SetBool("Peck", true);
        }

        if(anim.GetBool("Peck")){
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 5);
            for(int i = 0; i < hitColliders.Length; i++){
                GameObject obj = hitColliders[i].gameObject;
                Soldier_AI sol = obj.GetComponent<Soldier_AI>();
                if(sol != null && sol.IsDead && sol.edible){
                    score += 1;
                    scoretext.GetComponent<TMP_Text>().text = "Score: " + ((int)score).ToString();
                    sol.edible = false;
                    if(score >= 5.0){
                        SceneManager.LoadScene("Credits");
                    }
                }
            }
            //check if corpse is nearby, if so update score
        }

    }
}
