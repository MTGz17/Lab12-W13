using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeatherManager : MonoBehaviour
{
    private const string apiKey = "5832a0ae3cb7839a6d9975fb26ba7021";
    private string weatherUrl;

    public GameObject sun;
    public Material[] skyboxes;
    public TMP_Text weatherText;
    public TMP_Dropdown cityDropdown;

    private string[] cities = { "Orlando", "London", "Tokyo", "Moscow", "Sydney" , "Huslia"};

    private void Start()
    {
        weatherUrl = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid=" + apiKey + "&units=imperial";

        cityDropdown.ClearOptions();
        cityDropdown.AddOptions(new List<string>(cities));

        cityDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        ChangeCity(0);
    }

    private void OnDropdownValueChanged(int cityIndex)
    {
        ChangeCity(cityIndex);
    }

    public IEnumerator GetWeather(string city, Action<string> callback)
    {
        string url = string.Format(weatherUrl, city);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"Network error: {request.error}");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Protocol error: {request.responseCode}");
            }
            else
            {
                callback(request.downloadHandler.text);
            }
        }
    }

    public void OnWeatherDataReceived(string data)
    {
        WeatherInfo weatherInfo = JsonUtility.FromJson<WeatherInfo>(data);

        if (weatherInfo.weather[0].main == "Clear")
        {
            RenderSettings.skybox = skyboxes[0];
        }
        else if (weatherInfo.weather[0].main == "Rain")
        {
            RenderSettings.skybox = skyboxes[1];
        }
        else if (weatherInfo.weather[0].main == "Snow")
        {
            RenderSettings.skybox = skyboxes[2];
        }
        else if (weatherInfo.weather[0].main == "Clouds")
        {
            RenderSettings.skybox = skyboxes[3];
        }

        if (sun != null)
        {
            sun.GetComponent<Light>().intensity = weatherInfo.main.temp > 20 ? 1.5f : 0.7f;
            sun.GetComponent<Light>().color = weatherInfo.weather[0].main == "Clear" ? Color.yellow : Color.gray;
        }

        weatherText.text = $"Weather: {weatherInfo.weather[0].description}\nTemperature: {weatherInfo.main.temp}°C";
    }

    public void ChangeCity(int cityIndex)
    {
        string city = cities[cityIndex];
        StartCoroutine(GetWeather(city, OnWeatherDataReceived));
    }

    [System.Serializable]
    public class WeatherInfo
    {
        public Main main;
        public Weather[] weather;
    }

    [System.Serializable]
    public class Main
    {
        public float temp;
    }

    [System.Serializable]
    public class Weather
    {
        public string main;
        public string description;
    }
}
