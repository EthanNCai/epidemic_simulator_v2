using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PersonBehavior : MonoBehaviour
{
    public TimeManager timeManager;
    public GridValuesAttachedBehavior gridValuesAttachedBehavior;
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



    public void init(Place home, Place office, GridValuesAttachedBehavior gridValuesAttachedBehavior, TimeManager timeManager)
    {
        this.timeManager = timeManager;
        this.home = home;
        this.office = office;
        this.gridValuesAttachedBehavior = gridValuesAttachedBehavior;
        transform.position = home.GenerateInPlacePosition();
        pathStack = new Stack<UnityEngine.Vector3>();
        pathFinding = new PathFinding(gridValuesAttachedBehavior.pathFindingGridValuesManager);
        timeManager.OnHourChanged += async (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            await Scheduler(eventArgs);
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
                transform.position += direction * 5f * Time.deltaTime;
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
          }
         );
    }
    async Task Scheduler(TimeManager.OnHourChangedEventArgs eventArgs)
    {
        int max = (int)(timeManager.hourDuration * 4 * 1000);
        int randomWait = UnityEngine.Random.Range(0, max);
        await Task.Delay(randomWait);

        switch (eventArgs.newHour)
        {
            case TIME_TO_WORK: GoToPlace(office); return;
            case TIME_TO_HOME: GoToPlace(home); return;
            default: return;
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

    

}
