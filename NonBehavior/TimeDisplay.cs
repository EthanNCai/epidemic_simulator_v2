using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public TimeManager timeManager;
    private GameObject tmp;

    private int day = 0;
    private int hour = 0;
    void Start()
    {
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
        timeManager.OnDayChanged += (object sender, TimeManager.OnDayChangedEventArgs eventArgs) =>
        {
            day = eventArgs.newDay;
            tmp.text = "��" + day + "�� " + hour + ":00";
        };
        timeManager.OnHourChanged += (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            hour = eventArgs.newHour;
            tmp.text = "��" + day + "�� " + hour + ":00";
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
