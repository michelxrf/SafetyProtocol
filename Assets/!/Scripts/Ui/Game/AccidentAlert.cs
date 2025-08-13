using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Controls the on screen alert when an accident is imminent
/// </summary>
public class AccidentAlert : MonoBehaviour
{
    UIDocument uiDocument;
    VisualElement accidentAlertIcon;
    Label timer;

    [SerializeField] float alertBlinkInterval = .5f;
    [SerializeField] bool blinkAlert = true;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.style.display = DisplayStyle.None;

        timer = uiDocument.rootVisualElement.Q<Label>("Countdown");
        accidentAlertIcon = uiDocument.rootVisualElement.Q<VisualElement>("AlertIcon");

        if (blinkAlert)
            StartCoroutine(BlinkAlert());
    }

    private void Update()
    {
        UpdateAlert();
    }

    /// <summary>
    /// Updates the time the player have to answer the current question on screen.
    /// </summary>
    private void UpdateAlert()
    {
        if (WorkerManager.Instance.isCountingDown)
        {
            // updates timer ui
            timer.text = (WorkerManager.Instance.accidentRemainingTime).ToString($"#0.0" + "s");
            uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Blinks the alert icon on and off.
    /// </summary>
    private IEnumerator BlinkAlert()
    {
        while (true)
        {
            if (accidentAlertIcon.style.display == DisplayStyle.Flex)
            {
                accidentAlertIcon.style.display = DisplayStyle.None;
            }
            else
            {
                accidentAlertIcon.style.display = DisplayStyle.Flex;
            }

            yield return new WaitForSeconds(alertBlinkInterval);
        }
    }
}
