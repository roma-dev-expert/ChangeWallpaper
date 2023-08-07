﻿using Serilog;

namespace ChangeWallpaper.Services
{
    public abstract class BaseService
    {
        protected readonly ILogger Logger;

        protected BaseService(ILogger logger)
        {
            Logger = logger;
        }
    }
}
