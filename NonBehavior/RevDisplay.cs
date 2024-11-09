using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RevDisplay : MonoBehaviour
{
    public TimeManager timeManager;
    public RevenueManager revenueManager;
    private static string currencyTag = "HK$"; 
    // Start is called before the first frame update
    TextMeshProUGUI TM;
    void Start()
    {
        TextMeshProUGUI TM = GetComponent<TextMeshProUGUI>();
        timeManager.OnShiftHourChanged += (object sender,
        TimeManager.OnShiftHourChangedEventArgs eventArgs) =>
        {
            TM.text = currencyTag + revenueManager.GetRevenue().ToString();
        };
    }
    
    
    void Update()
    {
        
    }
}
