using BillingService.WebApi.Controllers.AiGenerateController.Payloads;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BillingService.WebApi.Controllers.AiGenerateController;

[ApiController]
[Route("api/v1/ai")]
public class AiGenerateController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private const string _baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    public AiGenerateController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
    }

    [HttpPost]
    [Route("chat")]
    public async Task<IActionResult> AskAsync(
        AskPayloadInput input,
        CancellationToken cancellationToken)
    {
        var systemPrompt = @"
            Você é um chatbot interno de suporte técnico especializado apenas no sistema de emissão de Notas Fiscais desenvolvido pela empresa.
            REGRAS:
            - Responda somente dúvidas sobre o sistema, produtos, estoque, notas fiscais, endpoints e APIs internas. Tudo relacionado a este contexto.
            - Não responda perguntas fora deste contexto.
            - Se perguntarem algo fora do permitido, diga educadamente: 'Desculpe, posso ajudar apenas com dúvidas relacionadas ao sistema de Notas Fiscais.'
            - Seja claro, educado e objetivo.
            - Se não souber a resposta, diga que não sabe.";

        try
        {
            var apiKey = _configuration["GEMINI_API_KEY"];
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, "The Google AI API key was not configured.");

            var requestBody = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = systemPrompt },
                        new { text = input.Question }
                    }
                }
            }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
            request.Headers.Add("x-goog-api-key", apiKey);
            request.Content = JsonContent.Create(requestBody);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                return StatusCode((int)response.StatusCode, error);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            string answer = "Não foi possível obter uma resposta.";

            if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var text))
            {
                answer = text.GetString() ?? answer;
            }

            return Ok(new AskPayloadOutput(answer));
        }
        catch (Exception exception)
        {
            return StatusCode(500, $"Erro inesperado: {exception.Message}");
        }
    }
}
