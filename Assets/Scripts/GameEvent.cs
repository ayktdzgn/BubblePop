using UnityEngine;
using UnityEngine.Events;

public static class GameEvent
{
    public class OnLevelWin : UnityEvent { }
    public static readonly OnLevelWin OnLevelWinEvent = new OnLevelWin();

    public class OnLevelFail: UnityEvent { }
    public static readonly OnLevelFail OnLevelFailEvent = new OnLevelFail();

    //Score Change
    public class OnMerge : UnityEvent<int,int> { }
    public static readonly OnMerge OnMergeEvent = new OnMerge();

    public class OnCheckForFalling : UnityEvent<Vector2Int> { }
    public static readonly OnCheckForFalling OnCheckForFallingEvent = new OnCheckForFalling();

    public class OnCombo : UnityEvent<int> { }
    public static readonly OnCombo OnComboEvent = new OnCombo();

    public class OnPerfect : UnityEvent { }
    public static readonly OnPerfect OnPerfectEvent = new OnPerfect();

    public class OnBubbleReachTarget : UnityEvent<Vector2Int> { }
    public static readonly OnBubbleReachTarget OnBubbleReachTargetEvent = new OnBubbleReachTarget();
}