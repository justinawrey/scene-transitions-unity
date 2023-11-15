using System;
using System.Collections;
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

    private IEnumerator LoadSceneRoutine(string toSceneName, IEnumerator setupRoutine, Action OnFinishCallback)
    {
      // Let the transition in animation play
      _animator.SetBool("SceneVisible", false);
      yield return new WaitForSeconds(_halfTransitionDuration);

      // Load the new scene first, so the setup routine can operate on it properly
      yield return LoadSceneAsync(toSceneName);
      if (setupRoutine != null)
      {
        yield return setupRoutine;
      }

      // Let the transition out animation play
      _animator.SetBool("SceneVisible", true);
      yield return new WaitForSeconds(_halfTransitionDuration);
      OnFinishCallback();
    }
  }
}