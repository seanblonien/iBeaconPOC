using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        var activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        var parentSceneIndex = gameObject.scene.buildIndex;
        if (activeSceneIndex == parentSceneIndex)
        {
            Response.Invoke();
        }
        else
        {
            Debug.Log("Did not call response because listener was not in active scene.");
        }
    }
}
