using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= 5){
            SceneManager.LoadScene("SampleScene");
            this.GetComponent<GameObject>().SetActive(false);
        }
    }
}
