using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Category
{
    public int wordCount;
    public string name;
    public List<string> words = new List<string>();

    public Category(string name)
    {
        this.wordCount = 0;
        this.name = name;
    }
    public void AddWord(string word)
    {
        words.Add(word);
        this.wordCount += 1;
    }
    public int GetWordCount()
    {
        return this.wordCount;
    }
    public string GetCategoryName()
    {
        return this.name;
    }
}

