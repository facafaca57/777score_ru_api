using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadContent : MonoBehaviour
{
    [SerializeField] public Image Live;
    [SerializeField] public Image Completed;
    [SerializeField] public Image Favorites;
    [SerializeField] public Sprite LiveON;
    [SerializeField] public Sprite LiveOFF;
    [SerializeField] public Sprite CompletedON;
    [SerializeField] public Sprite CompletedOFF;
    [SerializeField] public Sprite FavoritesON;
    [SerializeField] public Sprite FavoritesOFF;

    public GameObject prefab;
    public GameObject more;
    bool status;
    bool favorit;
    int StartLength = 0;
    int EndLength = 10;
    GameObject Manager;
    int Count = 0;
    DateTime date;

    DateTime dateNow = DateTime.Now;
    private void Start()
    {
        ShowLive();
    }

    public void ShowData(bool stat, bool favorite)
    {
        Manager = GameObject.Find("Manager");
        GetData json = Manager.GetComponent<GetData>();
        
        for (int i = StartLength; i < EndLength; i++)
        {
            try { date = Convert.ToDateTime(json.dataJSON[i].dateOfMatch).ToUniversalTime(); } catch (Exception e) { print(e); }

            if (favorite)
            {
                try { 
                    if (json.dataJSON[i].isFavorite == favorite)
                    {
                        CreateInstance(json.dataJSON[i], true);
                    }
                }
                catch (Exception e)
                {
                    print(e);
                    Count++;
                }
            }
            else
            {
                try
                {
                    if (((json.dataJSON[i].isLive == true && stat == true) || (dateNow < date && stat == true)) && Count <= 10)
                    {
                        CreateInstance(json.dataJSON[i], false);
                    }
                    else if(json.dataJSON[i].isLive == false && stat == false && dateNow > date)
                    {
                        CreateInstance(json.dataJSON[i], false);
                    }
                }
                catch (Exception e)
                {
                    print(e);
                    Count++;
                }
            }
        }

        if (Count > 10) {
            GameObject AddButton = (GameObject)Instantiate(more, transform);
            AddButton.GetComponentInChildren<Button>().onClick.AddListener(() => ShowMore(AddButton));
        }
        else {
            ShowMore();
        }
    }

    public void CreateInstance(DataJSON data, bool favorite)
    {
        //Debug.Log($"{data.id} : {data.homeTeam} - {data.awayTeam} - {DateTime.Parse(data.dateOfMatch).ToUniversalTime().ToString("HH:mm")} - [{data.final}]");
        GameObject newItem = (GameObject)Instantiate(prefab, transform);
        ShowData showdata = newItem.GetComponent<ShowData>();
        double[] coefs = { data.coef1, data.coef2, data.coef3 };
        showdata.id = data.id;
        showdata.homeTeam = data.homeTeam;
        showdata.awayTeam = data.awayTeam;
        showdata.coefs = coefs;
        showdata.final = data.final;
        showdata.dateOfMatch = data.dateOfMatch;
        showdata.isLive = data.isLive;
        showdata.isFavorite = data.isFavorite;
        showdata.showStar = favorite ? true : false;

        Count++;
    }

    public void ShowLive()
    {
        Destroy();
        status = true;
        favorit = false;

        Switch(true, false);
        ShowData(status, favorit);
    }
    public void ShowCompleted() {
        Destroy();
        status = false;
        favorit = false;

        Switch(false, true);
        ShowData(status, favorit);
    }

    public void ShowFavorites()
    {
        Destroy();
        status = false;
        favorit = true;

        Switch(false, false);
        ShowData(status, favorit);
    }

    public void Switch(bool LIVE, bool COMPLETED) 
    {
        //GameObject.Find("Manager").GetComponent<GetData>().SaveData();

        if (LIVE) {
            Live.GetComponent<Image>().sprite = LiveON;
            Completed.GetComponent<Image>().sprite = CompletedOFF;
            Favorites.GetComponent<Image>().sprite = FavoritesOFF;
        } else if (COMPLETED) {
            Live.GetComponent<Image>().sprite = LiveOFF;
            Completed.GetComponent<Image>().sprite = CompletedON;
            Favorites.GetComponent<Image>().sprite = FavoritesOFF;
        } else {
            Live.GetComponent<Image>().sprite = LiveOFF;
            Completed.GetComponent<Image>().sprite = CompletedOFF;
            Favorites.GetComponent<Image>().sprite = FavoritesON;
        }
        Count = 0;
        StartLength = 0;
        EndLength = 10;
    }

    public void ShowMore(GameObject btn)
    {
        Count = 0;
        StartLength += 10;
        EndLength = StartLength + 10;
        Destroy(btn);
        ShowData(status, favorit);
    }

    public void ShowMore()
    {
        StartLength += 10;
        EndLength = StartLength + 10;
        ShowData(status, favorit);
    }

    public void Destroy()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}

