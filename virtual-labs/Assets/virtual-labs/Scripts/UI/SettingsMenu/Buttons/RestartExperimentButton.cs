using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartExperimentButton : MonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private FadeScreen _fadeScreen;

    private UnityAction _reloadSceneDelegate;

    private void OnEnable()
    {
        _reloadSceneDelegate = () => StartCoroutine(ReloadScene());
        _restartButton.onClick.AddListener(_reloadSceneDelegate);
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(_reloadSceneDelegate);
    }

    private IEnumerator ReloadScene()
    {
        _fadeScreen.gameObject.SetActive(true);
        ExperimentManager.Instance.EndStageInvoke();
        yield return StartCoroutine(_fadeScreen.StartFade(3));

        //AssetBundlesManager.Instance.UnloadAssetBundle();

        //yield return new WaitForSeconds(0.2f);
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}