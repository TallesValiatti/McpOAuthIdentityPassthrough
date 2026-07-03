using Chat.WebApp.Models;
using Chat.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chat.WebApp.Controllers;

[ApiController]
[Route("api/chat")]
public sealed class ChatApiController : ControllerBase
{
    private readonly FoundryChatService _chatService;

    public ChatApiController(FoundryChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("conversations")]
    [ProducesResponseType(typeof(CreateConversationResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateConversation()
    {
        CreateConversationResponse response =
            await _chatService.CreateConversationAsync();

        return Created(
            $"/api/chat/conversations/{response.ConversationId}",
            response);
    }

    [HttpPost("conversations/{conversationId}/responses")]
    [ProducesResponseType(typeof(SendMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(
        [FromRoute] string conversationId,
        [FromBody] SendMessageRequest request)
    {
        SendMessageResponse response =
            await _chatService.SendMessageAsync(conversationId, request);

        return Ok(response);
    }

    [HttpPost("conversations/{conversationId}/responses/continue")]
    [ProducesResponseType(typeof(SendMessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SendMessageResponse>> ContinueAfterConsent(
        [FromRoute] string conversationId,
        [FromBody] ContinueAfterConsentRequest request)
    {
        SendMessageResponse response =
            await _chatService.ContinueAfterConsentAsync(conversationId, request);

        return Ok(response);
    }
}
