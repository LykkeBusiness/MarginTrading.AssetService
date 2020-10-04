using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;

namespace MarginTrading.AssetService.Services
{
    public delegate Task<Result<TValue, TError>> ValidateAsync<TValue, TError>(TValue value, string userName,
        string correlationId, TValue existing = null)
        where TValue : class
        where TError : struct, Enum;

    public class ValidationChainEngine<TValue, TError>
        where TValue : class
        where TError : struct, Enum
    {
        private readonly List<ValidateAsync<TValue, TError>> _rules =
            new List<ValidateAsync<TValue, TError>>();

        public void AddValidation(ValidateAsync<TValue, TError> validate)
        {
            _rules.Add(validate);
        }

        public async Task<Result<TValue, TError>> ValidateAllAsync(TValue value, string userName,
            string correlationId,
            TValue existing = null)
        {
            var i = value;
            foreach (var validate in _rules)
            {
                var result = await validate(i, userName, correlationId, existing);

                if (result.IsFailed) return result;
                i = result.Value;
            }

            return new Result<TValue, TError>(i);
        }
    }
}