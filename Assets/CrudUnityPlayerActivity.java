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
 
 public class CrudUnityPlayerActivity extends UnityPlayerActivity {
 
   private static final String TAG = "CrudUnityPlayerActivity";
 
   protected void onCreate(Bundle savedInstanceState) {
     // Calls UnityPlayerActivity.onCreate()
     super.onCreate(savedInstanceState);
   }
 
   // The result when the user chooses an image from the gallery, we need to send the Uri to unity so we can handle it.
   @Override
   protected void onActivityResult(int requestCode, int resultCode, Intent data) {
       super.onActivityResult(requestCode, resultCode, data);
 
       if (requestCode == 123) { // This code should match the request code in C#
           if (resultCode == RESULT_OK) {
 
               // Here you can handle the result, for example, get the URI of the selected file
               if (data != null && data.getData() != null) {
                   String selectedFileUri = data.getData().toString();
                   Log.d(TAG, "Selected File URI: " + selectedFileUri);
 
                   // Send the file URI to Unity, we send the result to a GameObject in our scene hierarchy.
                   // First argument is "GameObject".
                   // Second Argument is "Method Name".
                   // Third Argument is the value to send.
                   com.unity3d.player.UnityPlayer.UnitySendMessage("Manager", "ReceiverMessagesFromAndroid", selectedFileUri);
 
               }
           } else {
               Log.d(TAG, "File selection was canceled.");
           }
       }
   }

   @Override
  protected void onDestroy() {
      super.onDestroy();
      // you can implement this, to delete temp image when the activity is destroy or other things.
     // for example: com.unity3d.player.UnityPlayer.UnitySendMessage("Manager", "DeleteTempImage");
  }
 }
 