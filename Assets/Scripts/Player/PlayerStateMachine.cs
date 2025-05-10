using UnityEngine;

public class PlayerStateMachine
{
    public enum State
    {
        Iddle, Running, Jump, Falling, Slide, SuperJump
    }

    public State previousState = State.Iddle;
    public State currentState = State.Iddle;

    public State state
    {
        set
        {
            previousState = state;
            currentState = value;
        }
        get
        {
            return currentState;
        }
    }

    public bool StateHasChanged()
    {
        return currentState != previousState;
    }
}
