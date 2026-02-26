namespace BasicEnemy
{
    public abstract class State 
    {
        public StateEvent StateStage { get; set; } 
        public FiniteStateMachine FSM { get; } 

        protected State(FiniteStateMachine fsm) 
        {
            FSM = fsm; 
            StateStage = StateEvent.ENTER; 
        }

        public virtual void Enter() => StateStage = StateEvent.UPDATE;
        public abstract void Update(); 
        public virtual void Exit() => StateStage = StateEvent.EXIT;
    }
}