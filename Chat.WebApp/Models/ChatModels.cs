namespace Chat.WebApp.Models;

public sealed record CreateConversationResponse(string ConversationId);

public sealed record SendMessageRequest(string Message);

public sealed record ContinueAfterConsentRequest(string Message, string PreviousResponseId);

public static class ChatStatus
{
    public const string Completed = "completed";
    public const string OAuthConsentRequired = "oauth_consent_required";
}

public sealed record SendMessageResponse(
    string ConversationId,
    string ResponseId,
    string Status,
    string? OutputText = null,
    string? ConsentLink = null,
    string? ConsentServerLabel = null);
