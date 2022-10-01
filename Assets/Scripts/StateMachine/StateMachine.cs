using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State m_CurrentState;

    protected abstract State startingState { get; }

    void Start()
    {
        Set(startingState);
    }

    void Update()
    {
        m_CurrentState?.DoUpdate();
    }

    void OnDestroy()
    {
        m_CurrentState?.DoExit();
    }

    private void Set(State state)
    {
        m_CurrentState?.DoExit();
        m_CurrentState = state;
        m_CurrentState?.DoEnter();
    }


}
