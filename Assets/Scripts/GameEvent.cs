using UnityEngine;
using UnityEngine.Events;

public static class GameEvent
{
    //Score Change
    public class OnMerge : UnityEvent<int> { }
    public static readonly OnMerge OnMergeEvent = new OnMerge();

    public class OnBubbleReachTarget : UnityEvent<Vector2Int> { }
    public static readonly OnBubbleReachTarget OnBubbleReachTargetEvent = new OnBubbleReachTarget();
}