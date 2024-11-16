using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public enum SocialStatus
{
    Homing,
    Moving,
    Working,
    Shopping,
    Isolated,
    InClinic,
    Testing,
    BLANK,
}

public class PersonBehavior : MonoBehaviour
{
    private int wage = 1;
    private float aRandomTimeBeforeNoon;
    private RevenueManager revenueManager;
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

    private List<PlaceBehavior> clinics;
    private PlaceBehavior home;
    private PlaceBehavior office;
    private PlaceBehavior dstPlace;
    
    private UnityEngine.Vector3 currentPosition;
    private PlaceBehavior currentPlace = null;


    PathFindingNode startNode;



    private bool isInitialized =  false;
    public string _name;
    static string[] givenNames = new string[] { "蔡", "韦", "李", "翁", "郑", "王" };
    static string[] firstNames = new string[] { "俊志", "永佳", "雪婷", "挺", "宇涛", "宇", "心怡" };
    private const float DECISION_DISTANCE = 0.1f;
    private const float UNINFECTED_INFECTION_PROB = 0.3f;
    private const float RECOVERD_INFECTION_PROB = 0.1f;

    private const int FIRST_TIME_TO_WORK = 5;
    private const int HOME_OFFICE_MIN_DURATION = 4;
    private const int LAST_TIME_TO_WORK = 12;
    private const int LAST_TIME_TO_HOME = 23;
    private const int SEVERE_VIRUS_VOLUME = 80;
    private const int OUT_OF_HOSPITAL_LIMIT = 30;

    // commuting related
    public SocialStatus socialStatus;
    //public PlaceType dstType;
    private float timeToWorkDynamic;
    private float timeToHomeDynamic;

    // infection related
    private bool isSevereInfection = false;
    private PlaceBehavior registeredClinic = null;
    public Infection infection = null;
    public int maxExposedToday = 0;
    public InfectionStatus infectionStatus = InfectionStatus.UnInfected;
    private System.Random randomGenerator = new System.Random();
    public void init(PlaceBehavior home, PlaceBehavior office, GridValuesAttachedBehavior gridValuesAttachedBehavior, TimeManager timeManager, PeopleInfectionManager personInfectionManager, GameObject personObj,Infection infection,
        RevenueManager revenueManager, List<PlaceBehavior> clinics)
    {
        this.clinics = clinics;
        this.revenueManager = revenueManager;
        this._name = NameGenerator();
        this.timeManager = timeManager;
        this.home = home;
        this.office = office;
        this.gridValuesAttachedBehavior = gridValuesAttachedBehavior;
        this.personInfectionManager = personInfectionManager;
        this.personObj = personObj;
        this.infection = infection;
        this.currentPlace = home;

        transform.position = home.GenerateInPlacePosition();
        pathStack = new Stack<Vector3>();
        pathFinding = new PathFinding(gridValuesAttachedBehavior.pathFindingGVC);
 
        timeManager.OnShiftHourChanged += (object sender,
        TimeManager.OnShiftHourChangedEventArgs eventArgs) =>
        {
            HourSummaryCalculation(eventArgs);
        };
        timeManager.OnDayChanged += (object sender, TimeManager.OnDayChangedEventArgs eventArgs) =>
        {
            DaySummaryCalculation(eventArgs);
        };

        PlacePeopleManager.OnBuildConfirm += (object sender, PlacePeopleManager.OnBuildConfrimEventArg eventArgs) =>
        {
            switch (eventArgs.newPlaceBehavior.placeType)
            {
                case PlaceType.Clinic:
                {
                    clinics = eventArgs.updatedPlaceBehaviorList;
                    break;
                }
            }
            
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

    static public string[] SocialStatusTexts = new string[] { "正前往", "在家", "工作中", "住院中" };
    static public string GetSocialStatusDescriber(PersonBehavior personBehavior)
    {
        SocialStatus socialStatus = personBehavior.socialStatus;

        switch (socialStatus)
        {
            case SocialStatus.Moving: {
                    return SocialStatusTexts[0] + PlacePrototype.GetPlaceTypeDescriber(personBehavior.dstPlace.placeType); }
            case SocialStatus.Homing: { 
                    return SocialStatusTexts[1]; }
            case SocialStatus.Working: { 
                    return SocialStatusTexts[2]; }
            case SocialStatus.InClinic:
                {
                    return SocialStatusTexts[3];
                }
            default: { return "错误"; }
        }
    }

    private async Task MoveTo(CancellationTokenSource moveToCancelToken, SocialStatus delaredVSocialStatusDst)
    {
        float movingBaseSpeed = timeManager.GetDefaultMovingSpeed();
        float speed = movingBaseSpeed + movingBaseSpeed * (float)randomGenerator.NextDouble();
        currentPlace = null;
        while (true) {

            if (pathStack.Count == 0) {
                // arrived
                currentPlace = dstPlace;
                // switch socialStatus
                if (delaredVSocialStatusDst != SocialStatus.BLANK) 
                { 
                    socialStatus = delaredVSocialStatusDst;
                } else
                {
                    socialStatus = dstPlace.vSocialStatusDst;
                }
                // handle appointment (Only Medical Facilities need this action)
                if(dstPlace.placeType == PlaceType.Clinic)
                {
                    dstPlace.personEntered(this);
                }
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

    private async void GoToPlace(PlaceBehavior placeToGo, SocialStatus delaredVSocialStatusDst = SocialStatus.BLANK)
    {
        // if he's already here
        if(placeToGo == currentPlace){ return; }

        // set Status
        socialStatus = SocialStatus.Moving;

        // let's go ~
        dstPlace = placeToGo;
        await FindPath();
        moveToCancelToken?.Cancel();
        moveToCancelToken = new CancellationTokenSource();
        if (pathStack.Count != 0) {movingTask = MoveTo(moveToCancelToken, delaredVSocialStatusDst);}
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
            return UNINFECTED_INFECTION_PROB * (volumExposed * 0.1f);
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

    private string NameGenerator()
    {
        return givenNames[randomGenerator.Next(givenNames.Length)] +
            firstNames[randomGenerator.Next(firstNames.Length)];
    }
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

        if (currentTime > aRandomTimeBeforeNoon)
        {
            aRandomTimeBeforeNoon = float.MaxValue;
            // *** Possibility B - > if in medical facilities, but cured ***
            if (currentPlace!= null && currentPlace.placeType == PlaceType.Clinic)
            {
                if (infection == null)
                {
                    registeredClinic = null; 
                    currentPlace.personExited(this);
                }
                return;
            // ***********
            }
            else if (isSevereInfection)
            // *** Possibility A - > if severe and not in a Medical Facility, then seek for help ***
            {
                //if not in a clinic check clinic avaliability
                PlaceBehavior targetClinic = null;
                for (int i = 0; i < clinics.Count; i++)
                {
                    if (clinics[i].CheckIsAvaliable())
                    {
                        targetClinic = clinics[i];
                    }
                }
                if (targetClinic == null)
                {
                    // no clinic avaliable
                    //GoToPlace(home);
                    Debug.Log("A sim failed to find a available medical facility");
                }
                else
                {
                    targetClinic.personAppointed(this);
                    registeredClinic = targetClinic;
                    GoToPlace(targetClinic);
                }
                return;

            }
            // ***********
        }

        if (isSevereInfection || registeredClinic != null) { return; }

        // *** Possibility C - > if normal, consider go to work ***
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
        // ***********
    }

    public void HourSummaryCalculation(TimeManager.OnShiftHourChangedEventArgs eventArgs)
    {
        if (currentPlace?.placeType == PlaceType.Office)
        {
            revenueManager.Gain(wage);
        }

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
        
        aRandomTimeBeforeNoon = (float)randomGenerator.Next(11) + (float)randomGenerator.NextDouble();
        (timeToWorkDynamic,timeToHomeDynamic) = GetRandomWorkSchedule();
        GetComponent<SpriteRenderer>().color = GetDisplayColor();

        // BLOCK A : We Handle cases if you are already got infected
        if (infection != null)
        {
            bool isStageSwitch = infection.ProgressAndReturnIsNextStage();
            isSevereInfection = infection.CheckVirusVolume() > 80;
            infectionStatus = CheckStatus();
          
            if (infectionStatus == InfectionStatus.Recovered)
            {
                personInfectionManager.someOneJustRecoverd(personObj);
                infection = null;
                return;
            }else if (isStageSwitch)
            {
                personInfectionManager.
                    someOneJustProgressHisInfection(personObj);
                return;
            }
            return;
        }
        

        // all infected persons should already got returned
        Assert.IsTrue(infection==null);
        // BLOCK B : We Handle cases if you are a clean, uninfected one.

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
