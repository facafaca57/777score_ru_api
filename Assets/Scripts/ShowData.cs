using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications.Android;
using SimpleJSON;
using System;

public class ShowData : MonoBehaviour
{
    internal int id;
    internal string homeTeam;
    internal string awayTeam;
    internal double[] coefs;
    internal string dateOfMatch;
    internal string final;
    internal bool isLive;
    internal bool isFavorite;
    bool notf = false;
    internal bool showStar = false;
    [Header("Elements")]
    [SerializeField] public Text Title;
    [SerializeField] public Text Time;
    [SerializeField] public Text StatusText;
    [SerializeField] public Image StatusBG;
    [SerializeField] public Image Calendar;
    [SerializeField] public Image Star;
    [SerializeField] public Text Coef1;
    [SerializeField] public Text Coef2;
    [SerializeField] public Text Coef3;
    [SerializeField] public Sprite bgLive;
    [SerializeField] public Sprite bgOffline;
    [SerializeField] public Sprite FavoriteON;
    [SerializeField] public Sprite FavoriteOFF;
    [SerializeField] public Sprite NotfON;
    GameObject Manager;
    DateTime dateMatch;

    void Start()
    {
        DateTime dateNow = DateTime.Now;
        dateMatch = Convert.ToDateTime(dateOfMatch).ToUniversalTime();
        bool chck = dateNow < dateMatch;

        this.Title.text = ($"{homeTeam} - {awayTeam} {final}");
        this.Time.text = ($"Сегодня в {dateMatch.ToString("HH:mm")}");
        this.Coef1.text = coefs[0].ToString();
        this.Coef2.text = coefs[1].ToString();
        this.Coef3.text = coefs[2].ToString();
        if (isLive) { this.StatusText.text = "LIVE"; }
        else if (chck) { this.StatusText.text = "Ожидаеться"; }
        else { this.StatusText.text = "Завершен"; }
        StatusBG.GetComponent<Image>().sprite = isLive ? bgLive : bgOffline;
        Star.GetComponent<Image>().sprite = isFavorite ? FavoriteON : FavoriteOFF;

        Calendar.enabled = (isLive || chck) ? true : false;
        Star.enabled = (isLive || chck || showStar) ? true : false;

        CreateNotifiChannel();
    }
    public void AddFavorite()
    {
        if (!isFavorite)
        {
            Manager = GameObject.Find("Manager");
            Manager.GetComponent<GetData>().UpdateFavorite(id, true);
            Star.GetComponent<Image>().sprite = FavoriteON;
            isFavorite = true;
        }
        else {
            Manager = GameObject.Find("Manager");
            Manager.GetComponent<GetData>().UpdateFavorite(id, false);
            Star.GetComponent<Image>().sprite = FavoriteOFF;
            isFavorite = false;
        }
    }

    void CreateNotifiChannel() 
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void AddToNotification() {
        if (!notf) {
            var notification = new AndroidNotification();
            notification.Title = "777Score Footbal";
            notification.Text = ($"Матч - {homeTeam} : {awayTeam} {dateMatch.ToString("HH:mm")}");
            notification.FireTime = dateMatch.AddHours(-1);
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
            Calendar.GetComponent<Image>().sprite = NotfON;
            notf = true;
        }
    }
}
