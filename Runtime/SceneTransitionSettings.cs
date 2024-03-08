using System.Collections.Generic;
using UnityEngine;

namespace SceneTransitions
{
  [System.Serializable]
  public class SceneTransitionSetting
  {
    public string ToSceneName;
    public List<SceneTransition> PossibleTransitions;

    // choose randomly from the possibilities
    public SceneTransition Transition => PossibleTransitions[Random.Range(0, PossibleTransitions.Count)];
  }

  [CreateAssetMenu(fileName = "Scene Transition Settings", menuName = "Scene Transitions/Scene Transition Settings")]
  public class SceneTransitionSettings : ScriptableObject
  {
    [SerializeField] private List<SceneTransition> _defaultTransitions;
    [SerializeField] private List<SceneTransitionSetting> _overrides = new List<SceneTransitionSetting>();

    public SceneTransition GetTransitionForScene(string sceneName)
    {
      foreach (SceneTransitionSetting sceneTransitionSetting in _overrides)
      {
        if (sceneTransitionSetting.ToSceneName == sceneName)
        {
          return sceneTransitionSetting.Transition;
        }
      }

      return _defaultTransitions[Random.Range(0, _defaultTransitions.Count)];
    }
  }
}