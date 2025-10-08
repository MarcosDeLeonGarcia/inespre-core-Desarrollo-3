using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using INESPRE.Desktop.Core;

namespace INESPRE.Desktop.Api
{
    /// <summary>
    /// Cliente base para la API. 
    /// - Usa JSON (camelCase, case-insensitive)
    /// - Soporta 204 NoContent con métodos *Maybe*
    /// - Expone HttpClient protegido para clases hijas (ProductsApi, UsersApi, etc.)
    /// - Incluye helpers para querystrings y headers
    /// </summary>
    public class ApiClient
    {
        /// <summary>HttpClient compartido por APIs que heredan.</summary>
        protected readonly HttpClient _http;

        /// <summary>
        /// Opciones JSON comunes:
        /// - Case-insensitive
        /// - Permite leer números desde string
        /// - Converters para decimal/decimal? que aceptan coma o punto
        /// - Enum como string (opcional)
        /// </summary>
        protected static readonly JsonSerializerOptions _json =
            new(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                Converters =
                {
                    new DecimalFlexibleConverter(),
                    new NullableDecimalFlexibleConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

        public ApiClient()
        {
            var baseUrl = string.IsNullOrWhiteSpace(Session.ApiBase)
                ? "http://localhost/" // valor de seguridad para evitar Uri vacía
                : Session.ApiBase;

            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(100)
            };

            // Aceptamos JSON por defecto
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            RefreshAuth();
        }

        /// <summary>Actualiza el header Authorization según el Session.Token actual.</summary>
        public void RefreshAuth()
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(Session.Token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Session.Token);
        }

        /// <summary>Permite establecer (o limpiar) el Bearer token explícitamente.</summary>
        public void SetBearer(string? token)
        {
            // No escribimos en Session.Token (no tiene setter público).
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        // ==============================================================
        // Helpers privados
        // ==============================================================

        private static bool HasJsonBody(HttpResponseMessage res)
        {
            if (res.StatusCode == HttpStatusCode.NoContent) return false;
            if (res.Content is null) return false;

            if (res.Content.Headers.ContentLength.HasValue &&
                res.Content.Headers.ContentLength.Value == 0)
                return false;

            var media = res.Content.Headers.ContentType?.MediaType ?? "";
            return media.Contains("json", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task ThrowHttpError(HttpResponseMessage res)
        {
            var text = "";
            try { text = await res.Content.ReadAsStringAsync(); } catch { /* ignore */ }
            throw new HttpRequestException($"HTTP {(int)res.StatusCode} {res.ReasonPhrase}: {text}");
        }

        // ==============================================================
        // GET
        // ==============================================================

        public async Task<T> GetAsync<T>(string path)
        {
            var res = await _http.GetAsync(path);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);

            var obj = await res.Content.ReadFromJsonAsync<T>(_json);
            if (obj is null)
                throw new HttpRequestException("Respuesta JSON vacía o inválida.");
            return obj;
        }

        /// <summary>
        /// Versión GET que devuelve default(T) si no hay cuerpo JSON (204 o sin JSON).
        /// </summary>
        public async Task<T?> GetMaybeAsync<T>(string path)
        {
            var res = await _http.GetAsync(path);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);

            return HasJsonBody(res)
                ? await res.Content.ReadFromJsonAsync<T>(_json)
                : default;
        }

        // ==============================================================
        // POST
        // ==============================================================

        public async Task<TResp> PostAsync<TReq, TResp>(string path, TReq body)
        {
            var res = await _http.PostAsJsonAsync(path, body, _json);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);

            var obj = await res.Content.ReadFromJsonAsync<TResp>(_json);
            if (obj is null)
                throw new HttpRequestException("Respuesta JSON vacía o inválida.");
            return obj;
        }

        public async Task PostAsync<TReq>(string path, TReq body)
        {
            var res = await _http.PostAsJsonAsync(path, body, _json);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);
        }

        /// <summary>
        /// POST que devuelve default(TResp) si el servidor no retorna JSON (204/NoContent).
        /// </summary>
        public async Task<TResp?> PostMaybeAsync<TReq, TResp>(string path, TReq body)
        {
            var res = await _http.PostAsJsonAsync(path, body, _json);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);

            return HasJsonBody(res)
                ? await res.Content.ReadFromJsonAsync<TResp>(_json)
                : default;
        }

        // ==============================================================
        // PUT
        // ==============================================================

        public async Task<TResp> PutAsync<TReq, TResp>(string path, TReq body)
        {
            var res = await _http.PutAsJsonAsync(path, body, _json);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);

            var obj = await res.Content.ReadFromJsonAsync<TResp>(_json);
            if (obj is null)
                throw new HttpRequestException("Respuesta JSON vacía o inválida.");
            return obj;
        }

        public async Task PutAsync<TReq>(string path, TReq body)
        {
            var res = await _http.PutAsJsonAsync(path, body, _json);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);
        }

        /// <summary>
        /// PUT que devuelve default(TResp) si el servidor no retorna JSON (204/NoContent).
        /// </summary>
        public async Task<TResp?> PutMaybeAsync<TReq, TResp>(string path, TReq body)
        {
            var res = await _http.PutAsJsonAsync(path, body, _json);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);

            return HasJsonBody(res)
                ? await res.Content.ReadFromJsonAsync<TResp>(_json)
                : default;
        }

        // ==============================================================
        // DELETE
        // ==============================================================

        public async Task DeleteAsync(string path)
        {
            var res = await _http.DeleteAsync(path);
            if (!res.IsSuccessStatusCode) await ThrowHttpError(res);
        }

        // ==============================================================
        // UTILIDADES
        // ==============================================================

        /// <summary>
        /// Construye una URL con querystring a partir de un diccionario.
        /// Ignora pares con valores nulos o vacíos.
        /// </summary>
        public static string BuildUrl(string path, IDictionary<string, string?>? query)
        {
            if (query is null || query.Count == 0) return path;

            var sb = new StringBuilder(path);
            var first = !path.Contains('?');

            foreach (var kv in query)
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) continue;
                sb.Append(first ? '?' : '&');
                sb.Append(Uri.EscapeDataString(kv.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kv.Value!));
                first = false;
            }
            return sb.ToString();
        }

        /// <summary>Agrega o reemplaza un header arbitrario en el HttpClient.</summary>
        public void AddOrReplaceHeader(string name, string? value)
        {
            _http.DefaultRequestHeaders.Remove(name);
            if (!string.IsNullOrWhiteSpace(value))
                _http.DefaultRequestHeaders.Add(name, value);
        }
    }

    // =========================================================
    // CONVERTERS para decimales (aceptan "20,50" y "20.50")
    // =========================================================
    internal sealed class DecimalFlexibleConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetDecimal(out var d)) return d;
                throw new JsonException("Número decimal inválido.");
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString() ?? "";
                s = s.Trim();

                // Acepta coma o punto
                s = s.Replace(',', '.');

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    return d;

                // Último intento con cultura actual
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    return d;

                throw new JsonException($"No se puede convertir \"{s}\" a decimal.");
            }

            throw new JsonException($"Token {reader.TokenType} inesperado para decimal.");
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }

    internal sealed class NullableDecimalFlexibleConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            var conv = new DecimalFlexibleConverter();
            return conv.Read(ref reader, typeof(decimal), options);
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue) writer.WriteNumberValue(value.Value);
            else writer.WriteNullValue();
        }
    }
}
