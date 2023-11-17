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
    [SerializeField] private float _halfTransitionDuration = 0.5f;
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

    public void LoadScene(string toSceneName, IEnumerator setupRoutine, Action OnFinishCallback)
    {
      StartCoroutine(LoadSceneRoutine(toSceneName, setupRoutine, OnFinishCallback));
    }

    private void NotifyGameObjects()
    {
      List<GameObject> rootObjects = new List<GameObject>();
      SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);
      foreach (GameObject gameObject in rootObjects)
      {
        gameObject.BroadcastMessage("OnBeforeNextSceneSetup", null, SendMessageOptions.DontRequireReceiver);
      }
    }

    private IEnumerator LoadSceneRoutine(string toSceneName, IEnumerator setupRoutine, Action OnFinishCallback)
    {
      // Let the transition in animation play
      _animator.SetBool("SceneVisible", false);
      yield return new WaitForSeconds(_halfTransitionDuration);

      // Before doing any setup logic for the next scene,
      // notify all game objects of the scene change so they can unload accordingly.
      NotifyGameObjects();
      if (setupRoutine != null)
      {
        yield return setupRoutine;
      }

      yield return LoadSceneAsync(toSceneName);

      // Let the transition out animation play
      _animator.SetBool("SceneVisible", true);
      yield return new WaitForSeconds(_halfTransitionDuration);
      OnFinishCallback();
    }
  }
}