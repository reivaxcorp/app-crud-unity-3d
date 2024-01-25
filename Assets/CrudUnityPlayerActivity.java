/*
 * Nombre del Script: CrudUnityPlayerActivity.cs
 * Descripción: Necesitamos saber el resultado de la selección de archivos en Android,
 * por eso, una vez que el usuario elige una imagen, enviamos el Uri string
 * y lo manejamos desde Unity.
 * 
 * Licencia: Este código está bajo la Licencia MIT
 * 
 * Puedes usar, copiar, modificar, fusionar, publicar, distribuir, sublicenciar y/o vender copias
 * del software, y permitir a las personas a quienes se les provea el software hacer lo mismo,
 * sujeto a las siguientes condiciones:
 * 
 * Aviso de derechos de autor y licencia anterior incluido en todas las copias o partes sustanciales del software.
 * 
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O IMPLÍCITA,
 * INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD, IDONEIDAD PARA UN PROPÓSITO
 * PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS DUEÑOS O AUTORES DEL SOFTWARE SERÁN RESPONSABLES
 * POR NINGUNA RECLAMACIÓN, DAÑO U OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO O
 * CUALQUIER OTRO MOTIVO, DERIVADO DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTRO TIPO DE ACCIONES
 * EN EL SOFTWARE.
 * 
 * Organización: ReivaxCorp.
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


  // El resultado cuando el usuario elige una imagén de la galeria, debemos enviar la Uri, a unity para que podamos manejarla.
  @Override
  protected void onActivityResult(int requestCode, int resultCode, Intent data) {
      super.onActivityResult(requestCode, resultCode, data);

      if (requestCode == 123) { // Este código debe coincidir con el código de solicitud en C#
          if (resultCode == RESULT_OK) {

              // Aquí puedes manejar el resultado, por ejemplo, obtener la URI del archivo seleccionado
              if (data != null && data.getData() != null) {
                  String selectedFileUri = data.getData().toString();
                  Log.d(TAG, "Selected File URI: " + selectedFileUri);

                  // Envia la URI del archivo a Unity, enviamos el resultado a un GameObject de nuestra Jerarquia de objeto en nuestra escena.
                  // Primer argumento "GameObject".
                  // Segundo Argumento "Nombre del Método".
                  // Tercer Argumento el valor a enviar. 
                  com.unity3d.player.UnityPlayer.UnitySendMessage("Menu", "ReceiveData", selectedFileUri);

              }
          } else {
              Log.d(TAG, "La seleccion del archivo fue cancelada.");
          }
      }
  }
}