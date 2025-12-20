using System.Collections;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private RectTransform panelPos;

    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject crosshair;

    private Vector2 originalPosition;

    private bool isShown = false;

    private float displayDuration = 5f;
    private float currentDisplayDuration = 0;

    IEnumerator TranslateToPosition(Vector2 position)
    {
        while (panelPos.anchoredPosition != position)
        {
            panelPos.anchoredPosition = Vector2.MoveTowards(
                panelPos.anchoredPosition,
                position,
                //Ngl I just picked 800 for the funsies
                800 * Time.deltaTime
            );
            yield return null;
        }

        isShown = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = panelPos.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShown)
        {
            currentDisplayDuration += Time.deltaTime;
            if (currentDisplayDuration >= displayDuration)
            {
                StartCoroutine(TranslateToPosition(originalPosition));
                isShown = false;
                currentDisplayDuration = 0;
            }
        }
    }

    public void setTutorialText(string newText)
    {
        tutorialText.text = newText;

        // -10 to account for padding
        Vector2 newHeight = new Vector2(panelPos.sizeDelta.x - 10, tutorialText.preferredHeight);
        tutorialText.rectTransform.sizeDelta = newHeight;

        // 4 seconds minimum and 1 second for every 10 characters (cuz reading time yessssss)
        displayDuration = Mathf.Max(4, newText.Length / 10);

        Vector2 targetPosition = new Vector2(originalPosition.x - panelPos.sizeDelta.x * 2, originalPosition.y);
        StartCoroutine(TranslateToPosition(targetPosition));
    }

    public void endGameSetup()
    {
        winText.SetActive(true);
        crosshair.SetActive(false);
    }
}
