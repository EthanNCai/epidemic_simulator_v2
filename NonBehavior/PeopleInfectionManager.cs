using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEngine.GraphicsBuffer;

public class PeopleInfectionManager : MonoBehaviour
{
    public PlacePeopleManager peopleManager;
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
    public TimeManager timeManager;
    private List<GameObject> infectedPeoples = new List<GameObject>();
    private List<GameObject> recoverdPeoples = new List<GameObject>();
    //private int currentHour = -1;

    private const int DEFACT_VOLUME_GATE = 80;

    // compute contact in every hour
    private void CalculateContacts()
    {
        for (int i = 0; i < peopleManager.persons.Count; i++)
        {
            GameObject personObj = peopleManager.persons[i];
            PersonBehavior personBehavior = personObj.GetComponent<PersonBehavior>();
            
            if (personBehavior.infection == null) { continue; }

            VolumeVisualizeNode tileVirusVolume = gridValuesAttachedBehavior.virusVolumeGridValuesManager.GetGridObj(personObj.transform.position);
            PathFindingNode pathFindingNode = gridValuesAttachedBehavior.pathFindingGVC.GetGridObj(personObj.transform.position);
            int volumeToSet = Mathf.Max(tileVirusVolume.virusVolume, personBehavior.infection.CheckVirusVolume());
            tileVirusVolume.SetVirusVolume(volumeToSet);
        }
    }

    public int CheckExposeVolumeHere(Vector3 PersonPosition)
    {
        return gridValuesAttachedBehavior.virusVolumeGridValuesManager.GetGridObj(PersonPosition).virusVolume;
    }
    public void someOneJustGotInfected(GameObject person)
    {
        if (recoverdPeoples.Contains(person)) { recoverdPeoples.Remove(person); }
        infectedPeoples.Add(person);
    }
    public void someOneJustRecoverd(GameObject person)
    {
        infectedPeoples.Remove(person);
        recoverdPeoples.Add(person);
    }


    void Start()
    {
        timeManager.OnShiftHourChanged += (object sender, TimeManager.OnShiftHourChangedEventArgs eventArgs) =>
        {
           
            CalculateContacts();

        };
    }

    void Update()
    {
        
    }
}
