using FluentEmail.Core;

namespace turfbooking.Helper
{
    public class SendMail
    {
        private readonly IFluentEmail _fluentEmail;

        public SendMail(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task<bool> SendAsync(string to, string subject, string body, bool isHtml = true)
        {
            var response = await _fluentEmail
                .To(to)
                .Subject(subject)
                .Body(body, isHtml)
                .SendAsync();
            return response.Successful;
        }
    }
}
