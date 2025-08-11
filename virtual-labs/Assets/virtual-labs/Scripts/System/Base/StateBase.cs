using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs
{
    /// <summary> Base class for states pattern, inherit from this if you want to use state machine pattern</summary>
    public abstract class StateBase
    {
        public enum State
        {
            start, update, exit
        }

        // add this in execute if you want to exit state externally from
        // camera manager.
        public bool canExitState = false;
        public State currentState;
        public StateBase nextState;

        protected virtual void Start() { currentState = State.update; }
        protected virtual void Execute() { }
        protected virtual void Exit() { currentState = State.exit; }

        public StateBase Process()
        {
            if (currentState == State.start)
                Start();
            if (currentState == State.update)
                Execute();
            if (currentState == State.exit)
            {
                Exit();
                return nextState;
            }

            return this;
        }
    }
}