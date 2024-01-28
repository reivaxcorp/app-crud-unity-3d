/*
 * Licencia: Este código está bajo la Licencia MIT.
 * 
 * Puedes usar, copiar, modificar, fusionar, publicar, distribuir, sublicenciar y/o vender copias
 * del software, y permitir a las personas a las que se les proporcione el software que hagan lo mismo,
 * sujeto a las siguientes condiciones:
 * 
 * El aviso de copyright y el aviso de licencia anterior deben incluirse en todas las copias o porciones sustanciales del software.
 * 
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O IMPLÍCITA,
 * INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD, IDONEIDAD PARA UN PROPÓSITO PARTICULAR
 * Y NO INFRACCIÓN. EN NINGÚN CASO LOS AUTORES O TITULARES DE LOS DERECHOS DE AUTOR SERÁN RESPONSABLES
 * DE NINGÚN RECLAMO, DAÑO U OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO O DE OTRO MODO,
 * DERIVADOS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS NEGOCIOS EN EL SOFTWARE.
 * 
 * Organización: ReivaxCorp.
 */

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
 