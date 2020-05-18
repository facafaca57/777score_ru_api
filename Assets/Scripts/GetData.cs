using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class GetData : MonoBehaviour
{
    private readonly string urlAPI = $"https://777score.ru/api/v1/football/matches-by-date?date={DateTime.Now.ToString("yyyy-MM-dd")}";
    [SerializeField] GameObject content;

    public List<DataJSON> dataJSON = new List<DataJSON>();
    public List<DataJSON> localJSON = new List<DataJSON>();
    public string dateFromList;
    string path;

    public void Start()
    {
        path = Application.persistentDataPath + "/data.save";
        if (File.Exists(path))
        {
            LoadData();
            DateTime dateLocal = Convert.ToDateTime(dateFromList);

            if ( DateTime.Now.Date > dateLocal.Date ) { File.Delete(path); }
            StartCoroutine(GetRequest(urlAPI));
        }
        else
        {
            Debug.LogError("Save File not found " + path);
            StartCoroutine(GetRequest(urlAPI));
        }
    }
    public IEnumerator GetRequest(string api)
    {
        int dayCount = 1;
        DateTime dateNow = DateTime.Now;
        while (dayCount <= 3)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(api))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.Log("Error: " + webRequest.error);
                }
                else
                {
                    var data = JSON.Parse(webRequest.downloadHandler.text);
                    for (int i = 0; i < data["uniqueTournaments"].Count; i++)
                    {
                        for (int j = 0; j < data["uniqueTournaments"][i]["matches"].Count; j++)
                        {
                            var coef = getCoeficient(data["uniqueTournaments"][i]["matches"][j]["id"]);
                            dataJSON.Add(new DataJSON(
                                data["uniqueTournaments"][i]["matches"][j]["id"],
                                data["uniqueTournaments"][i]["matches"][j]["homeTeam"]["name"],
                                data["uniqueTournaments"][i]["matches"][j]["awayTeam"]["name"],
                                coef[0],
                                coef[1],
                                coef[2],
                                data["uniqueTournaments"][i]["matches"][j]["dateOfMatch"],
                                data["uniqueTournaments"][i]["matches"][j]["final"],
                                data["uniqueTournaments"][i]["matches"][j]["isLive"],
                                getFavorite(data["uniqueTournaments"][i]["matches"][j]["id"])
                            ));
                        }
                    }
                    api = $"https://777score.ru/api/v1/football/matches-by-date?date={dateNow.AddDays(-dayCount).ToString("yyyy-MM-dd")}";
                    dayCount++;
                }
            }
        }
        SaveData();
        content.GetComponent<LoadContent>().enabled = true;
    }

    public bool getFavorite(int id) {
        try { return localJSON.Find(item => item.id == id).isFavorite; } catch(Exception e) { return false; }
    }

    public double[] getCoeficient(int id)
    {
        try {
            double coef1 = localJSON.Find(item => item.id == id).coef1;
            double coef2 = localJSON.Find(item => item.id == id).coef2;
            double coef3 = localJSON.Find(item => item.id == id).coef3;
            double[] coefs = { coef1, coef2, coef3 };
            return coefs;
        } catch (Exception e) {
            double coef1 = Math.Round(UnityEngine.Random.Range(0.0f, 10.0f), 2);
            double coef2 = Math.Round(UnityEngine.Random.Range(0.0f, 10.0f), 2);
            double coef3 = Math.Round(UnityEngine.Random.Range(0.0f, 10.0f), 2);
            double[] coefs = { coef1, coef2, coef3 };
            return coefs;
        }
    }

    public void UpdateFavorite(int id, bool check) {
        dataJSON.Find(item => item.id == id).isFavorite = check ? true : false;
        new Thread(() =>{
            Thread.CurrentThread.IsBackground = true;
            SaveData();
        }).Start();
        //SaveData();
    }

    public void SaveData() {
        SaveSystem.SaveData(dataJSON, path);
    }

    public void LoadData() {
        var data = SaveSystem.LoadData();
        string[] listItem = data.Split('|');
        dateFromList = listItem[listItem.Length - 1];

        for (int i = 0; i < listItem.Length - 1; i++)
        {
            var listData = JSON.Parse(listItem[i]);
            localJSON.Add(new DataJSON(
                listData["id"],
                listData["homeTeam"],
                listData["awayTeam"],
                listData["coef1"],
                listData["coef2"],
                listData["coef3"],
                listData["dateOfMatch"],
                listData["final"],
                listData["isLive"],
                listData["isFavorite"]
            ));
        }
    }

    void OnApplicationQuit() { }
    void OnApplicationPause(bool pauseStatus) { if (pauseStatus) { } else { } }
}

[Serializable]
public class DataJSON
{
    public int id;
    public string homeTeam;
    public string awayTeam;
    public double coef1;
    public double coef2;
    public double coef3;
    public string dateOfMatch;
    public string final;
    public bool isLive;
    public bool isFavorite;

    public DataJSON(int id, string homeTeam, string awayTeam, double coef1, double coef2, double coef3, string dateOfMatch, string final, bool isLive, bool isFavorite)
    {
        this.id = id;
        this.homeTeam = homeTeam;
        this.awayTeam = awayTeam;
        this.coef1 = coef1;
        this.coef2 = coef2;
        this.coef3 = coef3;
        this.dateOfMatch = dateOfMatch;
        this.final = final;
        this.isLive = isLive;
        this.isFavorite = isFavorite;
    }
}































/*DataJSON data = JsonUtility.FromJson<DataJSON>(webRequest.downloadHandler.text);

[System.Serializable]
public class DataJSON
{
    public TournamentsJSON[] uniqueTournaments;
    public string timestamp;
    //public static DataJSON CreateFromJSON(string data) { return JsonUtility.FromJson<DataJSON>(data); }

        public string homeTeam;
        public string awayTeam;
        public string dateOfMatch;
        public string final;

        public DataJSON(string homeTeam, string awayTeam, string dateOfMatch, string final)
        {
            this.homeTeam = homeTeam;
            this.awayTeam = awayTeam;
            this.dateOfMatch = dateOfMatch;
            this.final = final; 
        }
        
}

[System.Serializable]
public class TournamentsJSON
{
    [JsonProperty("407")]
    public ListJSON[] JsonProperty;
}

[System.Serializable]
public class ListJSON
{
    //public MatchesJSON[] matches;
    public string name;
}

[System.Serializable]
public class MatchesJSON
{
    public int[] homeTeam;
    public int[] awayTeam;
    public int dateOfMatch;
    public int final;
}
*/
