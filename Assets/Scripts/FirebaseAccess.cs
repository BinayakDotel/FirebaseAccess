using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Firebase.Database;
using Firebase.Extensions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FirebaseAccess : MonoBehaviour
{
    private DatabaseReference databaseReference;
    UIManager uIManager;
    private bool checkForConnection;

    private void Start()
    {
        checkForConnection = true;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        uIManager = FindObjectOfType<UIManager>();
        uIManager.GenerateButton();
        Refresh();
    }
    private void Update()
    {
        if (checkForConnection)
        {
            print("CHECKING FOR CONNECTION");
            if (CheckInternet())
            {
                Refresh();
                checkForConnection = false;
            }
        }
    }
    /*public void RetrieveDatas()
    {
        if (!CheckInternet())
        {
            //StartCoroutine(NoInternet(2));
            return;
        }
        FirebaseDatabase.DefaultInstance.GetReference("words").Child("Catergories").GetValueAsync().ContinueWithOnMainThread(
                    task =>
                    {
                        if (task.IsFaulted)
                        {
                            print("ERROR");
                        }
                        else if (task.IsCanceled)
                        {
                            print("CANCELLED");
                        }
                        else if (task.IsCompleted)
                        {
                            DataSnapshot snapshot = task.Result;

                            int totalCategory = int.Parse(snapshot.ChildrenCount.ToString());
                            PlayerPrefs.SetInt("totalCategory",totalCategory);

                            //For getting all the Categories stored in the firebase
                            int categoryIndex=1;
                            int wordsIndex = 1;
                            foreach (var category in snapshot.Children)
                            {
                                PlayerPrefs.SetString($"Category_{categoryIndex}", category.Key);
                                foreach (var id in category.Children)
                                {
                                    //Words
                                    PlayerPrefs.SetString($"{category.Key}_word_{wordsIndex}", id.Child("name").Value.ToString());
                                    PlayerPrefs.SetInt($"{category.Key}_wordCount",wordsIndex);
                                    ++wordsIndex;
                                }
                                wordsIndex = 1;
                                ++categoryIndex;
                            }
                        }
                    }
                 );
        uIManager.GenerateButton();
    }*/
    public void Refresh()
    {
        if (!CheckInternet())
        {
            StartCoroutine(NoInternet(1));
            return;
        }
        FirebaseDatabase.DefaultInstance.GetReference("words").ValueChanged += FirebaseAccess_ValueChanged;
    }

    private void FirebaseAccess_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        string filePath = $"{Application.persistentDataPath}/wordSearch.json";

        try
        {
            File.WriteAllText(filePath, e.Snapshot.GetRawJsonValue());
            print("SAVED TO LOCAL DB");
        }
        catch
        {
            print("SOMETHING WENT WRONG");
        }

        int totalCategory = int.Parse(e.Snapshot.Child("Catergories").ChildrenCount.ToString());

        PlayerPrefs.SetInt("totalCategory", totalCategory);

        //For getting all the Categories stored in the firebase
        int categoryIndex = 1;
        int wordsIndex = 1;
        foreach (var category in e.Snapshot.Child("Catergories").Children)
        {
            PlayerPrefs.SetString($"Category_{categoryIndex}", category.Key);
            foreach (var id in category.Children)
            {
                //Words
                PlayerPrefs.SetInt($"{category.Key}_wordCount", wordsIndex);
                ++wordsIndex;
            }
            wordsIndex = 1;
            ++categoryIndex;
        }
        uIManager.GenerateButton();
    }
    private bool CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            checkForConnection = true;
            return false;
        }
        return true;
    }

    IEnumerator NoInternet(int time)
    {
        yield return new WaitForSeconds(time);
        if (PlayerPrefs.GetInt($"totalCategory") == 0) {
            uIManager.Status.text = "ERROR! CHECK INTERNET CONNECTION";
            checkForConnection = true;
        }
        else uIManager.Status.text = "";
    }
}