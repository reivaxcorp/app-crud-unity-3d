using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScrip : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterSomeSeconds());
    }

    private void OnCollisionEnter(Collision collision)
    {
      //  Destroy(gameObject);    
    }
   

    IEnumerator DestroyAfterSomeSeconds()
    {
        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }
}
