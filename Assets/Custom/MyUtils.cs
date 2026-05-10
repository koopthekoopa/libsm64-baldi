using UnityEngine;

public static class MyUtils
{
    public static GameObject FindChild(this GameObject gameObject, string name)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Debug.Log(gameObject.transform.GetChild(i).gameObject.name);
            if (gameObject.transform.GetChild(i).gameObject.name == name)
            {
                return gameObject.transform.GetChild(i).gameObject;
            }
            if (gameObject.transform.GetChild(i).childCount != 0)
            {
                return gameObject.transform.GetChild(i).gameObject.FindChild(name);
            }
        }
        return null;
    }
}