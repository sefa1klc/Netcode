using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    public AICharacterCombatManager aICharacterCombatManager;
    [Header("Current State")]
    [SerializeField] AIState _currentState;

    protected override void Awake()
    {
        base.Awake();

        aICharacterCombatManager = GetComponent<AICharacterCombatManager>();    
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ProcessStateMachine();
    }
    private void ProcessStateMachine()
    {
        AIState nextState = _currentState?.Tick(this);

        if (nextState != null)
        {
            _currentState = nextState;
        }
    }
}

