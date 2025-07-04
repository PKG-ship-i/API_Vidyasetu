using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Vidyasetu_API.Models;

namespace Vidyasetu_API.Common
{
    public class HelperService
    {
        private readonly VidyasetuAI_DevContext _db;
        private readonly IConfiguration _config;

        public HelperService(VidyasetuAI_DevContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public async Task<bool> IsDeviceAllowedAsync(long deviceId)
        {
            // Step 1: Check if device has >= 3 logs
            int logCount = await _db.DeviceLogDetails.CountAsync(log => log.DeviceId == deviceId);

            // Step 2: Get the device and its associated user
            var device = await _db.DeviceDetails
                .Where(d => d.Id == deviceId)
                .FirstOrDefaultAsync();

            if (device == null)
                return false; // Device not found - deny access

            if (device.UserId != null)
                return true; // User is associated with the device - allow acces

            if (logCount >= Convert.ToInt32(_config["AllowedRequestCount"]))
                return false; // No user, and already 3+ logs - deny access

            return true; // Less than 3 logs - allow
        }

        public async Task<DeviceLogDetail> AddNewDevicelog(DeviceLogDetail deviceLogDetail)
        {
            await _db.DeviceLogDetails.AddAsync(deviceLogDetail);
            await _db.SaveChangesAsync();
            return deviceLogDetail;
        }


        public  string GetDescriptionFromValue<TEnum>(int value) where TEnum : Enum
        {
            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
            var memberInfo = typeof(TEnum).GetMember(enumValue.ToString());

            if (memberInfo.Length > 0)
            {
                var attr = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
                return attr?.Description ?? enumValue.ToString();
            }

            return enumValue.ToString();
        }

    }
}
