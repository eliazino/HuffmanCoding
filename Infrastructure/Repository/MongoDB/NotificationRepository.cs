using Core.Application.DTOs.Filter;
using Core.Application.Interfaces.Repository;
using Core.Application.Interfaces.Repository.MongoDB;
using Core.Models.Entities;
using Core.Shared;
using Infrastructure.Persistence;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.MongoDB {
    [RegisterAsScoped]
    public class NotificationRepository : Repository<Notification>, INotificationRepository, Interfaces.MongoDB.IRepository<Notification> {
        public NotificationRepository(IMongoDb db) : base(db) { }

        public async Task<bool> createNotification(Notification report) {
            return await create(report);
        }

        public async Task<IEnumerable<Notification>> get(NotificationFilter filter) {
            List<Expression<Func<Notification, bool>>> conditions = new List<Expression<Func<Notification, bool>>>();
            Expression<Func<Notification, bool>> condition = null;
            if (filter.fieldIsSet(nameof(filter.startDate))) {
                if (!filter.fieldIsSet(nameof(filter.endDate))) { filter.endDate = Utilities.getTodayDate().unixTimestamp; }
                condition = f => f.dateCreated >= filter.startDate & filter.endDate <= filter.endDate;
                conditions.Add(condition);
            }
            if (filter.fieldIsSet(nameof(filter.targetID))) {
                condition = f => f.targetID == filter.targetID | f.targetIsPublic == true;
                conditions.Add(condition);
            }
            return await getByCondition(conditions);
        }
    }

    public class NotificationViewRepository : Repository<ViewHistory>, INotificationViewRepository, Interfaces.MongoDB.IRepository<ViewHistory> {
        public NotificationViewRepository(IMongoDb db) : base(db) { }

        public async Task<IEnumerable<ViewHistory>> getViews(string username, List<long> notificationID) {
            Expression<Func<ViewHistory, bool>> condition = T => T.username == username & notificationID.Contains(T.notifID);
            return await getByCondition(condition);
        }

        public async Task<bool> updateNoticationView(ViewHistory view) {
            Expression<Func<ViewHistory, bool>> condition = T => T.username == view.username & view.notifID == T.notifID;
            if (await isExist(condition))
                return true;
            return await create(view);
        }
    }

    public class NotificationTokenRepository : Repository<NotificationToken>, INotificationTokenRepository, Interfaces.MongoDB.IRepository<NotificationToken> {
        public NotificationTokenRepository(IMongoDb db) : base(db) { }        
        public async Task<IEnumerable<NotificationToken>> getToken(string username) {
            Expression<Func<NotificationToken, bool>> condition = T => T.username == username;
            return await getByCondition(condition);
        }

        public async Task<bool> updateNoticationToken(NotificationToken token) {
            Expression<Func<NotificationToken, bool>> condition = T => T.username == token.username;
            await delete(condition);
            return await create(token);
        }
    }
}
