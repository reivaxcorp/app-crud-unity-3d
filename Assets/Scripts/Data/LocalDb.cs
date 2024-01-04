using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LocalDb : IRepositoryLocal
{
    public void DeleteItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemLocal GetItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<List<ItemLocal>> GetItems()
    {
        throw new System.NotImplementedException();
    }

    public void SaveItem(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
