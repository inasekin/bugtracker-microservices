using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using VideoCallService.Domain.Interfaces;

namespace VideoCallService.Api.Hubs
{
    [AllowAnonymous] // Важно, чтобы разрешить анонимные подключения
    public class VideoHub : Hub
    {
        private readonly ILogger<VideoHub> _logger;
        private readonly IVideoRoomService _roomService;
        private static readonly Dictionary<string, string> _connectionMap = new Dictionary<string, string>();

        public VideoHub(ILogger<VideoHub> logger, IVideoRoomService roomService)
        {
            _logger = logger;
            _roomService = roomService;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Пользователь подключился: {ConnectionId}", Context.ConnectionId);
            
            // Отправляем приветственное сообщение для проверки соединения
            await Clients.Caller.SendAsync("ReceiveSystemMessage", "Подключение установлено");
            
            await base.OnConnectedAsync();
        }

        public async Task JoinRoom(string roomId, string userId, string username)
        {
            try
            {
                _logger.LogInformation("Попытка присоединения к комнате: {RoomId}, UserId: {UserId}, Username: {Username}", 
                    roomId, userId, username);
                
                // Сохраняем маппинг userId -> connectionId для дальнейшей адресации
                _connectionMap[userId] = Context.ConnectionId;
                    
                // Сначала добавляем в группу - это самое главное для работы
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                await Clients.Caller.SendAsync("ReceiveSystemMessage", $"Присоединен к комнате {roomId}");
                
                // Пытаемся найти комнату - но не выбрасываем исключение, чтобы не закрывать соединение
                try
                {
                    var room = await _roomService.GetRoomAsync(roomId);
                    if (room != null)
                    {
                        await _roomService.AddParticipantAsync(Guid.Parse(roomId), userId, username);
                        await Clients.Group(roomId).SendAsync("UserJoined", userId, username);
                        _logger.LogInformation("Пользователь {Username} присоединился к комнате {RoomId}", username, roomId);
                    }
                    else
                    {
                        _logger.LogWarning("Комната {RoomId} не найдена при попытке присоединения", roomId);
                        await Clients.Caller.SendAsync("RoomNotFound", roomId);
                    }
                }
                catch (Exception roomEx)
                {
                    _logger.LogError(roomEx, "Ошибка при обработке комнаты {RoomId}: {Message}", roomId, roomEx.Message);
                    await Clients.Caller.SendAsync("ErrorMessage", $"Ошибка при работе с комнатой: {roomEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при присоединении к комнате {RoomId}: {Message}", roomId, ex.Message);
                // Не выбрасываем исключение наверх, чтобы избежать закрытия соединения
                await Clients.Caller.SendAsync("ErrorMessage", $"Ошибка при присоединении: {ex.Message}");
            }
        }

        public async Task LeaveRoom(string roomId, string userId)
        {
            try
            {
                _logger.LogInformation("Пользователь {UserId} покидает комнату {RoomId}", userId, roomId);
                
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                
                // Удаляем маппинг userId -> connectionId
                if (_connectionMap.ContainsKey(userId))
                {
                    _connectionMap.Remove(userId);
                }
                
                // Отмечаем пользователя как покинувшего комнату
                try {
                    var room = await _roomService.GetRoomAsync(roomId);
                    if (room != null)
                    {
                        var participant = room.Participants?.Find(p => p.UserId.ToString() == userId);
                        if (participant != null)
                        {
                            participant.LeftAt = DateTime.UtcNow;
                            participant.IsActive = false;
                            
                            // Обновляем участника, но не закрываем комнату
                            await _roomService.UpdateParticipantStatusAsync(
                                Guid.Parse(roomId), 
                                Guid.Parse(userId), 
                                false);
                            
                            // Отправляем уведомление остальным участникам
                            await Clients.Group(roomId).SendAsync("UserLeft", userId);
                            _logger.LogInformation("Пользователь {UserId} покинул комнату {RoomId}", userId, roomId);
                        }
                        else
                        {
                            _logger.LogWarning("Участник {UserId} не найден в комнате {RoomId}", userId, roomId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Комната {RoomId} не найдена при выходе пользователя", roomId);
                    }
                }
                catch (Exception roomEx)
                {
                    _logger.LogError(roomEx, "Ошибка при обновлении статуса участника: {Message}", roomEx.Message);
                    // Продолжаем выполнение, чтобы не прерывать процесс выхода пользователя
                }
                
                // Отправляем сигнал клиенту для очистки ресурсов
                await Clients.Caller.SendAsync("LeaveConfirmed", roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выходе из комнаты: {Message}", ex.Message);
                // Пытаемся отправить ошибку клиенту
                await Clients.Caller.SendAsync("ErrorMessage", $"Ошибка при выходе из комнаты: {ex.Message}");
            }
        }

        public async Task SendSignal(string roomId, string userId, string signal, string targetUserId)
        {
            try
            {
                _logger.LogDebug("Отправка сигнала от {SenderId} к {TargetId} в комнате {RoomId}", userId, targetUserId, roomId);
                
                // Используем сохраненный ConnectionId вместо User
                if (_connectionMap.TryGetValue(targetUserId, out var connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveSignal", userId, signal);
                    _logger.LogDebug("Сигнал отправлен на ConnectionId: {ConnectionId}", connectionId);
                }
                else
                {
                    // Если ConnectionId не найден, отправляем сигнал через группу с указанием целевого userId
                    // Клиент должен проверять, предназначен ли сигнал ему
                    _logger.LogWarning("ConnectionId для пользователя {TargetId} не найден, отправка через группу", targetUserId);
                    await Clients.Group(roomId).SendAsync("ReceiveGroupSignal", userId, targetUserId, signal);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке сигнала: {Message}", ex.Message);
                // Не выбрасываем исключение, чтобы не разрывать соединение
                await Clients.Caller.SendAsync("ErrorMessage", $"Ошибка при отправке сигнала: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Обработка разрыва соединения
            if (exception != null)
            {
                _logger.LogWarning(exception, "Соединение разорвано с ошибкой: {ConnectionId}, {Message}", 
                    Context.ConnectionId, exception.Message);
            }
            else
            {
                _logger.LogInformation("Соединение разорвано: {ConnectionId}", Context.ConnectionId);
            }
            
            // Удаляем маппинг userId -> connectionId
            string disconnectedUserId = null;
            foreach (var kv in _connectionMap)
            {
                if (kv.Value == Context.ConnectionId)
                {
                    disconnectedUserId = kv.Key;
                    break;
                }
            }
            
            if (disconnectedUserId != null)
            {
                _connectionMap.Remove(disconnectedUserId);
                _logger.LogInformation("Удален маппинг для пользователя {UserId}", disconnectedUserId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
} 