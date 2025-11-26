using UnityEngine;

public class LapCounter : MonoBehaviour
{
    public int lapcounter;

    public void OnCollisionEnter(Collision collision)
    {
        if (CompareTag("Player"))
        {
            lapcounter++;
        }
    }
    void OnApplicationQuit()
    {
        lapcounter = 0;
    }



}
