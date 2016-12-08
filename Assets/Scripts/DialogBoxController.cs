using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogBoxController : MonoBehaviour
{

    public Text dialogTextBox;
    private string[] m_dialogLines;
    public float dialogTimer = 4;
    private float m_dialogTimer;
    private int m_currentIndex, m_endIndex;
    private bool m_receivedDialog;
    private bool m_firstPass;
    // Use this for initialization
    void Start()
    {
        m_dialogTimer = dialogTimer;
        m_currentIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitDialogBox();
        }
        if (m_dialogTimer >= dialogTimer || m_firstPass)
        {
            ContinueDialog();
            m_dialogTimer = 0;
            m_firstPass = false;
        }

        m_dialogTimer += Time.deltaTime;
    }
    private void ContinueDialog()
    {
        if (m_receivedDialog)
        {
            if (m_currentIndex <= m_endIndex)
            {
                dialogTextBox.text = m_dialogLines[m_currentIndex];
                m_currentIndex++;
            }
            else
            {
                ExitDialogBox();
            }
        }
    }
    public void SetDialog(TextAsset textFile)
    {
        m_dialogLines = textFile.text.Split('\n');
        m_receivedDialog = true;
        m_endIndex = m_dialogLines.Length - 1;
        m_firstPass = true;
    }
    //Hide and Perform the operations setting up the dialog box for the next time it's needed
    private void ExitDialogBox()
    {
        this.gameObject.SetActive(false);
        m_receivedDialog = false;
        m_currentIndex = 0;
        m_dialogTimer = dialogTimer;
        dialogTextBox.text = "";
    }
}
