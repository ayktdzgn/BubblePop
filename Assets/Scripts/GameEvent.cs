using UnityEngine.Events;

public static class GameEvent
{
    //Score Change
    public class OnScoreChanged : UnityEvent<int> { }
    public static readonly OnScoreChanged OnScoreChangedEvent = new OnScoreChanged();
}