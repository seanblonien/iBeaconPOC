using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocationService : MonoBehaviour
{
    public Text LatLongText;
    private int updateNum = 0;

    IEnumerator coroutine;

    IEnumerator Start()
    {
        coroutine = updateGPS();

        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(0.1f);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }


        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            DoUpdate();
            StartCoroutine(coroutine);
        }
    }

    private void DoUpdate()
    {
        var p = "Location: " + Input.location.lastData.latitude.ToString("R") + " " + Input.location.lastData.longitude.ToString("R") + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
        print("#" + updateNum + ": " + p);
        LatLongText.text = p;
        updateNum++;
    }

    IEnumerator updateGPS()
    {
        float UPDATE_TIME = 1f;
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {
            DoUpdate();
            yield return updateTime;
        }
    }

    void stopGPS()
    {
        Input.location.Stop();
        StopCoroutine(coroutine);
    }

    void OnDisable()
    {
        stopGPS();
    }
}