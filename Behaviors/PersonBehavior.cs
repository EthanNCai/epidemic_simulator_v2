using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Profiling;

public class PersonBehavior : MonoBehaviour
{
    //private WaitForSeconds waitForSeconds = new WaitForSeconds(randomWait / 1000f);
    private GameObject personObj;
    private TimeManager timeManager;
    private GridValuesAttachedBehavior gridValuesAttachedBehavior;
    private PeopleInfectionManager personInfectionManager;
    Stack<UnityEngine.Vector3> pathStack;
    //UnityEngine.Vector3 dst = UnityEngine.Vector3.zero;
    Task movingTask = null;
    CancellationTokenSource moveToCancelToken;
    PathFinding pathFinding ;
    private static readonly object gridValuesContainerLock = new object();
    PlaceBehavior home;
    PlaceBehavior office;
    PlaceBehavior dstPlace;
    
    
    private UnityEngine.Vector3 currentPosition;
    private PlaceBehavior currentPlace = null;


    PathFindingNode startNode;
    private bool isInitialized =  false;


    //CONSTS
    //private const int TIME_TO_WORK = 3;
    //private const int TIME_TO_HOME = 16;
    private const float DECISION_DISTANCE = 0.1f;
    //private const int MAXIMUM_RANDOM_WAIT = 12;
    private const float UNINFECTED_INFECTION_PROB = 0.3f;
    private const float RECOVERD_INFECTION_PROB = 0.1f;

    private const int FIRST_TIME_TO_WORK = 5;
    private const int HOME_OFFICE_MIN_DURATION = 4;
    private const int LAST_TIME_TO_WORK = 12;
    private const int LAST_TIME_TO_HOME = 23;


    private float timeToWorkDynamic;
    private float timeToHomeDynamic;

    // infection related
    public Infection infection = null;
    public int maxExposedToday = 0;
    public InfectionStatus infectionStatus = InfectionStatus.UnInfected;
    private System.Random randomGenerator = new System.Random();
    public void init(PlaceBehavior home, PlaceBehavior office, GridValuesAttachedBehavior gridValuesAttachedBehavior, TimeManager timeManager, PeopleInfectionManager personInfectionManager, GameObject personObj,Infection infection)
    {
        this.timeManager = timeManager;
        this.home = home;
        this.office = office;
        this.gridValuesAttachedBehavior = gridValuesAttachedBehavior;
        this.personInfectionManager = personInfectionManager;
        this.personObj = personObj;
        this.infection = infection;
        this.currentPlace = home;

        transform.position = home.GenerateInPlacePosition();
        pathStack = new Stack<UnityEngine.Vector3>();
        pathFinding = new PathFinding(gridValuesAttachedBehavior.pathFindingGVC);
        //timeManager.OnHourChanged += (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        //{
        //    //await Scheduler(eventArgs);
        //    //StartCoroutine(CoroutineScheduler(eventArgs));
        //};
        timeManager.OnShiftHourChanged += (object sender,
        TimeManager.OnShiftHourChangedEventArgs eventArgs) =>
        {
            HourSummaryCalculation(eventArgs);
        };
        timeManager.OnDayChanged += (object sender, TimeManager.OnDayChangedEventArgs eventArgs) =>
        {
            DaySummaryCalculation(eventArgs);
        };
        isInitialized = true;

        // initial generation
        (timeToWorkDynamic, timeToHomeDynamic) = GetRandomWorkSchedule();
        
    }

    
    void  Update()
    {
        if (!isInitialized) { return ; }
        currentPosition = transform.position;
        startNode = gridValuesAttachedBehavior.pathFindingGVC.GetGridObj(currentPosition);

        PlainScheduler();
    }


    

    private async Task MoveTo(CancellationTokenSource moveToCancelToken)
    {
        float movingBaseSpeed = timeManager.GetDefaultMovingSpeed();
        float speed = movingBaseSpeed + movingBaseSpeed * (float)randomGenerator.NextDouble();
        currentPlace = null;
        while (true) {

            if (pathStack.Count == 0) {
                currentPlace = dstPlace;
                break;
            }
            UnityEngine.Vector3 currentTarget = pathStack.Pop();
            while (UnityEngine.Vector3.Distance(transform.position, currentTarget) > DECISION_DISTANCE)
            {
                UnityEngine.Vector3 direction = (currentTarget - transform.position).normalized;
                
                transform.position += direction * speed * Time.deltaTime;
                await Task.Yield();
                if (moveToCancelToken.IsCancellationRequested)
                {
                    return;
                }
            }

        }
    }
    private async Task FindPath()
    {

          await Task.Run(() =>
          {
              //Profiler.BeginSample("My Sample2");
              lock (gridValuesContainerLock)
              {
                  pathStack.Clear();
                  Vector3 inPlaceShift = dstPlace.GenerateInPlacePosition();
                  pathStack.Push(inPlaceShift);
                  Vector3 startPosition = currentPlace != null ? currentPlace.cellPosition : currentPosition;
                  pathFinding.FindPath(startPosition, dstPlace.cellPosition, pathStack);
                  
              }
              //Profiler.EndSample();
          }
         );
    }

    private Color GetDisplayColor()
    {
        InfectionStatus currentInfectionStatus= CheckStatus();
        switch (currentInfectionStatus)
        {
            case InfectionStatus.UnInfected: { return Color.white; }
            case InfectionStatus.Recovered: { return Color.green; }
            default: {
                    int virusVolume = infection.CheckVirusVolume();
                    virusVolume = Mathf.Clamp(virusVolume, 0, 100);
                    float redIntensity = virusVolume / 100f;
                    Color color = new Color(1f, 1f - redIntensity, 1f - redIntensity, 0.5f);
                    return color;
                }
        }
    }

    private async void GoToPlace(PlaceBehavior placeToGo)
    {
        dstPlace = placeToGo;
        await FindPath();
        moveToCancelToken?.Cancel();
        moveToCancelToken = new CancellationTokenSource();
        if (pathStack.Count != 0) {movingTask = MoveTo(moveToCancelToken);}
    }

    public bool RollTheDice(float trueProb)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue < trueProb)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetInfectionProb(int volumExposed)
    {
        // volumExposed -> 0~100 integer
        if (infectionStatus == InfectionStatus.UnInfected)
        {
            return UNINFECTED_INFECTION_PROB * (volumExposed * 0.01f);
        }
        else if (infectionStatus == InfectionStatus.Recovered)
        {
            return RECOVERD_INFECTION_PROB * (volumExposed * 0.01f);
        }
        else
        {
            Debug.LogError("this guy shouldn't be checking this value");
            return -1f;
        }
    }
    public InfectionStatus CheckStatus()
    {
        if (infection == null)
        {
            return infectionStatus;
        }
        else
        {
            return infection.CheckStatus();
        }

    }
    //async Task AsyncScheduler(TimeManager.OnHourChangedEventArgs eventArgs)
    //{
    //    // Deprecated due to performance issue
    //    int max = (int)(timeManager.hourDuration * MAXIMUM_RANDOM_WAIT * 1000);
    //    int randomWait = UnityEngine.Random.Range(0, max);
    //    await Task.Delay(randomWait);

    //    switch (eventArgs.newHour)
    //    {
    //        case TIME_TO_WORK: GoToPlace(office); return;
    //        case TIME_TO_HOME: GoToPlace(home); return;
    //        default: return;
    //    }
    //}

    //IEnumerator CoroutineScheduler(TimeManager.OnHourChangedEventArgs eventArgs)
    //{
    //    //int max = (int)(timeManager.hourDuration * MAXIMUM_RANDOM_WAIT * 1000);
    //    //int randomWait = UnityEngine.Random.Range(0, max);
    //    yield return new WaitForSeconds(1 / 1000f);

    //    switch (eventArgs.newHour)
    //    {
    //        case TIME_TO_WORK: GoToPlace(office); break;
    //        case TIME_TO_HOME: GoToPlace(home); break;
    //        default: break;
    //    }
    //}


    private (float,float) GetRandomWorkSchedule()
    {
        float timeToWork = UnityEngine.Random.Range(
                FIRST_TIME_TO_WORK, LAST_TIME_TO_WORK
                );
        float timeToHome =  UnityEngine.Random.Range(
                timeToWork + HOME_OFFICE_MIN_DURATION, LAST_TIME_TO_HOME
                );

        return (timeToWork, timeToHome);
    }

    void PlainScheduler()
    {
        float currentTime = timeManager.GetTime24();
        if (currentTime > timeToWorkDynamic)
        {
            timeToWorkDynamic = float.MaxValue;
            GoToPlace(office);
        }
        else if(currentTime > timeToHomeDynamic)
        {
            timeToHomeDynamic = float.MaxValue;
            GoToPlace(home);
        }

    }

    public void HourSummaryCalculation(TimeManager.OnShiftHourChangedEventArgs eventArgs)
    {
        if (infection == null)
        {
            int exposedVolume = personInfectionManager.CheckExposeVolumeHere(currentPosition);
            if (exposedVolume > maxExposedToday)
            {
                maxExposedToday = exposedVolume;
            }
        }
    }
    public void DaySummaryCalculation(TimeManager.OnDayChangedEventArgs eventArgs)
    {
        (timeToWorkDynamic,timeToHomeDynamic) = GetRandomWorkSchedule();
        GetComponent<SpriteRenderer>().color = GetDisplayColor();

        if (infection != null)
        {
            infection?.Progress();
            infectionStatus = CheckStatus();
            if (infectionStatus == InfectionStatus.Recovered)
            {
                personInfectionManager.someOneJustRecoverd(personObj);
                infection = null;
                return;
            }
        }
        if (infection != null) { return; }

        float infectionProb = GetInfectionProb(maxExposedToday);
        bool isInfected = RollTheDice(infectionProb);

        if (isInfected)
        {
            this.infection = new Infection();
            personInfectionManager.someOneJustGotInfected(personObj);
        }
        this.maxExposedToday = 0;
    }
}
