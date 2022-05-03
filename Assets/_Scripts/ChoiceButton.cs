using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    Image buttonImage;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RightChoiceOnClick()
    {
        buttonImage.color = Color.green;
        DisableAllChoiceButtons();
    }

    public void WrongChoiceOnClick()
    {
        buttonImage.color = Color.red;
        DisableAllChoiceButtons();
    }

    void DisableAllChoiceButtons()
    {
        var objects = GameObject.FindGameObjectsWithTag("ChoiceButtons");
        foreach (var o in objects)
        {
            Button button = o.GetComponent<Button>();
            button.interactable = false;
        }
    }
}
