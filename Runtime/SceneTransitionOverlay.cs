using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransitions
{
  // The scene loading coroutine must be done on this monobehaviour
  // because it will persist across scenes.  If we were to start the scene load
  // coroutine from the trigger object, the coroutine would die right when the new scene loads
  // and the old scene unloads.
  public class SceneTransitionOverlay : MonoBehaviour
  {
    [SerializeField] private float _transitionDuration = 1;
    private Animator _animator;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
      DontDestroyOnLoad(gameObject);
    }

    private IEnumerator LoadSceneAsync(string name)
    {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
      while (!asyncLoad.isDone)
      {
        yield return null;
      }
    }

    public void LoadScene(string toSceneName, List<SetupRoutine> setupRoutines, Action OnFinishCallback)
    {
      StartCoroutine(LoadSceneRoutine(toSceneName, setupRoutines, OnFinishCallback));
    }

    private void NotifyGameObjects(string cbName)
    {
      List<GameObject> rootObjects = new List<GameObject>();
      SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);
      foreach (GameObject gameObject in rootObjects)
      {
        gameObject.BroadcastMessage(cbName, null, SendMessageOptions.DontRequireReceiver);
      }
    }

    private IEnumerator LoadSceneRoutine(string toSceneName, List<SetupRoutine> setupRoutines, Action OnFinishCallback)
    {
      List<IEnumerator> beforeNextSceneLoadSetupRoutines = new List<IEnumerator>();
      List<IEnumerator> afterNextSceneLoadSetupRoutines = new List<IEnumerator>();

      // preserve sort order!
      if (setupRoutines != null)
      {
        foreach (SetupRoutine routine in setupRoutines)
        {
          if (routine.Timing == SceneTransitionCallbackTiming.BeforeNextSceneLoad)
          {
            beforeNextSceneLoadSetupRoutines.Add(routine.Routine);
          }
          else
          {
            afterNextSceneLoadSetupRoutines.Add(routine.Routine);
          }
        }
      }

      // Let the transition in animation play
      _animator.SetBool("SceneVisible", false);
      NotifyGameObjects("OnTransitionOutStart");
      yield return new WaitForSeconds(_transitionDuration / 2f);
      NotifyGameObjects("OnTransitionOutEnd");

      foreach (IEnumerator routine in beforeNextSceneLoadSetupRoutines)
      {
        yield return routine;
      }

      yield return LoadSceneAsync(toSceneName);

      foreach (IEnumerator routine in afterNextSceneLoadSetupRoutines)
      {
        yield return routine;
      }

      // Let the transition out animation play
      _animator.SetBool("SceneVisible", true);
      NotifyGameObjects("OnTransitionInStart");
      yield return new WaitForSeconds(_transitionDuration / 2f);
      NotifyGameObjects("OnTransitionInEnd");

      OnFinishCallback();
    }
  }
}