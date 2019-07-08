﻿// Copyright (c) 2019 Lykke Corp.

using System;

namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface IOperationExecutionInfo<T> where T: class
    {
        string OperationName { get; }
        string Id { get; }
        DateTime LastModified { get; }

        T Data { get; }
    }
}