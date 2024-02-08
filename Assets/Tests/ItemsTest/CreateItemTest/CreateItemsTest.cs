using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class CreateItemsTest: IResult
{

    [SetUp]
    public void SetUp(){}

    [TestCase (2, 2)]
    [TestCase(3, 3)]
    [TestCase(9, 9)]
    [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Add_Items_Local_Db(int cant, int expectedResult)
    {
        for(int index = 0; index < cant; index ++ )
        {
            MyApplicationTest.GetRepository().SaveLocalItem(CreateItemLocalTest());
        }
        Assert.AreEqual(expectedResult, 
            MyApplicationTest.GetRepository().GetLocalItems().Count, 
            $"Datos locales agregados es {cant} y esperados es {expectedResult}");
    }

    [Test]
    [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Put_One_Item_Remote()
    {
        MyApplicationTest.GetRepository().SaveItemRemote(GetOneItemRemoteTest(), this);

        Assert.AreEqual(1, MyApplicationTest.GetRepository().GetItemsRemote().Count, "El tamaño es de 1");
    }

    [Test]
    [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Put_One_Item_Remote_And_Get_Local()
    {
        MyApplicationTest.GetRepository().SaveItemRemote(GetOneItemRemoteTest(), this);
        SyncronizeData(); 
        Assert.AreEqual(1, MyApplicationTest.GetRepository().GetLocalItems().Count, "El tamaño es de 1");
    }

    [Test]
    public void Modify_Item_Remote_Update_Local()
    {
        // SETUP
        // primero guardamos un ítem en la fake remote y sincronizamos con la local db
        ItemRemoteTest itemRemoteTest = GetOneItemRemoteTest();
        MyApplicationTest.GetRepository().SaveItemRemote(itemRemoteTest, this);
        SyncronizeData();

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            MyApplicationTest.GetRepository().GetItemRemoteById(itemRemoteTest.Id);
        getSaveItemRemote.Name = "Item modificado";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated = 
            MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.Name, getItemSaveLocalOutdated.Name);
        SyncronizeData(); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
          MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));
        // ahora deben ser iguales
        Assert.AreEqual(getSaveItemRemote.Name, getItemSaveLocalUpdated.Name);
    }

    [Test]
    public void Delete_Item_Remote_Update_Local()
    {
        // SETUP
        // primero guardamos un ítem en la fake remote y sincronizamos con la local db
        ItemRemoteTest itemRemoteTest = GetOneItemRemoteTest();
        MyApplicationTest.GetRepository().SaveItemRemote(itemRemoteTest, this);
        SyncronizeData();

        // EXERCISE
        // simulamos una eliminación el la base de datos remota
        MyApplicationTest.GetRepository().DeleteItemRemote(itemRemoteTest);


        // VERIFY
        Assert.AreNotEqual(MyApplicationTest.GetRepository().GetItemsRemote().Count,
            MyApplicationTest.GetRepository().GetLocalItems().Count);
        SyncronizeData();
        // deben ser igaules
        Assert.AreEqual(MyApplicationTest.GetRepository().GetItemsRemote().Count,
            MyApplicationTest.GetRepository().GetLocalItems().Count);
        
    }

    public void SyncronizeData()
    {
        List<ItemLocalTest> itemsLocalList = MyApplicationTest.GetRepository().GetLocalItems();
        List<ItemRemoteTest> itemsRemoteList = MyApplicationTest.GetRepository().GetItemsRemote();
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
    }

    private void CreateItem(ItemLocalTest itemLocalTest)
    {
        Debug.Log("Item created id: " + itemLocalTest.Id);
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

    [TearDown]
    public void TearDown()
    {
        ItemRemoteTestManager.GetInstance().ClearAllData();
        ItemLocalTestManager.GetInstance().ClearAllData();
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

    private ItemRemoteTest GetOneItemRemoteTest()
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
