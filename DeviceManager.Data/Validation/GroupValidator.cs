using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Data.Validation
{
    public class GroupValidator : AbstractValidator<Group>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            SetupNameRules();
            SetupDisplayNameRules();
            SetupLastModifiedRules();
            SetupCreatedRules();
            SetupUserIdRules();
        }

        private void SetupNameRules()
        {
            RuleFor(g => g.Name)
                .NotEmpty()
                .MaximumLength(GroupConfiguration.NameMaxLength);

            Transform(g => g, g => g)
                .MustAsync(async (g, ct) =>
                    await _unitOfWork.Groups.SingleOrDefaultAsync(x =>
                        x.Id != g.Id && x.Name == g.Name && x.UserId == g.UserId, ct) is null)
                .WithMessage("group must have unique name");
        }

        private void SetupDisplayNameRules()
        {
            RuleFor(g => g.DisplayName)
                .MaximumLength(GroupConfiguration.DisplayNameMaxLenght);
        }

        private void SetupLastModifiedRules()
        {
            RuleFor(g => g.LastModified)
                .GreaterThanOrEqualTo(g => g.Created)
                .Unless(g => g.LastModified is null);
        }

        private void SetupCreatedRules()
        {
            RuleFor(g => g.Created)
                .NotEmpty();
        }

        private void SetupUserIdRules()
        {
            RuleFor(g => g.UserId)
                .NotEmpty();
        }
    }
}