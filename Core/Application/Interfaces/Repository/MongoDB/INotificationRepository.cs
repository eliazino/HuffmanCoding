using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository.MongoDB {
    public interface INotificationRepository {
        Task<bool> createNotification(Models.Entities.Notification report);
        Task<IEnumerable<Models.Entities.Notification>> get(NotificationFilter filter);
    }
    public interface INotificationTokenRepository {
        Task<IEnumerable<NotificationToken>> getToken(string username);
        Task<bool> updateNoticationToken(NotificationToken token);
    }
    public interface INotificationViewRepository {
        Task<bool> updateNoticationView(ViewHistory view);
        Task<IEnumerable<ViewHistory>> getViews(string username, List<long> notificationID);
    }
}
