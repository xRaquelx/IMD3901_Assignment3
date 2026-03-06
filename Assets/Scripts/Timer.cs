using TMPro;
using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI startTimerText;

    private float timer = 0f;

    public bool isTimerRunning = false;
    
    void Start()
    {
        startTimerText.gameObject.SetActive(true);
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            int mins = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            int milliseconds = Mathf.FloorToInt((timer * 1000f) % 1000f);
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", mins, seconds, milliseconds);
        }
    }

    IEnumerator StartCountdown()
    {        
        for (int i = 3; i > 0; i--)
        {
            startTimerText.text = i.ToString();
            yield return new WaitForSeconds(1f);
            if (i == 1)
            {
                isTimerRunning = true;
                startTimerText.gameObject.SetActive(false);
            }     
        }            
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }
}
