/*
 * TimeUtils.cs
 * Script for resizing textures in Unity
 * Original Author: OpenAI GPT-3.5
 * Modified by [ReivaxCorp.]
 */

using System;

public class TimeUtils
{
    public static long GetTimeStampUnix()
    {
        // Obtener la marca de tiempo actual del dispositivo en formato Unix
        long timestampUnix = (long)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        return timestampUnix;
    }

    public static string ConvertTimeStampUnixToDate(long timestamp)
    {
        // Convertir el timestamp Unix a una fecha y hora
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);

        // Ahora, puedes formatear la fecha y hora según lo necesites
        string formattedDateTime = dateTime.ToString("dd/MM/yyyy HH:mm:ss");

        return formattedDateTime;
    }
}
