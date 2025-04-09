using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class DynamicFetch : MonoBehaviour
{

    public Action<int> OnFetch;
    public bool isDynamicFetching = true;
    public Button Button;

    private string apiUrl = "http://localhost/sqlconnect/weather.php";
    private string apiKey = "bb426a56953a694b517bf41b50f2bd7b"; 
    private string city = "London"; 

    int dataFetchesCounter = 0; 
    float timer = 0;
    bool isCoroutineRunning = false;

    public List<WeatherResponse> weatherResponseList = new List<WeatherResponse>();


    void Awake()
    {
       // StartCoroutine(FetchExistingDat());
    }

    IEnumerator Start() //allows asynchronousity, fetch in PHP
    {

        string uniqueUrl = apiUrl + "?t=" + DateTime.Now.Ticks; // Add unique query parameter
        using (UnityWebRequest request = UnityWebRequest.Get(uniqueUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                string jsonResponse = request.downloadHandler.text;
                Debug.Log("JSON Response: " + jsonResponse);
                ProcessWeatherData(jsonResponse);
                OnFetch?.Invoke(dataFetchesCounter);
                dataFetchesCounter++;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        void ProcessWeatherData(string json)
        {
            WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);
            weatherResponseList.Add(weatherResponse);
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.dt).DateTime;
            weatherResponse.date = dateTime;
            Debug.Log($"Parsed Date: {dateTime}");
        }

        isCoroutineRunning = false;

    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 && isCoroutineRunning == false && isDynamicFetching)
        {
            isCoroutineRunning = true;
            timer = 0;
            StartCoroutine(Start());
        }
    }

    public void OnButtonClick()
    {
        if (isDynamicFetching)
        {
            isDynamicFetching = false;
            Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Stop Fetching";
        }
        else
        {
            isDynamicFetching = true;
            Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Start Fetching";
        }
    }
    public class WeatherResponse
    {
        public Main main;
        public Weather[] weather;

        public int id;
        public string city;
        public long dt;
        public DateTime date;
        public bool isFetched;
    }
    [System.Serializable]
    public class Main
    {
        public float temp;
    }

    [System.Serializable]
    public class Weather
    {
        public string description;
    }
}

