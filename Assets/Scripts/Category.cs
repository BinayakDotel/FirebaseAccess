using System;

[Serializable]
public class Category
{
    public int id;
    public string name;

    public Category(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
    public int getID()
    {
        return this.id;
    }
    public string getName()
    {
        return this.name;
    }
}

