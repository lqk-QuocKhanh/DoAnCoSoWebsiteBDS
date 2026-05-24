namespace VietPropEstate.Application.Features.Chat.DTOs;

public sealed record OnlineStatusDto(string UserId, bool IsOnline, DateTime? LastSeen);
