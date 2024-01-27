/*
 * Script Name: CrudUnityPlayerActivity.cs
 * Description: We need to know the result of file selection on Android,
 * so once the user chooses an image, we send the Uri string
 * and handle it from Unity.
 * 
 * License: This code is under the MIT License.
 * 
 * You can use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the software, and allow persons to whom the software is furnished to do the same,
 * subject to the following conditions:
 * 
 * Copyright notice and the above license notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE
 * AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Organization: ReivaxCorp.
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
        // Calls UnityPlayerActivity.onCreate()
        super.onCreate(savedInstanceState);
    }

    // The result when the user chooses an image from the gallery, we need to send
    // the Uri to unity so we can handle it.
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == 123) { // This code should match the request code in C#
            if (resultCode == RESULT_OK) {

                // Here you can handle the result, for example, get the URI of the selected file
                if (data != null && data.getData() != null) {
                    String selectedFileUri = data.getData().toString();
                    Log.d(TAG, "Selected File URI: " + selectedFileUri);

                    // Obtener bytes de la imagen
                    String fileNameWithBase64 = getFileNameAndBase64Data(selectedFileUri);

                    // Send the file URI to Unity, we send the result to a GameObject in our scene
                    // hierarchy.
                    // First argument is "GameObject".
                    // Second Argument is "Method Name".
                    // Third Argument is the value to send.
                    com.unity3d.player.UnityPlayer.UnitySendMessage("Manager", "ReceiveDataFromAndroid",
                            fileNameWithBase64);

                }
            } else {
                Log.d(TAG, "File selection was canceled.");
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
