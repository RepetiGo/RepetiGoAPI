using System.Collections.Concurrent;

namespace FlashcardApp.Api.Common
{
    public class ResetCode
    {
        private static ConcurrentDictionary<string, string> _codes = new ConcurrentDictionary<string, string>();
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public string GenerateCode(string email)
        {
            string code = RandomString(8);
            bool isNew = _codes.TryAdd(email, code);
            if (!isNew)
            {
                _codes.TryRemove(email, out _);
                _codes.TryAdd(email, code);
            }
            _ = Countdown(email, code);
            return code;
        }
        public bool ValidateResetCode(string email, string code)
        {
            if (email == null || code == null)
            {
                return false;
            }
            try
            {
                if (code == _codes[email].ToString())
                {
                    _codes.TryRemove(_codes[email], out _);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public async Task Countdown(string email, string code)
        {
            await Task.Delay(new TimeSpan(0, 15, 0)).ContinueWith(t =>
            {
                _codes.TryRemove(new KeyValuePair<string, string>(email, code));
            });
        }
    }
}
