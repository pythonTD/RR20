using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Resolution : MonoBehaviour
{
    public InputField width;
    public InputField height;

    public int w = 1920;
    public int h = 1080;
    // Start is called before the first frame update
    void Start()
    {
        width.onEndEdit.AddListener(delegate { w = int.Parse(width.text.ToString()); });
        height.onEndEdit.AddListener(delegate { h = int.Parse(height.text.ToString()); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetResolution()
    {
        Screen.SetResolution(w, h, false);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
}
