﻿using System.Net.Http.Headers;
using Chat.Service.Application.Users.Commands;
using Chat.Service.Infrastructure.Helper;

namespace Chat.Service.Services;

public class AuthService : BaseService<AuthService>, IAuthService
{
    public async Task<ResultDto<string>> CreateAsync(string account, string password)
    {
        var query = new AuthQuery(account, password);
        await PublishAsync(query);

        var jwt = GetService<IOptions<JwtOptions>>()?.Value;

        var claims = JwtHelper.GetClaimsIdentity(query.Result);

        var token = JwtHelper.GeneratorAccessToken(claims, jwt);

        return token.CreateResult();
    }

    public async Task<ResultDto<string>> GithubAuthAsync(string accessToken)
    {
        using var http = new HttpClient();
        var github = GetOptions<GithubOptions>();
        http.DefaultRequestHeaders.Add("User-Agent", "Chat");
        http.DefaultRequestHeaders.Add("Accept", "application/json");
        var response =
            await http.PostAsync(
                $"https://github.com/login/oauth/access_token?code={accessToken}&client_id={github?.ClientId}&client_secret={github?.ClientSecrets}",
                null);
        var result = await response.Content.ReadFromJsonAsync<GitTokenDto>();
        if (result is null) throw new Exception("Github授权失败");

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.access_token);
        var githubUser = await http.GetFromJsonAsync<GithubUserDto>("https://api.github.com/user");
        if (githubUser is null) throw new Exception("Github授权失败");

        var query = new AuthTypeQuery("Github", githubUser.id.ToString());
        await PublishAsync(query);

        if (query.Result is null)
        {
            var command = new CreateUserCommand(new CreateUserDto
            {
                Account = "chat_" + StringHelper.RandomString(8),
                Avatar = "/favicon.png",
                Password = "Aa123456",
                Name = githubUser.name,
                GithubId = githubUser.id.ToString()
            });

            await PublishAsync(command);
            query.Result = command.Result;
        }

        var claims = JwtHelper.GetClaimsIdentity(query.Result);
        var jwt = GetService<IOptions<JwtOptions>>()?.Value;

        var token = JwtHelper.GeneratorAccessToken(claims, jwt);

        return token.CreateResult();
    }

    public async Task<ResultDto<string>> GiteeAuthAsync(string accessToken)
    {
        try
        {
            using var http = new HttpClient();
            var gitee = GetOptions<GiteeOptions>();
            var url =
                $"https://gitee.com/oauth/token?grant_type=authorization_code&redirect_uri={gitee.redirectUri}&response_type=code&code={accessToken}&client_id={gitee.ClientId}&client_secret={gitee.ClientSecrets}";

            Logger.LogWarning("Gitee授权 {url}", url);
            var response =
                await http.PostAsync(url,
                    null);

            var json = await response.Content.ReadAsStringAsync();

            Logger.LogWarning("Gitee授权 {json}", json);
            var result = JsonSerializer.Deserialize<GitTokenDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (result is null) throw new Exception("Gitee授权失败");

            var githubUser =
                await http.GetFromJsonAsync<GiteeDto>("https://gitee.com/api/v5/user?access_token=" +
                                                      result.access_token);
            if (githubUser is null) throw new Exception("Gitee授权失败");

            var query = new AuthTypeQuery("Gitee", githubUser.id.ToString());
            await PublishAsync(query);

            if (query.Result is null)
            {
                var command = new CreateUserCommand(new CreateUserDto
                {
                    Account = "chat_" + StringHelper.RandomString(8),
                    Avatar = githubUser.avatar_url,
                    Name = githubUser.name,
                    Password = "Aa123456",
                    GiteeId = githubUser.id.ToString()
                });

                await PublishAsync(command);
                query.Result = command.Result;
            }
            
            var claims = JwtHelper.GetClaimsIdentity(query.Result);
            var jwt = GetService<IOptions<JwtOptions>>()?.Value;

            var token = JwtHelper.GeneratorAccessToken(claims, jwt);

            return token.CreateResult();
        }
        catch (Exception e)
        {
            Logger.LogError("Gitee授权失败 {e}", e);
            return "".CreateResult("500", e.Message);
        }
    }

}