namespace BillingService.WebApi.Controllers.AiGenerateController.Payloads;

public sealed class AskPayloadOutput
{
    public string Answer { get; init; }

    public AskPayloadOutput(string answer)
    {
        Answer = answer;
    }
}
