using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfectionStatusDisplay : MonoBehaviour
{
    public TimeManager timeManager;
    public PlacePeopleManager placePeopleManager;
    public PeopleInfectionManager peopleInfectionManager;
    private TextMeshProUGUI tmp;
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        timeManager.OnDayChanged += (object sender, TimeManager.OnDayChangedEventArgs eventArgs) =>
        {
            SendInfosToUI();
        };
        SendInfosToUI();
    }
    void Update()
    {
        
    }

    //private void SendInfosToUIInit()
    //{
    //    int stage1PeopleCounts = peopleInfectionManager.stage1InfectedPersons.Count;
    //    int stage2PeopleCounts = peopleInfectionManager.stage2InfectedPersons.Count;
    //    int peopleCounts = placePeopleManager.GetPeopleCounts();
    //    tmp.text =
    //    "未感染: " + (peopleCounts - 1).ToString() +
    //    "已痊愈: " + peopleInfectionManager.recoverdPeoples.Count.ToString() +
    //    "潜伏期: " + stage1PeopleCounts.ToString() +
    //    "发病期: " + stage2PeopleCounts.ToString();
    //}
    private void SendInfosToUI()
    {
        int stage1PeopleCounts = peopleInfectionManager.stage1InfectedPersons.Count;
        int stage2PeopleCounts = peopleInfectionManager.stage2InfectedPersons.Count;
        int recoverdPeopleCounts = peopleInfectionManager.recoverdPeoples.Count;
        int peopleCounts = placePeopleManager.GetPeopleCounts();
        tmp.text =
        "未感染: " + (peopleCounts - stage1PeopleCounts - stage2PeopleCounts - recoverdPeopleCounts).ToString() +
        "已痊愈: " + recoverdPeopleCounts.ToString() +
        "潜伏期: " + stage1PeopleCounts.ToString() +
        "发病期: " + stage2PeopleCounts.ToString();
    }
}
