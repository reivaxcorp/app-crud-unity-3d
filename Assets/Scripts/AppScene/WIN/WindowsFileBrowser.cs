using UnityEngine;
using System.Runtime.InteropServices; // Necesario para llamar a la DLL de Windows
using System;

public class WindowsFileBrowser : MonoBehaviour
{
    // Estructura necesaria para la API de Windows
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public string filter = null;
        public string customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public string file = null;
        public int maxFile = 0;
        public string fileTitle = null;
        public int maxFileTitle = 0;
        public string initialDir = null;
        public string title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public string defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public string templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    public void OpenExplorer(Action<string> onPathSelected)
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "Archivos de imagen (*.png;*.jpg)\0*.png;*.jpg\0Todos los archivos (*.*)\0*.*\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.persistentDataPath;
        ofn.title = "Selecciona una imagen para tu cubo";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008; // Explorer-style, file must exist

        if (GetOpenFileName(ofn))
        {
            Debug.Log("Archivo seleccionado: " + ofn.file);
            onPathSelected?.Invoke(ofn.file);
            // Aquí llamarías a tu lógica de cargar textura con la ruta: ofn.file
        }
    }
}