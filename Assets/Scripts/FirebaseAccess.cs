using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseAccess : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private DatabaseReference _ref;
    UIManager uIManager;
    private bool checkForConnection;

    private void Start()
    {
        checkForConnection = false;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        /*_ref = databaseReference.GetReference("words").Child("Catergories");
        _ref.ValueChanged += HandleValueChanged;*/

        uIManager = FindObjectOfType<UIManager>();
        uIManager.GenerateButton();
        Refresh();
    }
    private void Update()
    {
        if (checkForConnection)
        {
            Refresh();
            checkForConnection = false;
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
        FirebaseDatabase.DefaultInstance.GetReference("words").Child("Catergories").ValueChanged += FirebaseAccess_ValueChanged;
    }

    private void FirebaseAccess_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int totalCategory = int.Parse(e.Snapshot.ChildrenCount.ToString());
        print($"totalCategory::{totalCategory}");

        PlayerPrefs.SetInt("totalCategory", totalCategory);

        //For getting all the Categories stored in the firebase
        int categoryIndex = 1;
        int wordsIndex = 1;
        foreach (var category in e.Snapshot.Children)
        {
            PlayerPrefs.SetString($"Category_{categoryIndex}", category.Key);
            foreach (var id in category.Children)
            {
                //Words
                PlayerPrefs.SetString($"{category.Key}_word_{wordsIndex}", id.Child("name").Value.ToString());
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

        /*void HandleValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            //RetrieveDatas();
            print("REFRESH");
        }
        private bool CheckInternet()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
               Debug.Log("Error. Check internet connection!");
               return false;
            }
            return true;
        }
        private void OnDestroy()
        {
            databaseReference = null;
            _ref = null;
        }*/
