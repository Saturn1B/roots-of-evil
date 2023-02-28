using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Typewritter : MonoBehaviour
{
    float delay = .075f;
    public string fullText;
    string currentText;

    [SerializeField] GameObject passText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowText());
    }

    public IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            currentText = fullText.Substring(0, i);
            this.GetComponent<TMPro.TMP_Text>().text = currentText;
            yield return new WaitForSeconds(delay);
        }

        passText.SetActive(true);
    }
}
