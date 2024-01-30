using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResult
{
    public void SetResultCrudUi(bool successful, string msj);
    public void SetResultWriteDocument(bool successful, string title, string body);
}