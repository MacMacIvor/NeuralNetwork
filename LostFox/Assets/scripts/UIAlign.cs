using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAlign : MonoBehaviour
{
    public Canvas canvas;
    public RawImage[] foxHealth;
    public TMP_Text timerString;



    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        for (int i = 0; i < foxHealth.Length; i++)
        {
            foxHealth[i].transform.localPosition = new Vector3(-canvas.GetComponent<RectTransform>().rect.width / 2 + 85 + 100 * i, canvas.GetComponent<RectTransform>().rect.height / 2 - 50, 0);
            //85162
        }
        timerString.transform.localPosition = new Vector3(canvas.GetComponent<RectTransform>().rect.width / 2 - 162, canvas.GetComponent<RectTransform>().rect.height / 2 - 40, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
