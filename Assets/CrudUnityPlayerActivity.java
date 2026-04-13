/*********************************************************************************
 * Descripción:            Activity que nos ayuda a comunicarnos con el sistema de android,
 *                         nos devolvera el resultado de la selección de archivos en nuestro Manager en Unity.
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


 package com.reivaxcorp.unityappcrud;

 import com.unity3d.player.UnityPlayerActivity;
 import android.content.Intent;
 import android.os.Bundle;
 import android.util.Log;
 import android.util.Base64;
 import java.io.ByteArrayOutputStream;
 import java.io.InputStream;
 import android.net.Uri;
 import android.database.Cursor;
 import android.provider.MediaStore;
 import android.content.ContentResolver;
 
 public class CrudUnityPlayerActivity extends UnityPlayerActivity {
 
     private static final String TAG = "CrudUnityPlayerActivity";
 
     protected void onCreate(Bundle savedInstanceState) {
         // Llama a UnityPlayerActivity.onCreate()
         super.onCreate(savedInstanceState);
     }
 
     // El resultado cuando el usuario elige una imagen de la galería, necesitamos enviar
     // la Uri a Unity para que podamos manejarla.
     @Override
        protected void onActivityResult(int requestCode, int resultCode, Intent data) {
            super.onActivityResult(requestCode, resultCode, data);

            if (requestCode == 123 && resultCode == RESULT_OK && data != null) {
                Uri selectedUri = data.getData();
                if (selectedUri != null) {
                    try {
                        // 1. Creamos un archivo temporal en el cache de la app
                        java.io.File tempFile = new java.io.File(getCacheDir(), "temp_upload.jpg");
                        
                        // 2. Copiamos los bytes desde el "content://" al archivo real
                        InputStream inputStream = getContentResolver().openInputStream(selectedUri);
                        java.io.FileOutputStream outputStream = new java.io.FileOutputStream(tempFile);
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = inputStream.read(buffer)) != -1) {
                            outputStream.write(buffer, 0, bytesRead);
                        }
                        outputStream.close();
                        inputStream.close();

                        // 3. Le pasamos a Unity la ruta del archivo real
                        String absolutePath = tempFile.getAbsolutePath();
                        Log.d(TAG, "Archivo copiado a cache: " + absolutePath);
                        
                        com.unity3d.player.UnityPlayer.UnitySendMessage("Manager", "ReceiveDataFromAndroid", absolutePath);

                    } catch (Exception e) {
                        Log.e(TAG, "Error copiando archivo: " + e.getMessage());
                    }
                }
            }
        }
 
     private String getFileNameAndBase64Data(String imageUri) {
         // Obtener el nombre del archivo y los bytes en Base64
         byte[] imageData = getBytesFromImage(imageUri);
         String base64Data = Base64.encodeToString(imageData, Base64.DEFAULT);
 
         // Concatenar el nombre del archivo y los datos en Base64
         String fileName = getFileNameFromUri(imageUri);
         return fileName + "|" + base64Data;
     }
 
     private byte[] getBytesFromImage(String imageUri) {
         try {
             // Obtener un InputStream desde la URI de la imagen
             InputStream inputStream = getContentResolver().openInputStream(Uri.parse(imageUri));
 
             // Leer los datos de la imagen en un array de bytes
             ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
             byte[] buffer = new byte[4096]; // Puedes ajustar el tamaño del buffer según tus necesidades
 
             int bytesRead;
             while ((bytesRead = inputStream.read(buffer)) != -1) {
                 byteArrayOutputStream.write(buffer, 0, bytesRead);
             }
 
             // Cerrar el InputStream
             inputStream.close();
 
             // Obtener los bytes de la imagen
             return byteArrayOutputStream.toByteArray();
         } catch (Exception e) {
             e.printStackTrace();
             return null;
         }
     }
 
     private String getFileNameFromUri(String uriString) {
         Uri uri = Uri.parse(uriString);
         String fileName = null;
 
         if (uri.getScheme().equals("content")) {
             // Si la URI es del tipo "content", intenta obtener el nombre a través de un
             // Cursor
             ContentResolver contentResolver = getContentResolver();
             Cursor cursor = contentResolver.query(uri, null, null, null, null);
 
             try {
                 if (cursor != null && cursor.moveToFirst()) {
                     int displayNameIndex = cursor.getColumnIndex(MediaStore.Images.Media.DISPLAY_NAME);
                     if (displayNameIndex != -1) {
                         fileName = cursor.getString(displayNameIndex);
                     }
                 }
             } finally {
                 if (cursor != null) {
                     cursor.close();
                 }
             }
         }
 
         if (fileName == null) {
             // Si no se pudo obtener el nombre del archivo a través del Cursor, intenta
             // extraerlo de la URI
             fileName = uri.getLastPathSegment();
         }
 
         return fileName;
     }
 
 }
 