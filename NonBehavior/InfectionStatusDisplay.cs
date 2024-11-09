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
    //    "δ��Ⱦ: " + (peopleCounts - 1).ToString() +
    //    "��Ȭ��: " + peopleInfectionManager.recoverdPeoples.Count.ToString() +
    //    "Ǳ����: " + stage1PeopleCounts.ToString() +
    //    "������: " + stage2PeopleCounts.ToString();
    //}
    private void SendInfosToUI()
    {
        int stage1PeopleCounts = peopleInfectionManager.stage1InfectedPersons.Count;
        int stage2PeopleCounts = peopleInfectionManager.stage2InfectedPersons.Count;
        int recoverdPeopleCounts = peopleInfectionManager.recoverdPeoples.Count;
        int peopleCounts = placePeopleManager.GetPeopleCounts();
        tmp.text =
        "δ��Ⱦ: " + (peopleCounts - stage1PeopleCounts - stage2PeopleCounts - recoverdPeopleCounts).ToString() +
        "��Ȭ��: " + recoverdPeopleCounts.ToString() +
        "Ǳ����: " + stage1PeopleCounts.ToString() +
        "������: " + stage2PeopleCounts.ToString();
    }
}
