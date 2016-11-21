using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogBoxTrigger : MonoBehaviour
{
    public GameObject dialogBox;
    public TextAsset dialogText;
    public Image dialogImage;
    public Sprite newSprite;
    private DialogBoxController m_dialogController;
    void Start()
    {
        m_dialogController = dialogBox.GetComponent<DialogBoxController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!dialogBox.gameObject.activeSelf)
        {
            m_dialogController.SetDialog(dialogText);
            dialogImage.GetComponent<Image>().sprite = newSprite;
            dialogBox.gameObject.SetActive(true);
        }
    }

}
