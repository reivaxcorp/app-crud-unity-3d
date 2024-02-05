using System.Collections.Generic;

public class ItemManagerTest
{
    public string Id;
    public bool IsRemove;
    public bool IsAdd;
    public bool IsImageUpdated { get; private set; }
    public bool IsFieldsUpdated { get; private set; }
    public List<ItemLocalTest> itemsToAdd { get; private set; }
    public List<ItemLocalTest> itemsToRemove { get; private set; }


    public ItemManagerTest(string id, bool isImageUpdated, bool isFieldsUpdated, bool isRemove, bool isAdd)
    {
        this.Id = id;
        this.IsRemove = isRemove;
        this.IsAdd = isAdd;
        this.IsImageUpdated = isImageUpdated;
        this.IsFieldsUpdated = isFieldsUpdated;
    }
}