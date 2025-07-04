namespace Vidyasetu_API.DTOs
{
    public class RegisterDeviceDto
    {
        public required string DeviceIdentifier { get; set; }
        public required string DeviceToken { get; set; }
    }
}
