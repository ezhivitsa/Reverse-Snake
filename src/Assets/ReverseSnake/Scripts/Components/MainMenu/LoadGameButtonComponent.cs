using Assets.ReverseSnake.Scripts.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameButtonComponent : MonoBehaviour
{
    Selectable button;

    void OnEnable()
    {
        SaveState.OnLoaded += OnStateLoaded;
        SaveState.Load();
	}

    void OnDestroy()
    {
        SaveState.OnLoaded -= OnStateLoaded;
    }

    private void OnStateLoaded()
    {
        var state = SaveState.State;
        if (state.Steps != null && state.Steps.Count > 0)
        {
            var button = gameObject.GetComponent<Selectable>();
            button.interactable = true;
        }
    }
}
