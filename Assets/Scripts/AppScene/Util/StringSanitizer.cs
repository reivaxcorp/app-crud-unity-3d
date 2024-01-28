/*
 * Código de ejemplo para sanitizar cadenas en C#
 * Autor: OpenAI GPT-3 (https://www.openai.com/)
 * 
 * Este código es proporcionado como un ejemplo y no está asociado con ningún proyecto específico.
 * Puedes utilizar, modificar y distribuir este código de acuerdo con los términos de la licencia en tu proyecto.
 */

using System.Text.RegularExpressions;

public class StringSanitizer
{
    public static string SanitizeString(string input)
    {
        string allowedCharacters = "A-Za-z0-9_\\-\\.";

        // Eliminar caracteres no permitidos
        string sanitizedString = Regex.Replace(input, $"[^{Regex.Escape(allowedCharacters)}]", "");

        return sanitizedString;
    }
 
}
