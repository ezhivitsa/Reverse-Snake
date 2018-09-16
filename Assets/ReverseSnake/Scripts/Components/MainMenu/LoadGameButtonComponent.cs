using Assets.ReverseSnake.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameButtonComponent : MonoBehaviour
{
    Selectable button;

    void OnEnable()
    {
        SaveData.OnLoaded += OnStateLoaded;
        SaveData.Load();
	}

    void OnDestroy()
    {
        SaveData.OnLoaded -= OnStateLoaded;
    }

    private void OnStateLoaded()
    {
        var state = SaveData.state;
        if (state.Steps != null && state.Steps.Count > 0)
        {
            var button = gameObject.GetComponent<Selectable>();
            button.interactable = true;
        }
    }
}
