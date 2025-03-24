using UnityEngine;
using UnityEngine.SceneManagement;

public class NonGameScene : MonoBehaviour
{
    private float started;
    private float wait = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        started = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - started > wait)
        {
            if(Input.anyKey) {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
}
