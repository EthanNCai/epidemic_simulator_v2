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
    Stack<Vector3> pathStack;
    Vector3 dst = Vector3.zero;
    Task movingTask = null;
    CancellationTokenSource moveToCancelToken;
    PathFinding pathFinding ;
    private static readonly object gridValuesContainerLock = new object();
    Vector3 home = new Vector3(3, 3);
    Vector3 office = new Vector3(8,8);
    private static int TIME_TO_WORK = 4;
    private static int TIME_TO_HOME = 15;
    private Vector3 currentPosition;
    PathFindingNode startNode;

    void Start()
    {
        transform.position = home;
        pathStack = new Stack<Vector3>();
        pathFinding = new PathFinding(gridValuesAttachedBehavior.pathFindingGridValuesManager);

        timeManager.OnHourChanged += async (object sender, TimeManager.OnHourChangedEventArgs eventArgs) =>
        {
            await Scheduler(eventArgs);
        };
    }
    void  Update()
    {
        currentPosition = transform.position;
        startNode = gridValuesAttachedBehavior.pathFindingGridValuesManager.GetGridObj(currentPosition);
    }
    private async Task MoveTo(CancellationTokenSource moveToCancelToken)
    {
        while (true) {

            if (pathStack.Count == 0) {
                break;
            }
            Vector3 currentTarget = pathStack.Pop();
            while (Vector3.Distance(transform.position, currentTarget) > 0.3f)
            {
                Vector3 direction = (currentTarget - transform.position).normalized;
                transform.position += direction * 2f * Time.deltaTime;
                await Task.Yield();
                if (moveToCancelToken.IsCancellationRequested)
                {
                    return;
                }
            }

        }
    }
    private async Task FindPath(Vector3 dst)
    {
          await Task.Run(() =>
          {
              lock (gridValuesContainerLock)
              { 
                  pathFinding.FindPath(startNode, gridValuesAttachedBehavior.pathFindingGridValuesManager.GetGridObj(dst), pathStack); 
              }
          }
         );
    }
    async Task Scheduler(TimeManager.OnHourChangedEventArgs eventArgs)
    {
        int max = (int)(timeManager.hourDuration * 1 * 1000);
        int randomWait = UnityEngine.Random.Range(0, max);
        if (eventArgs.newHour == TIME_TO_WORK)
        {
            await Task.Delay(randomWait);
            await FindPath(office);
            moveToCancelToken?.Cancel();
            moveToCancelToken = new CancellationTokenSource();
            movingTask = MoveTo(moveToCancelToken);
        }
        if (eventArgs.newHour == TIME_TO_HOME)
        {
            await Task.Delay(randomWait);
            await FindPath(home);
            moveToCancelToken?.Cancel();
            moveToCancelToken = new CancellationTokenSource();
            movingTask = MoveTo(moveToCancelToken);
        }
    }
    
}
