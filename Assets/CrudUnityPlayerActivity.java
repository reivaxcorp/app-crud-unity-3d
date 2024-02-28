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
 
         if (requestCode == 123) { // Este código debe coincidir con el código de solicitud en C#
             if (resultCode == RESULT_OK) {
 
                 // Aquí puedes manejar el resultado, por ejemplo, obtener la URI del archivo seleccionado
                 if (data != null && data.getData() != null) {
                     String selectedFileUri = data.getData().toString();
                     Log.d(TAG, "URI del archivo seleccionado: " + selectedFileUri);
 
                     // Obtener bytes de la imagen
                     String fileNameWithBase64 = getFileNameAndBase64Data(selectedFileUri);
 
                     // Envía la URI del archivo a Unity, enviamos el resultado a un GameObject en nuestra jerarquía de escena.
                     // El primer argumento es "GameObject".
                     // El segundo argumento es "Nombre del método".
                     // El tercer argumento es el valor a enviar.
                     com.unity3d.player.UnityPlayer.UnitySendMessage("Manager", "ReceiveDataFromAndroid",
                             fileNameWithBase64);
 
                 }
             } else {
                 Log.d(TAG, "La selección de archivos fue cancelada.");
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
 