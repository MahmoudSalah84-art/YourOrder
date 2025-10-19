using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Infrastructure.Data;

namespace yourOrder.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string action, string performedBy, string targetUser, string ip)
        {
            var log = new AuditLog
            {
                Action = action,
                PerformedBy = performedBy,
                TargetUser = targetUser,
                IPAddress = ip,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
