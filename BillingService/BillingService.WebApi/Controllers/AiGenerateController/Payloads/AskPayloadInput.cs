namespace BillingService.WebApi.Controllers.AiGenerateController.Payloads;

public sealed class AskPayloadInput
{
    public string Question { get; init; }

    public AskPayloadInput(string question)
    {
        Question = question;
    }
}
