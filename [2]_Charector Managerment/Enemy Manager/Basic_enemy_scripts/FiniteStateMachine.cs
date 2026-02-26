using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace BasicEnemy
{
    public abstract class FiniteStateMachine : MonoBehaviour
    {
        public State CurrentState { get; set; }
        public State NextState { get; set; }

        public void ProcessFSM()
        {
            if (CurrentState == null) return;
            switch (CurrentState.StateStage)
            {
                case StateEvent.ENTER:
                    CurrentState.Enter();
                    break;
                case StateEvent.UPDATE:
                    CurrentState.Update();
                    break;
                case StateEvent.EXIT:
                    CurrentState.Exit();
                    CurrentState = NextState;
                    break;
            }
        }

        protected virtual void Update()
        {
            ProcessFSM();
        }
    }
}