# Serie de Tutoriales de Unity y Firebase: Creación, subida, actualización y borrado de imágenes. Asi como también escritura de documentos en Firebase

¡Bienvenido a la Serie de Tutoriales de Unity y Firebase: Texturas! En esta serie de tutoriales, aprenderás cómo leer, cargar, subir y borrar texturas en Unity 3D utilizando Firebase en una aplicación Android. Esta guía completa te llevará paso a paso a través de todo el proceso, desde la configuración de Firebase hasta la implementación de un sistema CRUD (create-read-update-delete) completo en tu aplicación Unity.

## Visión General

Esta serie de tutoriales está diseñada para ayudarte a comprender la integración entre Unity y Firebase, centrándose específicamente en el manejo de texturas dentro de tus proyectos de Unity. Al final de esta serie, podrás:

- Implementar un sistema de inicio de sesión de usuario para autenticar usuarios y gestionar permisos.
- Leer y cargar texturas desde Firebase Database.
- Subir texturas a Firebase Storage.
- Descargar texturas desde Firebase Storage.
- Borrar y actualizar texturas almacenadas en Firebase.

## Empezando

Para comenzar, asegúrate de tener los siguientes requisitos previos:

- Unity instalado en tu máquina de desarrollo.
- Un proyecto Firebase activo creado en la Consola de Firebase.
- Conocimientos básicos de Unity y programación en C#.
- Entorno de desarrollo de Android configurado si estás apuntando a dispositivos Android.

## Contenido del Repositorio

Este repositorio contiene todo el código fuente y los activos utilizados en la serie de tutoriales. Cada tutorial tendrá su propia rama correspondiente en el repositorio. Puedes encontrar el código de cada tutorial en su respectiva rama.

## Videos de los Tutoriales

La serie de tutoriales consta de varios videos, cada uno cubriendo un aspecto específico del trabajo con Firebase y texturas en Unity. Puedes encontrar la lista de reproducción completa [aquí](https://youtube.com/playlist?list=PLsvltDspdJcfiiWy2baA2MCNzBm32USjv&si=q7dTsZltYs-d3eOI).

## Uso del Código

- Derechos de Autor (c) [2024] ReivaxCorp
- Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
  de este software y de los archivos de documentación asociados (el "Software"),
  para tratar en el Software sin restricción, incluyendo sin limitación los
  derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
  sublicenciar, y/o vender copias del Software, y para permitir a las personas a
  quienes pertenezca el Software, sujeto a las siguientes condiciones:
- El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
  todas las copias o partes sustanciales del Software.
  EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
  IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
  IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
  AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
  RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
  O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
  TRATOS EN EL SOFTWARE.

## Reglas de Firebase RealtimeDatabase y Firebase Storage.

{
- RealtimeDAtabase:

  ```json
  "rules": {      
     "users": {
      "$userUid": {
       	"items": {
           ".read": "auth != null && $userUid === auth.uid && auth.token.email_verified == true",
           ".write": "auth != null && $userUid === auth.uid && auth.token.email_verified == true",
            "name": {".validate": "newData.isString() && newData.val().length <= 30"}
      	}
      }
    }
  }
  ```
  
- FirebaseStorage:

```markdown
service firebase.storage {
  match /b/{bucket}/o {
    match /users/{uidFolder}/imageItems/{fileName} {
      allow write: if request.auth.uid != null
      						 && request.auth.uid == uidFolder
                   && request.auth.token.email_verified == true
                   && request.resource.size < 5 * 1024 * 1024;
      allow read: if request.auth.uid != null
      						&& request.auth.uid == uidFolder
                  && request.auth.token.email_verified == true;
      allow delete:  if request.auth.uid != null
      						&& request.auth.uid == uidFolder
                  && request.auth.token.email_verified == true;
                   
    }
  }
}
```

## Conéctate Conmigo

Si tienes alguna pregunta, comentario o sugerencia, no dudes en ponerte en contacto:

- Correo electrónico: [reivaxcorp@email.com](mailto:reivaxcorp@gmail.com)
- LinkedIn: [Javier Monzón](https://www.linkedin.com/in/javier-monzón-a527952b5)
- YouTube: [ReivaxCorp.](https://www.youtube.com/channel/UCFaeV4z3zCTvF48ay6q7MtQ)
- Google Play: [ReivaxCorp](https://play.google.com/store/apps/dev?id=6165909766232622777)
- Sitio Web: [reivaxcorp.com](https://reivaxcorp.com)

¡Suerte con todo!
