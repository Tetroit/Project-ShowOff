using System.Collections;

public interface IInteractable
{
    IEnumerator Interact();
    IEnumerator Deselect();
}
