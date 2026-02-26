using UnityEngine;
using UnityEngine.Events;

namespace GameManger
{
    [System.Serializable]
    public class GameStateUnityEvent : UnityEvent<GameState> { } 

    [CreateAssetMenu(fileName = "GameStateEvent", menuName = "Game Events/Game State Event")]
    public class GameStateEventSO : ScriptableObject
    {
        public GameStateUnityEvent OnEventRaised = new GameStateUnityEvent();

        public void RaiseEvent(GameState newState)
        {
            OnEventRaised.Invoke(newState);
        }
    }
}