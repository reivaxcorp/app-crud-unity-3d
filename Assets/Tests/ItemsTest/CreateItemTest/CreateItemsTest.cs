using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class CreateItemsTest: IResult
{

    [SetUp]
    public void SetUp()
    {
    //    MyApplicationTest.GetRepository().SaveLocalItem(CreateItemLocalTest());
     //   MyApplicationTest.GetRepository().SaveLocalItem(CreateItemLocalTest());
    }

    [TestCase (2, 2)]
    [TestCase(3, 5)]
    [TestCase(4, 9)]
    public void AddItemsLocalDb(int cant, int expectedResult)
    {
        for(int index = 0; index < cant; index ++ )
        {
            MyApplicationTest.GetRepository().SaveLocalItem(CreateItemLocalTest());
        }
        Assert.AreEqual(expectedResult, 
            MyApplicationTest.GetRepository().GetLocalItems().Count, 
            $"Datos locales agregados es {cant} y esperados es {expectedResult}");
    }

    [TearDown]
    public void TearDown()
    {
        // Código que se ejecuta después de cada prueba
    }

    [Test]
    public async void VerifyUpdatesTest()
    {
        List<ItemLocalTest> itemsLocalList = MyApplicationTest.GetRepository().GetLocalItems();
        List<ItemRemoteTest> itemsRemoteList = await MyApplicationTest.GetRepository().GetItemsRemote();
        List<ItemLocalTest> itemsUpdated = new List<ItemLocalTest>();


        if (itemsLocalList != null && itemsLocalList.Count > 0 && 
            itemsRemoteList != null && itemsRemoteList.Count > 0)
        {
            List<ItemManagerTest> itemUpdates =
                CheckUpdatesTest.CheckUpdatesItems(itemsRemoteList, itemsLocalList);


            foreach (ItemManagerTest itemToUpdate in itemUpdates)
            {

                if (itemToUpdate.IsFieldsUpdated && itemToUpdate.IsImageUpdated)
                {
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    MyApplicationTest.GetRepository().RemoveTexture(itemToUpdate.Id);
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
                else if (itemToUpdate.IsFieldsUpdated)
                {
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
                else if (itemToUpdate.IsImageUpdated)
                {
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    MyApplicationTest.GetRepository().RemoveTexture(itemToUpdate.Id);
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
                else
                {
                    // Nuevo item añadido
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
            }
        }
        else
        {
            if(itemsRemoteList != null && itemsRemoteList.Count > 0)
            {
                // no hay items guardados en la base de datos local, asi que leemos los items remotos
                foreach (ItemRemoteTest itemRemote in itemsRemoteList)
                {
                    // Nuevo item añadido
                    ItemLocalTest itemLocal = itemRemote.ItemRemoteToItemLocal();
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
            } else if (itemsLocalList != null && itemsLocalList.Count > 0)
            {
                foreach (ItemLocalTest itemLocal in itemsLocalList)
                {
                    // Nuevo item añadido
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
            } else
            {
                Debug.Log("No hay datos guardados local ni remotamente");
            }
        }

        MyApplicationTest.GetRepository().SaveLocalItems(itemsUpdated);

        // Assert.IsEmpty(MyApplicationTest.GetRepository().GetLocalItems());
      Assert.AreEqual(9, MyApplicationTest.GetRepository().GetLocalItems().Count, "El tamaño es de 9");
    }

    private void CreateItem(ItemLocalTest itemLocalTest)
    {
        Debug.Log("Item created");
    }


    public void SetResultCrudUi(bool successful, string msj)
    {
        if(successful)
        {
            Debug.Log("Documento escrito");
        } else
        {
            Debug.Log("Error al escribir el documento");
        }
    }

    public void SetResultWriteDocument(bool successful, string title, string body)
    {
        if(successful)
        {
            Debug.Log("Documento escrito"); 
        }
    }

    private ItemLocalTest CreateItemLocalTest()
    {
        // Genera datos aleatorios para el item
        string id = Guid.NewGuid().ToString();
        string name = $"Item_{id.Substring(0, 5)}";
        string path = $"Path_{id.Substring(0, 5)}";
        string imageIdMetadata = $"ImageId_{id.Substring(0, 5)}";
        long creationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Crea una nueva instancia de ItemRemote
        ItemLocalTest item = new ItemLocalTest(id, name, path, imageIdMetadata, creationDate);
        return item;
    }

    private ItemRemoteTest CreateItemRemoteTest()
    {
        // Genera datos aleatorios para el item
        string id = Guid.NewGuid().ToString();
        string name = $"Item_{id.Substring(0, 5)}";
        string path = $"Path_{id.Substring(0, 5)}";
        string imageIdMetadata = $"ImageId_{id.Substring(0, 5)}";
        long creationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Crea una nueva instancia de ItemRemote
        ItemRemoteTest item = new ItemRemoteTest(id, name, path, imageIdMetadata, creationDate);
        return item;
    }
}
