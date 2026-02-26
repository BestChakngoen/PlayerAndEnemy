using UnityEngine;

public interface IHoldInteractable
{
    bool RequiresHold { get; }
    void BeginHold(GameObject interactor);
    void EndHold(GameObject interactor);
}