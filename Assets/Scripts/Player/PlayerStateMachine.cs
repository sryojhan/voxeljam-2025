using UnityEngine;

public class PlayerStateMachine
{
    public enum State
    {
        Iddle, Running, Jump, Falling, Slide, SuperJump,

        Void
    }

    public State previousState = State.Void;
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

    public void ConsumeState()
    {
        previousState = currentState;
    }
}
