using UnityEngine;
using TMPro;

public class ControlTutorial : MonoBehaviour
{
    public TMP_Text tutorial;
    public float cooldown = 0.5f;
    float allowTime = 0.0f; //time in seconds where it is allowable to press C
    bool active;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //tutorial = GetComponent<TMP_Text>();
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.C) && allowTime < Time.time){
            active = !active;
            tutorial.gameObject.SetActive(active);
            allowTime = Time.time + cooldown;
        }
    }
}
