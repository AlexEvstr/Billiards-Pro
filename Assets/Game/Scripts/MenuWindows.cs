using System.Collections;
using UnityEngine;

public class MenuWindows : MonoBehaviour
{
    [SerializeField] private GameObject SettingsWindow;
    [SerializeField] private GameObject ResultsWindow;
    [SerializeField] private float animationDuration = 0.2f;

    public void OpenSettings()
    {
        StartCoroutine(AnimateWindow(SettingsWindow, true));
    }

    public void CloseSettings()
    {
        StartCoroutine(AnimateWindow(SettingsWindow, false));
    }

    public void OpenResults()
    {
        StartCoroutine(AnimateWindow(ResultsWindow, true));
    }

    public void CloseResults()
    {
        StartCoroutine(AnimateWindow(ResultsWindow, false));
    }

    private IEnumerator AnimateWindow(GameObject window, bool opening)
    {
        if (opening)
            window.SetActive(true);

        Vector3 startScale = opening ? Vector3.zero : Vector3.one;
        Vector3 peakScale = Vector3.one * 1.1f;
        Vector3 endScale = opening ? Vector3.one : Vector3.zero;

        float time = 0f;

        while (time < animationDuration)
        {
            float t = time / animationDuration;
            window.transform.localScale = Vector3.Lerp(startScale, peakScale, t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        window.transform.localScale = peakScale;

        time = 0f;
        while (time < animationDuration)
        {
            float t = time / animationDuration;
            window.transform.localScale = Vector3.Lerp(peakScale, endScale, t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        window.transform.localScale = endScale;

        if (!opening)
            window.SetActive(false);
    }
}
