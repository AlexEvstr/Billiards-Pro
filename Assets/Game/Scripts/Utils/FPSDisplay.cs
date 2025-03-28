using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	[SerializeField] private Text _fpsText;
	public Color textClr = Color.white;

	float deltaTime = 0.0f;

    private float timePassed = 0f;
    private int frameCount = 0;

    void Update()
    {
        timePassed += Time.unscaledDeltaTime;
        frameCount++;

        if (timePassed >= 0.5f)
        {
            float fps = frameCount / timePassed;
            _fpsText.text = string.Format("{0:0.} FPS", fps);

            timePassed = 0f;
            frameCount = 0;
        }
    }
}













