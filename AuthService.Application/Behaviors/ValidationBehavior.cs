using System.Reflection;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;
using FluentValidation;
using MediatR;

namespace AuthService.Application.Behaviors
{
    /// <summary>
    /// Ejecuta todos los IValidator&lt;TRequest&gt; registrados antes del handler.
    /// Todos los comandos/queries de esta solución devuelven Result o Result&lt;T&gt;,
    /// así que en vez de lanzar ValidationException (que no encajaría con ese
    /// patrón y perdería el código 400 que ya da ResultExtensions.ToActionResult),
    /// construimos el Fail(...) correspondiente por reflexión y cortamos el pipeline.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var failures = validators
                .Select(v => v.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count == 0)
            {
                return await next();
            }

            var errorMessage = string.Join(" | ", failures.Select(f => f.ErrorMessage));

            return BuildFailureResult(errorMessage);
        }

        private static TResponse BuildFailureResult(string errorMessage)
        {
            var responseType = typeof(TResponse);

            if (responseType == typeof(Result))
            {
                return (TResponse)(object)Result.Fail(errorMessage);
            }

            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var failMethod = responseType.GetMethod(
                    nameof(Result.Fail),
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                    binder: null,
                    types: [typeof(string)],
                    modifiers: null)!;

                return (TResponse)failMethod.Invoke(null, [errorMessage])!;
            }

            throw new ValidationException(errorMessage);
        }
    }
}
