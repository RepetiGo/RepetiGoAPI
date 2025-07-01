using System.Collections.Concurrent;

namespace RepetiGo.Api.Helpers
{
    public static class ResetCode
    {
        private static readonly ConcurrentDictionary<string, string> _codes = new ConcurrentDictionary<string, string>();

        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateCode(string email)
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

        public static bool ValidateResetCode(string email, string code)
        {
            if (email is null || code is null)
            {
                return false;
            }
            try
            {
                if (code == _codes[email].ToString())
                {
                    _codes.TryRemove(email, out _);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static async Task Countdown(string email, string code)
        {
            await Task.Delay(new TimeSpan(0, 15, 0)).ContinueWith(t =>
            {
                _codes.TryRemove(new KeyValuePair<string, string>(email, code));
            });
        }
    }
}
