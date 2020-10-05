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
        private readonly ICqrsEntityChangedSender _entityChangedSender;

        public TickFormulaService(
            ITickFormulaRepository tickFormulaRepository,
            IAuditService auditService,
            ICqrsEntityChangedSender entityChangedSender)
        {
            _tickFormulaRepository = tickFormulaRepository;
            _auditService = auditService;
            _entityChangedSender = entityChangedSender;
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
            await _entityChangedSender.SendEntityCreatedEvent<ITickFormula, TickFormulaContract, TickFormulaChangedEvent>(
                model, username, correlationId);
            
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
            await _entityChangedSender.SendEntityEditedEvent<ITickFormula, TickFormulaContract, TickFormulaChangedEvent>(
                currentSettings, model, username, correlationId);

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
                await _entityChangedSender
                    .SendEntityDeletedEvent<ITickFormula, TickFormulaContract, TickFormulaChangedEvent>(existing,
                        username, correlationId);
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
    }
}