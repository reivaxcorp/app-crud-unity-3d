using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressText : MonoBehaviour
{
    private bool startAnimText;
    private Coroutine animationCoroutine; // Almacena la referencia a la corrutina

    public void StartProgressTextAnimation(string text, TextMeshProUGUI textToAnim)
    {
        this.startAnimText = true;
        textToAnim.color = Color.blue;
        animationCoroutine = StartCoroutine(AnimateText(text, textToAnim));
    }
    public void StopProgressTextAnimation()
    {
        this.startAnimText = false;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    private IEnumerator AnimateText(string text, TextMeshProUGUI textToAnim)
    {
        while (startAnimText) 
        {
            textToAnim.text = text + " .";
            yield return new WaitForSeconds(0.5f);

            textToAnim.text = text + " ..";
            yield return new WaitForSeconds(0.5f);

            textToAnim.text = text + " ...";
            yield return new WaitForSeconds(0.5f);

            textToAnim.text = text;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
