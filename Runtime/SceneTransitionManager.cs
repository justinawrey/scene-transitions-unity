using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneTransitions
{
  public enum SceneTransitionCallbackTiming
  {
    BeforeNextSceneLoad,
    AfterNextSceneLoad
  }

  public class SetupRoutine
  {
    private IEnumerator _routine;
    private SceneTransitionCallbackTiming _timing;

    public IEnumerator Routine => _routine;
    public SceneTransitionCallbackTiming Timing => _timing;

    public SetupRoutine(IEnumerator routine, SceneTransitionCallbackTiming timing)
    {
      _routine = routine;
      _timing = timing;
    }
  }

  public static class SceneTransitionManager
  {
    private static GameObject _sceneTransitionOverlayPrefab;
    private static GameObject _transitionObject;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void OnAfterSceneLoad()
    {
      _sceneTransitionOverlayPrefab = Resources.Load<GameObject>("Prefabs/scene-transition-overlay");
    }

    // setupRoutine is executed after the new scene is loaded, but before the fade in transition
    public static void LoadScene(string toSceneName, List<SetupRoutine> setupRoutines = null)
    {
      if (_transitionObject != null)
      {
        return;
      }

      _transitionObject = GameObject.Instantiate(_sceneTransitionOverlayPrefab, Vector3.zero, Quaternion.identity);
      _transitionObject.GetComponent<SceneTransitionOverlay>().LoadScene(toSceneName, setupRoutines, OnTransitionComplete);
    }

    private static void OnTransitionComplete()
    {
      GameObject.Destroy(_transitionObject);
      _transitionObject = null;
    }
  }
}
