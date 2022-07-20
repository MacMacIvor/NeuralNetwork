using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class UIScript : MonoBehaviour
{
    [Range(0,4)]
    public int typeOfUI = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0)) && Input.mousePosition.x > transform.position.x - GetComponent<RectTransform>().rect.width / 2 && Input.mousePosition.x < transform.position.x + GetComponent<RectTransform>().rect.width / 2 && Input.mousePosition.y < transform.position.y + GetComponent<RectTransform>().rect.height / 2 && Input.mousePosition.y > transform.position.y - GetComponent<RectTransform>().rect.height / 2)
        {
            
            switch (typeOfUI)
            {
                case 0:
                    SceneManager.LoadScene("1");
                    break;
                case 1:
                    SceneManager.LoadScene("Controls");
                    break;
                case 2:
                    SceneManager.LoadScene("Credits");
                    break;
                case 3:
                    Application.Quit();
                    break;
                case 4:
                    SceneManager.LoadScene("TitleScreen");
                    break;
            }
        }
    }
}
