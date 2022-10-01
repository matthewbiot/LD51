using System;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    private State m_CurrentState;
    private bool m_Complete;

    protected State state => m_CurrentState;
    protected bool complete => m_Complete;

    public void DoEnter()
    {
        state?.DoEnter();
        OnEnter();
    }

    public void DoUpdate()
    {
        state?.DoUpdate();
        OnUpdate();
    }

    public void DoExit()
    {
        state?.DoExit();
        OnExit();
    }

    protected void Set(State state, Predicate<State> predicate = null)
    {
        if (predicate != null && !predicate.Invoke(m_CurrentState))
            return;

        m_CurrentState?.DoExit();
        m_CurrentState = state;
        m_CurrentState?.DoEnter();
    }

    protected void Complete()
    {
        m_Complete = true;
    }

    protected virtual void OnEnter()
    {
        Debug.Log($"State: {name}.{GetType().Name}");
    }

    protected virtual void OnUpdate()
    {

    }

    protected virtual void OnExit()
    {

    }
}
