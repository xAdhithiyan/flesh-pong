using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KevinCastejon.MoreAttributes;

public class TimerManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private List<Timer> timers = new List<Timer>();
    // Update is called once per frame
    private void LateUpdate()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            Timer timer = timers[i];
            if (timer.owner == null)
            {
                timers.RemoveAt(i);
            }
            else if (timer.owner.activeInHierarchy)
            {
                for (int j = 0; j < timer.times.Length; j++)
                {
                    if (timer.times[j].time > 0)
                        timer.times[j].time -= Time.deltaTime;
                    else if (timer.times[j].time < 0)
                    {
                        timer.times[j].time = 0;
                        timer.InvokeOnTimeIsZero(j);
                    }
                }
            }            
        }
    }
    /// <summary>
    /// Generates an timer used to store times, requires owner object to cover deletion of owner
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public Timer GenerateTimers(GameObject owner, int amount = 1)
    {
        if(amount <= 0)
        {
            Debug.Break();
            Debug.LogError("Cannot generate a zero or negative sized timer");
            return null;
        }
        Timer tempTimer = new Timer(amount,owner);
        timers.Add(tempTimer);
        return tempTimer;
        
    }

    public Timer GenerateTimers(Type enumName, GameObject owner)
    {
        Timer tempTimer = new Timer(enumName, owner);
        timers.Add(tempTimer);
        return tempTimer;
    }
    
}
