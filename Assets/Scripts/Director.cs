using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Director : MonoBehaviour
{
    [SerializeField] ImageTracker imageTracker;
    [SerializeField] UnityTransportClient transportClient;
    [SerializeField] GameObject btnSendData;
    [SerializeField] TMP_InputField IPInputField;

    string IP = "192.168.1.67";
    string messageCache;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        btnSendData.SetActive(false);
        imageTracker.OnQRCodeIdentified += OnQRCodeRecognized;

        yield return new WaitForSeconds(1.0f);

        IPInputField.text = IP;
        // transportClient.Connect(IP);    
    }

    private void OnDestroy()
    {
        transportClient.Disconnect();
        imageTracker.OnQRCodeIdentified -= OnQRCodeRecognized;
    }


    public void OnBtnConnect()
    {
        transportClient.Connect(IPInputField.text);
    }

    public void OnBtnSendData()
    {
        //Vector3 position = transform.position;
        //string message = $"{position.x++},{position.y},{position.z}";

        transportClient.Send(messageCache);
    }

    public void OnBtnSendData(int idx)
    {
        messageCache = idx.ToString();

        transportClient.Send(messageCache);
    }

    void OnQRCodeRecognized(string message)
    {
        btnSendData.SetActive(true);
        messageCache = message;
    }
}
