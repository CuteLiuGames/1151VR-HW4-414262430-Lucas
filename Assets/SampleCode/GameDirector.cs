using UnityEngine;
using TMPro;

public class GameDirector : MonoBehaviour
{
    GameObject car;
    GameObject flag;
    GameObject distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.car = GameObject.Find("car");
        this.flag = GameObject.Find("flag");
        this.distance = GameObject.Find("Distance");
    }

    // Update is called once per frame
    void Update()
    {
        float length = this.flag.transform.position.x - this.car.transform.position.x;
        if (length > 0f)
        {
            distance.GetComponent<TextMeshProUGUI>().text = "離目標還有 " + length.ToString("F2") + "m";
        }
        else
        {
            distance.GetComponent<TextMeshProUGUI>().text = "抵達目標！";
        }
    }
}
