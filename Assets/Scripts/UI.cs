using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public DynamicFetch dynamicFetch;

    public GameObject rowPrefab;
    public List<GameObject> rows;

    int lastFetchedIndex = 0;
    private void Awake()
    {
        dynamicFetch.OnFetch += OnFetch;

        
    }

    private void OnFetch(int value)
    {
        GameObject newRow = Instantiate(rowPrefab, transform);
        dynamicFetch.weatherResponseList[value].isFetched = true;
        newRow.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = value.ToString();
        newRow.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { Edit(value); });
        newRow.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Add(value); });
        newRow.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { Delete(value); });
        newRow.transform.GetChild(4).GetChild(0).GetComponent<TMPro.TMP_Text>().text = dynamicFetch.weatherResponseList[value].id.ToString();
        newRow.transform.GetChild(5).GetChild(0).GetComponent<TMPro.TMP_Text>().text = dynamicFetch.weatherResponseList[value].city;
        newRow.transform.GetChild(6).GetChild(0).GetComponent<TMPro.TMP_Text>().text = dynamicFetch.weatherResponseList[value].main.temp.ToString();
        newRow.transform.GetChild(7).GetChild(0).GetComponent<TMPro.TMP_Text>().text = dynamicFetch.weatherResponseList[value].weather[0].description;
        newRow.transform.GetChild(8).GetChild(0).GetComponent<TMPro.TMP_Text>().text = dynamicFetch.weatherResponseList[value].date.ToString();
        newRow.transform.GetChild(9).GetChild(0).GetComponent<TMPro.TMP_Text>().text = dynamicFetch.weatherResponseList[value].isFetched.ToString();

        //newRow.transform.GetChild
        rows.Add(newRow);
        lastFetchedIndex = value;
    }


    void Edit(int value)
    {
        Transform id = rows[value].transform.GetChild(4).GetChild(1);
        id.gameObject.SetActive(true);
        id.GetComponent<TMPro.TMP_InputField>().contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
        dynamicFetch.weatherResponseList[value].id = int.Parse(id.GetComponent<TMPro.TMP_InputField>().text);
        id.GetComponent<TMPro.TMP_InputField>().text = dynamicFetch.weatherResponseList[value].id.ToString();

        //...//

        StartCoroutine(Sender(value, 1));

    }

    void Add(int value)//change to add
    {
        dynamicFetch.weatherResponseList.Add(dynamicFetch.weatherResponseList[value]);
        dynamicFetch.weatherResponseList[dynamicFetch.weatherResponseList.Count - 1].isFetched = false;
        //dynamicFetch.dataFetchesCounter++;
        //OnFetch(dynamicFetch.dataFetchesCounter);

        //StartCoroutine(Sender(dynamicFetch.dataFetchesCounter, 2));
    }

    void Delete(int value)
    {
        dynamicFetch.weatherResponseList.RemoveAt(value);
        rows.RemoveAt(value);
        Destroy(rows[value]);
        StartCoroutine(Sender(value, 3));
    }


    IEnumerator Sender(int value, int method)
    {
        WWWForm form = new WWWForm();
        form.AddField("value", value);
        form.AddField("method", method);
        Debug.Log("before sending request");
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/dataprocess.php", form))
        {
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);
        }

    }

    void Sort()
    {

    }
}
