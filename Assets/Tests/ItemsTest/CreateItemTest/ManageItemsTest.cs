using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine;

[TestFixture]
public class ManageItemsTest : IResult
{

    [SetUp]
    public void SetUp() { }

    [TestCase(2, 2)]
    [TestCase(3, 3)]
    [TestCase(9, 9)]
    // [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Add_Items_Local_Db(int cant, int expectedResult)
    {
        for (int index = 0; index < cant; index++)
        {
            MyApplicationTest.GetRepository().SaveLocalItem(CreateItemLocalTest());
        }
        Assert.AreEqual(expectedResult,
            MyApplicationTest.GetRepository().GetLocalItems().Count,
            $"Datos locales agregados es {cant} y esperados es {expectedResult}");
    }

    [Test]
    // [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Put_One_Item_Remote()
    {
        MyApplicationTest.GetRepository().SaveItemRemote(GetOneItemRemoteTest(), this);

        Assert.AreEqual(1, MyApplicationTest.GetRepository().GetItemsRemote().Count, "El tamaño es de 1");
    }

    [Test]
    // [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Put_One_Item_Remote_And_Get_Local()
    {
        MyApplicationTest.GetRepository().SaveItemRemote(GetOneItemRemoteTest(), this);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote());
        Assert.AreEqual(1, MyApplicationTest.GetRepository().GetLocalItems().Count, "El tamaño es de 1");
    }

    [Test]
    public void Modify_ImageName_and_NameItem_Remote_Update_Local()
    {
        // SETUP
        // primero guardamos un ítem en la fake remote y sincronizamos con la local db
        ItemRemoteTest itemRemoteTest = GetOneItemRemoteTest();
        MyApplicationTest.GetRepository().SaveItemRemote(itemRemoteTest, this);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            MyApplicationTest.GetRepository().GetItemRemoteById(itemRemoteTest.Id);
        getSaveItemRemote.ImageName = "ImageId_11122233344455566";
        getSaveItemRemote.Name = "Milanesa a la napolitana";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
            MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.ImageName, getItemSaveLocalOutdated.ImageName);
        Assert.AreNotEqual(getSaveItemRemote.Name, getItemSaveLocalOutdated.Name);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote()); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
          MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));
        // ahora deben ser iguales
        Assert.AreEqual(getSaveItemRemote.ImageName, getItemSaveLocalUpdated.ImageName);
        Assert.AreEqual(getSaveItemRemote.Name, getItemSaveLocalUpdated.Name);
    }

    [Test]
    public void Modify_ImageMetaData_Item_Remote_Update_Local()
    {
        // SETUP
        // primero guardamos un ítem en la fake remote y sincronizamos con la local db
        ItemRemoteTest itemRemoteTest = GetOneItemRemoteTest();
        MyApplicationTest.GetRepository().SaveItemRemote(itemRemoteTest, this);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            MyApplicationTest.GetRepository().GetItemRemoteById(itemRemoteTest.Id);
        getSaveItemRemote.ImageName = "ImageId_11122233344455566";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
            MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.ImageName, getItemSaveLocalOutdated.ImageName);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote()); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
          MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));
        // ahora deben ser iguales
        Assert.AreEqual(getSaveItemRemote.Name, getItemSaveLocalUpdated.Name);
    }

    [Test]
    public void Modify_ImagePath_Item_Remote_Update_Local()
    {
        // SETUP
        // primero guardamos un ítem en la fake remote y sincronizamos con la local db
        ItemRemoteTest itemRemoteTest = GetOneItemRemoteTest();
        MyApplicationTest.GetRepository().SaveItemRemote(itemRemoteTest, this);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            MyApplicationTest.GetRepository().GetItemRemoteById(itemRemoteTest.Id);
        getSaveItemRemote.ImageName = "adsfasdfasdfasd232fsdf.png";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
            MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.ImageName, getItemSaveLocalOutdated.ImageName);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote()); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
          MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));
        // ahora deben ser iguales
        Assert.AreEqual(getSaveItemRemote.ImageName, getItemSaveLocalUpdated.ImageName);
    }

    [Test]
    public void Modify_Name_Item_Remote_Update_Local()
    {
        // SETUP
        // primero guardamos un ítem en la fake remote y sincronizamos con la local db
        ItemRemoteTest itemRemoteTest = GetOneItemRemoteTest();
        MyApplicationTest.GetRepository().SaveItemRemote(itemRemoteTest, this);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            MyApplicationTest.GetRepository().GetItemRemoteById(itemRemoteTest.Id);
        if(getSaveItemRemote == null)
        {
            Debug.Log("null");
        }
        getSaveItemRemote.Name = "Item modificado";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
            MyApplicationTest.GetRepository().GetLocalItems().Find(item => item.Id.Equals(itemRemoteTest.Id));

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.Name, getItemSaveLocalOutdated.Name);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote()); // Sincronizamos los datos local con la remota.
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
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote());

        // EXERCISE
        // simulamos una eliminación el la base de datos remota
        MyApplicationTest.GetRepository().DeleteItemRemoteById(itemRemoteTest.Id);


        // VERIFY
        Assert.AreNotEqual(MyApplicationTest.GetRepository().GetItemsRemote().Count,
            MyApplicationTest.GetRepository().GetLocalItems().Count);
        SyncronizeData(MyApplicationTest.GetRepository().GetItemsRemote());
        // deben ser igaules
        Assert.AreEqual(MyApplicationTest.GetRepository().GetItemsRemote().Count,
            MyApplicationTest.GetRepository().GetLocalItems().Count);

    }

    public async void SyncronizeData(List<ItemRemoteTest> itemsRemoteList)
    {
        List<ItemLocalTest> itemsLocalList = MyApplicationTest.GetRepository().GetLocalItems();
         List<ItemLocalTest> itemsToSave = new List<ItemLocalTest>();

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        // Estamos con conexión a internet.
        if (itemsRemoteList != null)
        {
            List<ItemManagerTest> itemListUpdates =
  CheckUpdatesTest.CheckUpdatesItems(itemsRemoteList, itemsLocalList);


            foreach (ItemManagerTest itemToUpdate in itemListUpdates)
            {
                Task task = Task.CompletedTask; // Inicializar una tarea completada

                if (itemToUpdate.IsFieldsUpdated && itemToUpdate.IsImageUpdated)
                {
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    MyApplicationTest.GetRepository().RemoveTexture(itemToUpdate.Id);
                    itemsToSave.Add(itemLocal);
                    task = CreateItemInScene(itemLocal);
                }
                else if (itemToUpdate.IsFieldsUpdated)
                {
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    itemsToSave.Add(itemLocal);
                    task = CreateItemInScene(itemLocal);
                }
                else if (itemToUpdate.IsImageUpdated)
                {
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    MyApplicationTest.GetRepository().RemoveTexture(itemToUpdate.Id);
                    itemsToSave.Add(itemLocal);
                    task = CreateItemInScene(itemLocal);
                }
                else if (itemToUpdate.IsRemove)
                {
                    MyApplicationTest.GetRepository().DeleteLocalItemById(itemToUpdate.Id);
                }
                else if (itemToUpdate.IsAdd)
                {
                    // Nuevo item añadido
                    ItemLocalTest itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    itemsToSave.Add(itemLocal);
                    task = CreateItemInScene(itemLocal);
                }
                else
                {
                    // sin cambios el ítem local con el ítem remoto
                    ItemLocalTest itemLocal = itemsLocalList.Find(item => item.Id == itemToUpdate.Id);
                    itemsToSave.Add(itemLocal);
                    task = CreateItemInScene(itemLocal);
                }
                tasks.Add(task); // Agregar la tarea a la lista de tareas
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);

            MyApplicationTest.GetRepository().SaveLocalItems(itemsToSave);

        } else
        {
            // Estamos sin sin conexión a internet, cargamos los datos locales
            foreach (ItemLocalTest itemLocal in itemsLocalList)
            {
                Task task = Task.CompletedTask;
                task = CreateItemInScene(itemLocal);
                tasks.Add(task);
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);
        }
    }

    // Metodos await no funciona en Test,
    // asi que lo omitimos y lo dejamos lo mas parecido a la implementacion final
    private async Task<bool> CreateItemInScene(ItemLocalTest itemLocalTest)
    {
        //await Task.Delay(500); 
        Debug.Log("Item created id: " + itemLocalTest.Id);
        return true;
    }

    public void SetResultCrudUi(bool successful, string msj)
    {
        if (successful)
        {
            Debug.Log("Documento escrito");
        }
        else
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
        if (successful)
        {
            Debug.Log("Documento escrito");
        }
    }

    private ItemLocalTest CreateItemLocalTest()
    {
        // Genera datos aleatorios para el item
        string id = Guid.NewGuid().ToString();
        string name = $"Item_{id.Substring(0, 5)}";
        string imageName = $"ImageName_{id.Substring(0, 5)}";
        long creationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Crea una nueva instancia de ItemRemote
        ItemLocalTest item = new ItemLocalTest(id, name, imageName, creationDate);
        return item;
    }

    private ItemRemoteTest GetOneItemRemoteTest()
    {
        // Genera datos aleatorios para el item
        string id = Guid.NewGuid().ToString();
        string name = $"Item_{id.Substring(0, 5)}";
        string imageName = $"ImageName_{id.Substring(0, 5)}";
        long creationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Crea una nueva instancia de ItemRemote
        ItemRemoteTest item = new ItemRemoteTest(id, name, imageName, creationDate);
        return item;
    }
}
