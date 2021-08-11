using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class b_floorKiller : BHIII_bullet
{
    new void Start()
    {
        base.Start();
    }

    public void StartFloor()
    {
        timer = 4;
    }

    public IEnumerator FloorStart() {
        float t = 0;
        while (rendererObj.color != Color.white) {
            t += Time.deltaTime;
            rendererObj.color = Color.Lerp(Color.clear, Color.white, t);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(0.8f);
        collision.enabled = true;
        yield return new WaitForSeconds(2.5f);
        t = 0;
        while (rendererObj.color != Color.clear)
        {
            t += Time.deltaTime;
            rendererObj.color = Color.Lerp(Color.white, Color.clear, t);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        collision.enabled = false;
    }

    new void Update()
    {
    }


    public override void OnImpact()
    {
        base.OnImpact();
    }
}
