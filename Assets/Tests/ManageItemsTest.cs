/*********************************************************************************
 * Nombre del Archivo:     ManageItemsTest.cs
 * Descripción:            Test para la creación, actualizacion, borrado y lectura de los ítems en la 
 *                         escena 
 *                         
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/


using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
// Punto de entrada de los tests. 
/// Aquí se prueba las partes sensibles de CRUD, solo la más importantes. 
/// Se simula la lectura y escritura de archivos en forma local y remota, así como 
/// también las texturas.
/// </summary>
[TestFixture]
public class ManageItemsTest : IResultTest 
{

    [SetUp]
    public void SetUp() { }

    [TestCase(2, 2)]
    [TestCase(3, 3)]
    [TestCase(9, 9)]
    // [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Add_Items_Local_Db(int cant, int expectedResult)
    {

        List<ItemLocalTest> saveLocalItemList = new List<ItemLocalTest>();
        for (int index = 0; index < cant; index++)
        {
            saveLocalItemList.Add(CreateItemLocalTest());
        }

        MyApplicationTest.GetRepository().SaveLocalItemsAsync(saveLocalItemList);

        Assert.AreEqual(expectedResult,
            LoadLocalData().Count,
            $"Datos locales agregados es {cant} y esperados es {expectedResult}");
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

    [Test]
    // [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Put_One_Item_Remote()
    {
        MyApplicationTest.GetRepository().SaveItemRemote(GetOneItemRemoteTest(), this);

        Assert.AreEqual(1, GetFakeDbRemoteListening().Count, "El tamaño es de 1");
    }

    [Test]
    // [Ignore("Esta prueba está desactivada temporalmente por razones específicas")]
    public void Put_One_Item_Remote_And_Get_Local()
    {
        MyApplicationTest.GetRepository().SaveItemRemote(GetOneItemRemoteTest(), this);
        SyncronizeData(GetFakeDbRemoteListening());
        Assert.AreEqual(1, LoadLocalData().Count, "El tamaño es de 1");
    }

    [Test]
    public void Modify_ImageName_and_NameItem_Remote_Update_Local()
    {
        // SETUP
        // primero guardamos un ítem en la fake remote y sincronizamos con la local db
        ItemRemoteTest itemRemoteTest = GetOneItemRemoteTest();
        MyApplicationTest.GetRepository().SaveItemRemote(itemRemoteTest, this);
        SyncronizeData(GetFakeDbRemoteListening());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            GetItemRemoteById(itemRemoteTest.Id);
        getSaveItemRemote.ImageName = "ImageId_11122233344455566";
        getSaveItemRemote.Name = "Milanesa a la napolitana";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
             MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.ImageName, getItemSaveLocalOutdated.ImageName);
        Assert.AreNotEqual(getSaveItemRemote.Name, getItemSaveLocalOutdated.Name);
        SyncronizeData(GetFakeDbRemoteListening()); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
            MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);
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
        SyncronizeData(GetFakeDbRemoteListening());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            GetItemRemoteById(itemRemoteTest.Id);
        getSaveItemRemote.ImageName = "ImageId_11122233344455566";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
            MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.ImageName, getItemSaveLocalOutdated.ImageName);
        SyncronizeData(GetFakeDbRemoteListening()); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
          MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);
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
        SyncronizeData(GetFakeDbRemoteListening());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            GetItemRemoteById(itemRemoteTest.Id);
        getSaveItemRemote.ImageName = "adsfasdfasdfasd232fsdf.png";

        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
            MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.ImageName, getItemSaveLocalOutdated.ImageName);
        SyncronizeData(GetFakeDbRemoteListening()); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
          MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);
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
        SyncronizeData(GetFakeDbRemoteListening());

        // EXERCISE
        // simulamos una modificacion el la base de datos remota
        ItemRemoteTest getSaveItemRemote =
            GetItemRemoteById(itemRemoteTest.Id);
        if(getSaveItemRemote == null)
        {
            Debug.Log("null");
        }
        getSaveItemRemote.Name = "Item modificado";
            
        // obtenemos el item local, pero este debe estar desactualizado, ya que se cambio el nombre
        ItemLocalTest getItemSaveLocalOutdated =
            MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);

        // VERIFY
        // no deben ser igaules
        Assert.AreNotEqual(getSaveItemRemote.Name, getItemSaveLocalOutdated.Name);
        SyncronizeData(GetFakeDbRemoteListening()); // Sincronizamos los datos local con la remota.
        ItemLocalTest getItemSaveLocalUpdated =
          MyApplicationTest.GetRepository().GetLocalItemById(itemRemoteTest.Id);
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
        SyncronizeData(GetFakeDbRemoteListening());

        // EXERCISE
        // simulamos una eliminación el la base de datos remota
        MyApplicationTest.GetRepository().DeleteItemRemoteById(itemRemoteTest.Id);


        // VERIFY
        Assert.AreNotEqual(GetFakeDbRemoteListening().Count,
            LoadLocalData().Count);
        SyncronizeData(GetFakeDbRemoteListening());
        // deben ser igaules
        Assert.AreEqual(GetFakeDbRemoteListening().Count,
            LoadLocalData().Count);

    }

    /// <summary>
    /// Sincronizamos datos remotos con los datos locales.
    /// </summary>
    /// <param name="itemsRemoteList">La lista con el que se realizará la operación. Puede ser null.</param>
    public async void SyncronizeData(List<ItemRemoteTest> itemsRemoteList)
    {
        List<ItemLocalTest> itemsLocalList = LoadLocalData();
         List<ItemLocalTest> itemsToSave = new List<ItemLocalTest>();

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        // Estamos con conexión a internet.
        if (itemsRemoteList != null)
        {
            List<ItemUpdateTest> itemListUpdates =
  CheckUpdatesTest.CheckUpdatesItems(itemsRemoteList, itemsLocalList);


            foreach (ItemUpdateTest itemToUpdate in itemListUpdates)
            {

                if (itemToUpdate.IsFieldsUpdated && itemToUpdate.IsImageUpdated)
                {
                    ItemLocalTest itemLocalUptated = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id)
                        .ItemRemoteToItemLocal();
                    ItemLocalTest itemOld = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    DeleteOldImage(itemOld.ImageName);
                    CreateItemInScene(itemLocalUptated);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsFieldsUpdated)
                {
                    ItemLocalTest itemLocalUptated = 
                        itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    ItemLocalTest itemOld = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    DeleteOldImage(itemOld.ImageName);
                    CreateItemInScene(itemLocalUptated);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsImageUpdated)
                {
                    ItemLocalTest itemLocalUptated = 
                        itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    ItemLocalTest itemOld = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    DeleteOldImage(itemOld.ImageName);
                    CreateItemInScene(itemLocalUptated);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsRemove)
                {
                    ItemLocalTest itemLocalToDelete = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    DeleteOldImage(itemLocalToDelete.ImageName);
                    DeleteItemInScene(itemLocalToDelete);
                }
                else if (itemToUpdate.IsAdd)
                {
                    // Nuevo item añadido
                    ItemLocalTest itemLocalToAdd = 
                        itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    CreateItemInScene(itemLocalToAdd);
                    itemsToSave.Add(itemLocalToAdd);
                }
                else
                {
                    // sin cambios el ítem local con el ítem remoto
                    ItemLocalTest itemLocal = itemsLocalList.Find(item => item.Id == itemToUpdate.Id);
                    CreateItemInScene(itemLocal);
                    itemsToSave.Add(itemLocal);
                }
            }
        } 
        else
        {
            // Estamos sin sin conexión a internet, cargamos los datos locales
            foreach (ItemLocalTest itemLocal in itemsLocalList)
            {
                CreateItemInScene(itemLocal);
            }
        }

        // Esperar a que todas las tareas se completen
        await Task.WhenAll(tasks);

        MyApplicationTest.GetRepository().SaveLocalItemsAsync(itemsToSave);
    }

    /// <summary>
    /// Necesitamos la forma de simular una lectura a la db remota
    /// Ya que los datos se escuchan en tiempo real
    /// </summary>
    /// <returns></returns>
    private List<ItemRemoteTest> GetFakeDbRemoteListening()
    {
        return ItemRemoteTestManager.GetInstance().GetItemsRemote();
    }

    private ItemRemoteTest GetItemRemoteById(string id)
    {
        return ItemRemoteTestManager.GetInstance().GetItemRemoteById(id);
    }

    private void DeleteItemInScene(ItemLocalTest itemLocalTest)
    {
        Debug.Log("Ítem en la escena ficticio ha sido borrado " + itemLocalTest.Id);
    }

    private void DeleteOldImage(string oldImageName)
    {
        Debug.Log("Imagén remota borrada de forma fictisia " + oldImageName);
    }

    private List<ItemLocalTest> LoadLocalData()
    {
        List<ItemLocalTest> itemsLocalList = MyApplicationTest.GetRepository().GetLocalItemsAsync();
        return itemsLocalList;
    }

    // Metodos await no funciona en Test,
    // asi que lo omitimos y lo dejamos lo mas parecido a la implementacion final
    private bool CreateItemInScene(ItemLocalTest itemLocalTest)
    {
        Debug.Log("Item created id: " + itemLocalTest.Id);
        return true;
    }

    public void SetResultCrudUi(string title, string msj)
    {
        Debug.Log("Documento escrito " + title + " " + msj);
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
