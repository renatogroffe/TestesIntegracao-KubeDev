using FluentValidation;
using APIAcoes.Models;

namespace APIAcoes.Validators;

public class AcaoValidator : AbstractValidator<Acao>
{
    public AcaoValidator()
    {
        RuleFor(c => c.Codigo).NotEmpty().WithMessage("Preencha o campo 'Codigo'")
            .MinimumLength(4).WithMessage("O campo 'Codigo' deve possuir no mínimo 4 caracteres")
            .MaximumLength(10).WithMessage("O campo 'Codigo' deve possuir no máximo 10 caracteres");

        RuleFor(c => c.Valor).NotEmpty().WithMessage("Preencha o campo 'Valor'")
            .GreaterThan(0).WithMessage("O campo 'Valor' deve ser maior do 0");
    }                
}