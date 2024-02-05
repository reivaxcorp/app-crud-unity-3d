using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.tvOS;

public class MyApplicationTest : MonoBehaviour
{
    private static MyFakeRepository repository;
   
    public static MyFakeRepository GetRepository()
    {
        if(repository == null)
        {
            RemoteDbTest remoteDb = new RemoteDbTest();
            LocalDbTest localDb = new LocalDbTest();
            repository = new MyFakeRepository(localDb, remoteDb);
            return repository;
        }
        return repository;
    }
}
