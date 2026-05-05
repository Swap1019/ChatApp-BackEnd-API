namespace ChatApp.Application.Authorization.Common
{
    public interface IPolicy<TRequest>
    {
        Task<PolicyResult> EvaluateAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}

