#pragma warning disable OPENAI001

using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Chat.WebApp.Models;
using Microsoft.Extensions.Options;
using OpenAI.Responses;

namespace Chat.WebApp.Services;

public sealed class FoundryChatService
{
    private readonly AIProjectClient _projectClient;
    private readonly FoundryOptions _options;

    public FoundryChatService(
        AIProjectClient projectClient,
        IOptions<FoundryOptions> options)
    {
        _projectClient = projectClient;
        _options = options.Value;
    }

    public async Task<CreateConversationResponse> CreateConversationAsync()
    {
        var conversationsClient =
            _projectClient.ProjectOpenAIClient.GetProjectConversationsClient();

        ProjectConversation conversation =
            await conversationsClient.CreateProjectConversationAsync(
                new ProjectConversationCreationOptions());

        return new CreateConversationResponse(ConversationId: conversation.Id);
    }

    public async Task<SendMessageResponse> SendMessageAsync(
        string conversationId,
        SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
            throw new ArgumentException("Conversation id is required.", nameof(conversationId));

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Message is required.", nameof(request));

        ProjectResponsesClient responsesClient =
            _projectClient.ProjectOpenAIClient
                .GetProjectResponsesClientForAgent(_options.AgentName);

        CreateResponseOptions responseOptions = new()
        {
            AgentConversationId = conversationId
        };

        responseOptions.InputItems.Add(
            ResponseItem.CreateUserMessageItem(request.Message));

        ResponseResult response =
            await responsesClient.CreateResponseAsync(responseOptions);

        return BuildResponse(conversationId, response);
    }

    public async Task<SendMessageResponse> ContinueAfterConsentAsync(
        string conversationId,
        ContinueAfterConsentRequest request)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
            throw new ArgumentException("Conversation id is required.", nameof(conversationId));

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Message is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.PreviousResponseId))
            throw new ArgumentException("Previous response id is required.", nameof(request));

        ProjectResponsesClient responsesClient =
            _projectClient.ProjectOpenAIClient
                .GetProjectResponsesClientForAgent(_options.AgentName);

        // After the user consents, submit another response using the previous
        // response id and force the tool usage so Foundry resumes the MCP call
        // with the user's identity. The agent reference is bound to the client,
        // so the conversation id is intentionally omitted here.
        CreateResponseOptions responseOptions = new()
        {
            PreviousResponseId = request.PreviousResponseId,
            ToolChoice = ResponseToolChoice.CreateRequiredChoice()
        };

        responseOptions.InputItems.Add(
            ResponseItem.CreateUserMessageItem(request.Message));

        ResponseResult response =
            await responsesClient.CreateResponseAsync(responseOptions);

        return BuildResponse(conversationId, response);
    }

    private static SendMessageResponse BuildResponse(
        string conversationId,
        ResponseResult response)
    {
        // On the first use of an OAuth identity passthrough MCP tool, Foundry
        // returns an oauth_consent_request item instead of the final answer.
        // Convert each output item to its agent-specific representation to spot it.
        foreach (ResponseItem item in response.OutputItems)
        {
            if (item.AsAgentResponseItem() is OAuthConsentRequestResponseItem consentRequest)
            {
                return new SendMessageResponse(
                    ConversationId: conversationId,
                    ResponseId: response.Id,
                    Status: ChatStatus.OAuthConsentRequired,
                    ConsentLink: consentRequest.ConsentLink?.ToString(),
                    ConsentServerLabel: consentRequest.ServerLabel);
            }
        }

        return new SendMessageResponse(
            ConversationId: conversationId,
            ResponseId: response.Id,
            Status: ChatStatus.Completed,
            OutputText: response.GetOutputText()?.Trim());
    }
}
