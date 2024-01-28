using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResultCrud { 
    public void SetResultCrudUi(bool successful, string msj);
    public void ResultPathReference(string pathReference);
}
