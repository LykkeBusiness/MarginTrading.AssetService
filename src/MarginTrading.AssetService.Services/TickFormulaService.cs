using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.TickFormula;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Extensions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class TickFormulaService : ITickFormulaService
    {
        private readonly ITickFormulaRepository _tickFormulaRepository;
        private readonly IAuditService _auditService;
        private readonly ICqrsMessageSender _cqrsMessageSender;
        private readonly IConvertService _convertService;

        public TickFormulaService(
            ITickFormulaRepository tickFormulaRepository,
            IAuditService auditService,
            ICqrsMessageSender cqrsMessageSender,
            IConvertService convertService)
        {
            _tickFormulaRepository = tickFormulaRepository;
            _auditService = auditService;
            _cqrsMessageSender = cqrsMessageSender;
            _convertService = convertService;
        }

        public Task<ITickFormula> GetByIdAsync(string id)
            => _tickFormulaRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<ITickFormula>> GetAllAsync()
            => _tickFormulaRepository.GetAllAsync();

        public async Task<Result<TickFormulaErrorCodes>> AddAsync(ITickFormula model, string username,
            string correlationId)
        {
            SetDefaultLadderAndTicksIfNeeded(model);

            var validationResult = ValidateLaddersAndTicks(model);

            if (validationResult.IsFailed)
                return validationResult;

            var addResult = await _tickFormulaRepository.AddAsync(model);

            if (addResult.IsFailed)
                return addResult;

            await _auditService.TryAudit(correlationId, username, model.Id, AuditDataType.TickFormula,
                model.ToJson());
            await PublishTickFormulaChangedEvent(null, model, username, correlationId, ChangeType.Creation);

            return new Result<TickFormulaErrorCodes>();
        }

        public async Task<Result<TickFormulaErrorCodes>> UpdateAsync(ITickFormula model, string username,
            string correlationId)
        {
            var currentSettings = await _tickFormulaRepository.GetByIdAsync(model.Id);

            if (currentSettings == null)
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.TickFormulaDoesNotExist);

            SetDefaultLadderAndTicksIfNeeded(model);

            var validationResult = ValidateLaddersAndTicks(model);

            if (validationResult.IsFailed)
                return validationResult;

            var updateResult = await _tickFormulaRepository.UpdateAsync(model);

            if (updateResult.IsFailed)
                return updateResult;

            await _auditService.TryAudit(correlationId, username, currentSettings.Id, AuditDataType.TickFormula,
                model.ToJson(), currentSettings.ToJson());
            await PublishTickFormulaChangedEvent(currentSettings, model, username, correlationId, ChangeType.Edition);

            return new Result<TickFormulaErrorCodes>();
        }

        public async Task<Result<TickFormulaErrorCodes>> DeleteAsync(string id, string username, string correlationId)
        {
            var existing = await _tickFormulaRepository.GetByIdAsync(id);

            if (existing == null)
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.TickFormulaDoesNotExist);

            if (await _tickFormulaRepository.AssignedToAnyProductAsync(id))
            {
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.CannotDeleteTickFormulaAssignedToAnyProduct);
            }
            var result = await _tickFormulaRepository.DeleteAsync(id);
            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, username, id, AuditDataType.TickFormula,
                    oldStateJson: existing.ToJson());  
                await PublishTickFormulaChangedEvent(existing, null, username, correlationId, ChangeType.Deletion);
            }

            return result;
        }

        private void SetDefaultLadderAndTicksIfNeeded(ITickFormula model)
        {
            if (model.PdlTicks != null && model.PdlTicks.Any() ||
                model.PdlLadders != null && model.PdlLadders.Any())
                return;

            model.PdlTicks = new List<decimal> {0.01M};
            model.PdlLadders = new List<decimal> {0};
        }

        private Result<TickFormulaErrorCodes> ValidateLaddersAndTicks(ITickFormula model)
        {
            if (model.PdlLadders.Count != model.PdlTicks.Count)
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.PdlLaddersAndTicksMustHaveEqualLengths);

            if (model.PdlLadders.Any(x => x < 0))
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes
                    .PdlLaddersValuesMustBeGreaterOrEqualToZero);

            if (model.PdlTicks.Any(x => x <= 0))
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.PdlTicksValuesMustBeGreaterThanZero);

            if (model.PdlLadders[0] != 0)
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.PdlLaddersMustStartFromZero);

            if (!model.PdlLadders.IsAscendingSortedWithNoDuplicates())
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes
                    .PdlLaddersMustBeInAscendingOrderWithoutDuplicates);

            if (!model.PdlTicks.IsAscendingSorted())
                return new Result<TickFormulaErrorCodes>(TickFormulaErrorCodes.PdlTicksMustBeInAscendingOrder);

            return new Result<TickFormulaErrorCodes>();
        }
        
        private async Task PublishTickFormulaChangedEvent(ITickFormula oldTickFormula,
            ITickFormula newTickFormula,
            string username, string correlationId, ChangeType changeType)
        {
            await _cqrsMessageSender.SendEvent(new TickFormulaChangedEvent()
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = correlationId,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldTickFormula = 
                    _convertService.Convert<ITickFormula, TickFormulaContract>(
                        oldTickFormula),
                NewTickFormula = 
                    _convertService.Convert<ITickFormula, TickFormulaContract>(
                        newTickFormula),
            });
        }
    }
}