﻿namespace RepetiGo.Api.Interfaces.Services
{
    public interface ISettingsService
    {
        Task<bool> CreateUserSettings(string userId);
    }
}
