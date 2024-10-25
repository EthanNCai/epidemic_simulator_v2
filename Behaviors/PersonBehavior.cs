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
    Place home;
    Place office;
    Place dstPlace;
    
    
    private UnityEngine.Vector3 currentPosition;
    private Place currentPlace = null;


    PathFindingNode startNode;
    private bool isInitialized =  false;


    //CONSTS
    private const int TIME_TO_WORK = 4;
    private const int TIME_TO_HOME = 15;
    private const float DECISION_DISTANCE = 0.1f;
    private const int MAXIMUM_RANDOM_WAIT = 6;
    private const float UNINFECTED_INFECTION_PROB = 1.0f;
    private const float RECOVERD_INFECTION_PROB = 0.0f;

    // infection related
    public Infection infection = null;
    public int maxExposedToday = 0;
    public InfectionStatus infectionStatus = InfectionStatus.UnInfected;

    public void init(Place home, Place office, GridValuesAttachedBehavior gridValuesAttachedBehavior, TimeManager timeManager, PeopleInfectionManager personInfectionManager, GameObject personObj,Infection infection)
    {
        this.timeManager = timeManager;
        this.home = home;
        this.office = office;
        this.gridValuesAttachedBehavior = gridValuesAttachedBehavior;
        this.personInfectionManager = personInfectionManager;
        this.personObj = personObj;
        this.infection = infection;

        transform.position = home.GenerateInPlacePosition();
        pathStack = new Stack<UnityEngine.Vector3>();
        pathFinding = new PathFinding(gridValuesAttachedBehavior.pathFindingGridValuesManager);
        timeManager.OnHourChanged += async (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            await Scheduler(eventArgs);
        };
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
    }

    
    void  Update()
    {
        if (!isInitialized) { return ; }
        currentPosition = transform.position;
        startNode = gridValuesAttachedBehavior.pathFindingGridValuesManager.GetGridObj(currentPosition);
    }
    private async Task MoveTo(CancellationTokenSource moveToCancelToken)
    {
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
                float movingSpeed = timeManager.GetCorrespondingMovingSpeed();
                transform.position += direction * movingSpeed * Time.deltaTime;
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
                  if (currentPlace != null)
                  {
                      pathFinding.FindPath(currentPlace.cellPosition, dstPlace.cellPosition, pathStack);
                  }
                  else
                  {
                      pathFinding.FindPath(currentPosition, dstPlace.cellPosition, pathStack);
                  }
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

    private async void GoToPlace(Place place)
    {
        dstPlace = place;
        //Debug.Log(currentPlace?.ToString());
        await FindPath();
        //Debug.Log("Path Found!");
        moveToCancelToken?.Cancel();
        moveToCancelToken = new CancellationTokenSource();
        //Debug.Log("Move To Set Out!");
        movingTask = MoveTo(moveToCancelToken);
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
    async Task Scheduler(TimeManager.OnHourChangedEventArgs eventArgs)
    {
        //Profiler.BeginSample("My Sample2");
        int max = (int)(timeManager.hourDuration * MAXIMUM_RANDOM_WAIT * 1000);
        int randomWait = UnityEngine.Random.Range(0, max);
        await Task.Delay(randomWait);

        switch (eventArgs.newHour)
        {
            case TIME_TO_WORK: GoToPlace(office); return;
            case TIME_TO_HOME: GoToPlace(home); return;
            default: return;
        }
        //Profiler.EndSample();
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
