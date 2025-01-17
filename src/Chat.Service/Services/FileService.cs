﻿using Chat.Contracts.Files;
using Chat.Service.Application.Files.Commands;
using Chat.Service.Application.System.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Service.Services;

public class FileService : BaseService<FileService>, IFileService
{
    [Authorize]
    public async Task<ResultDto<string>> UploadAsync(IFormFile file)
    {
        // 判断当前文件大小
        if (file.Length > 1024 * 1024 * 10)
        {
            return "文件大小不能超过5M".Fail();
        }

        var command = new UploadCommand(file.OpenReadStream(), file.FileName);
        await PublishAsync(command);
        return command.Result.CreateResult();
    }

    [Authorize]
    public async Task<ResultDto<string>?> UploadBase64Async(UploadBase64Dto dto)
    {
        var bytes = Convert.FromBase64String(dto.Value);
        if (bytes.Length > 1024 * 1024 * 10)
        {
            return "文件大小不能超过5M".Fail();
        }

        using var stream = new MemoryStream(bytes);

        var command = new UploadCommand(stream, dto.FileName);
        await PublishAsync(command);
        return command.Result.CreateResult();
    }

    [Authorize]
    public async Task DeleteAsync(string uri)
    {
        var command = new DeleteFileSystemCommand(uri);
        await PublishAsync(command);
    }
}