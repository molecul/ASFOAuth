using ArchiSteamFarm.IPC.Controllers.Api;
using ArchiSteamFarm.IPC.Responses;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;
using System.Net;
using System.Text.Json.Serialization;

namespace ASFOAuth.IPC;

public sealed record OAuthRequest
{
    [JsonInclude]
    [JsonRequired]
    public string? BotName { get; set; }
    [JsonInclude]
    [JsonRequired]
    public string? OAuthUrl { get; set; }
}

public sealed record OAuthResponse
{
    public bool Success { get; set; }
    public string? LoginUrl { get; set; }
}

public sealed record OpenIdRequest
{
    [JsonInclude]
    [JsonRequired]
    public string? BotName { get; set; }
    [JsonInclude]
    [JsonRequired]
    public string? OpenIdUrl { get; set; }
}

public sealed record OpenIdResponse
{
    public bool Success { get; set; }
    public string? LoginUrl { get; set; }
}

/// <summary>
/// 基础控制器
/// </summary>
[Route("/Api", Name = nameof(ASFOAuth))]
public class ASFOAuthController : ArchiController
{
    /// <summary>
    /// Steam登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [HttpPost("OAuth")]
    [SwaggerOperation(Summary = "Steam登录", Description = "通过SteamOAuth登录第三方网站")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, $"The request has failed, check {nameof(GenericResponse.Message)} from response body for actual reason. Most of the time this is ASF, understanding the request, but refusing to execute it due to provided reason.", typeof(GenericResponse))]
    [ProducesResponseType(typeof(GenericResponse<OAuthResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(GenericResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<GenericResponse>> Oauth([FromBody] OAuthRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrEmpty(request.BotName) || string.IsNullOrEmpty(request.OAuthUrl))
        {
            return BadRequest(new GenericResponse(false, "BotName or OAuthUrl can not be null"));
        }

        var bot = Bot.GetBot(request.BotName);
        if (bot == null)
        {
            return BadRequest(new GenericResponse(false, string.Format(CultureInfo.CurrentCulture, Strings.BotNotFound, request.BotName)));
        }

        var result = await Core.WebRequest.LoginViaSteamOAuth(bot, request.OAuthUrl).ConfigureAwait(false);

        var response = new OAuthResponse
        {
            Success = result.StartsWith("https"),
            LoginUrl = result,
        };

        return Ok(new GenericResponse<OAuthResponse>(response));
    }

    [HttpGet("OAuth/{botName:required}/{oAuthUrl:required}")]
    [HttpPost("OAuth/{botName:required}/{oAuthUrl:required}")]
    [SwaggerOperation(Summary = "Steam登录", Description = "通过SteamOAuth登录第三方网站")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, $"The request has failed, check {nameof(GenericResponse.Message)} from response body for actual reason. Most of the time this is ASF, understanding the request, but refusing to execute it due to provided reason.", typeof(GenericResponse))]
    [ProducesResponseType(typeof(GenericResponse<OAuthResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(GenericResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<GenericResponse>> Oauth([FromRoute] string botName, [FromRoute] string oAuthUrl)
    {
        if (string.IsNullOrEmpty(botName))
        {
            throw new ArgumentNullException(nameof(botName));
        }

        var bot = Bot.GetBot(botName);
        if (bot == null)
        {
            return BadRequest(new GenericResponse(false, string.Format(CultureInfo.CurrentCulture, Strings.BotNotFound, botName)));
        }

        if (string.IsNullOrEmpty(oAuthUrl))
        {
            return BadRequest(new GenericResponse(false, "OAuthUrl can not be null"));
        }

        var result = await Core.WebRequest.LoginViaSteamOAuth(bot, oAuthUrl).ConfigureAwait(false);

        var response = new OAuthResponse
        {
            Success = result.StartsWith("https"),
            LoginUrl = result,
        };

        return Ok(new GenericResponse<OAuthResponse>(response));
    }
    
    /// <summary>
    /// Steam登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [HttpPost("OpenId")]
    [SwaggerOperation(Summary = "Steam登录", Description = "通过SteamOpenId登录第三方网站")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, $"The request has failed, check {nameof(GenericResponse.Message)} from response body for actual reason. Most of the time this is ASF, understanding the request, but refusing to execute it due to provided reason.", typeof(GenericResponse))]
    [ProducesResponseType(typeof(GenericResponse<OpenIdResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(GenericResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<GenericResponse>> OpenId([FromBody] OpenIdRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrEmpty(request.BotName) || string.IsNullOrEmpty(request.OpenIdUrl))
        {
            return BadRequest(new GenericResponse(false, "BotName or OpenIdUrl can not be null"));
        }

        var bot = Bot.GetBot(request.BotName);
        if (bot == null)
        {
            return BadRequest(new GenericResponse(false, string.Format(CultureInfo.CurrentCulture, Strings.BotNotFound, request.BotName)));
        }

        var result = await Core.WebRequest.LoginViaSteamOpenId(bot, request.OpenIdUrl).ConfigureAwait(false);

        var response = new OpenIdResponse
        {
            Success = result.StartsWith("https"),
            LoginUrl = result,
        };

        return Ok(new GenericResponse<OpenIdResponse>(response));
    }

    [HttpGet("OpenId/{botName:required}/{OpenIdUrl:required}")]
    [HttpPost("OpenId/{botName:required}/{OpenIdUrl:required}")]
    [SwaggerOperation(Summary = "Steam登录", Description = "通过SteamOpenId登录第三方网站")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, $"The request has failed, check {nameof(GenericResponse.Message)} from response body for actual reason. Most of the time this is ASF, understanding the request, but refusing to execute it due to provided reason.", typeof(GenericResponse))]
    [ProducesResponseType(typeof(GenericResponse<OpenIdResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(GenericResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<GenericResponse>> OpenId([FromRoute] string botName, [FromRoute] string OpenIdUrl)
    {
        if (string.IsNullOrEmpty(botName))
        {
            throw new ArgumentNullException(nameof(botName));
        }

        var bot = Bot.GetBot(botName);
        if (bot == null)
        {
            return BadRequest(new GenericResponse(false, string.Format(CultureInfo.CurrentCulture, Strings.BotNotFound, botName)));
        }

        if (string.IsNullOrEmpty(OpenIdUrl))
        {
            return BadRequest(new GenericResponse(false, "OpenIdUrl can not be null"));
        }

        var result = await Core.WebRequest.LoginViaSteamOpenId(bot, OpenIdUrl).ConfigureAwait(false);

        var response = new OpenIdResponse
        {
            Success = result.StartsWith("https"),
            LoginUrl = result,
        };

        return Ok(new GenericResponse<OpenIdResponse>(response));
    }

}
